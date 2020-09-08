using BeatSaverApi.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace BeatSaverApi
{
    public class BeatSaver
    {
        private readonly string beatSaver;
        private readonly string beatSaverApi;
        private readonly string beatSaverHotApi;
        private readonly string beatSaverRatingApi;
        private readonly string beatSaverLatestApi;
        private readonly string beatSaverDownloadsApi;
        private readonly string beatSaverPlaysApi;
        private readonly string beatSaverSearchApi;
        private readonly string beatSaverDetailsApi;
        private readonly string downloadPath;
        private string songsPath;

        private List<string> excludedCharacters = new List<string>
        {
            "<",
            ">",
            ":",
            "/",
            @"\",
            "|",
            "?",
            "*"
        };

        public event EventHandler<DownloadCompletedEventArgs> DownloadStarted;
        public event EventHandler<DownloadCompletedEventArgs> DownloadCompleted;

        public BeatSaver(string songsPath)
        {
            beatSaver = "https://beatsaver.com";
            beatSaverApi = $"{beatSaver}/api";
            beatSaverHotApi = $"{beatSaverApi}/maps/hot";
            beatSaverRatingApi = $"{beatSaverApi}/maps/rating";
            beatSaverLatestApi = $"{beatSaverApi}/maps/latest";
            beatSaverDownloadsApi = $"{beatSaverApi}/maps/downloads";
            beatSaverPlaysApi = $"{beatSaverApi}/maps/plays";
            beatSaverSearchApi = $"{beatSaverApi}/search/text";
            beatSaverDetailsApi = $"{beatSaverApi}/maps/detail";
            this.songsPath = songsPath;

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            downloadPath = $@"{appData}\BeatSaverApi";

            if (!Directory.Exists(downloadPath))
                Directory.CreateDirectory(downloadPath);

            DownloadCompleted += BeatSaver_DownloadCompleted;
            DownloadStarted += BeatSaver_DownloadStarted;
        }

        private void BeatSaver_DownloadStarted(object sender, DownloadCompletedEventArgs e)
        {
            e.Song.IsDownloading = true;
        }

        private void BeatSaver_DownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            e.Song.IsDownloading = false;
            e.Song.IsDownloaded = true;
        }

        public async Task<OnlineBeatMaps> GetOnlineBeatmaps(MapSort mapSort, int page = 0)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, "BeatSaverApi");
                    string json = null;

                    switch (mapSort)
                    {
                        case MapSort.Hot:
                            json = await webClient.DownloadStringTaskAsync($"{beatSaverHotApi}/{page}");
                            break;
                        case MapSort.Rating:
                            json = await webClient.DownloadStringTaskAsync($"{beatSaverRatingApi}/{page}");
                            break;
                        case MapSort.Latest:
                            json = await webClient.DownloadStringTaskAsync($"{beatSaverLatestApi}/{page}");
                            break;
                        case MapSort.Downloads:
                            json = await webClient.DownloadStringTaskAsync($"{beatSaverDownloadsApi}/{page}");
                            break;
                        case MapSort.Plays:
                            json = await webClient.DownloadStringTaskAsync($"{beatSaverPlaysApi}/{page}");
                            break;
                        default:
                            break;
                    }

                    OnlineBeatMaps beatSaverMaps = JsonConvert.DeserializeObject<OnlineBeatMaps>(json);
                    string[] songsDownloaded = Directory.GetDirectories(songsPath);

                    foreach (OnlineBeatMap song in beatSaverMaps.Maps)
                        foreach (string directory in Directory.GetDirectories(songsPath))
                            song.IsDownloaded = songsDownloaded.Any(x => new DirectoryInfo(x).Name.Split(" ")[0] == song.Key);

                    return beatSaverMaps;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OnlineBeatMaps> GetOnlineBeatmaps(string query, int page = 0)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, "BeatSaverApi");
                    string json = await webClient.DownloadStringTaskAsync($"{beatSaverSearchApi}/{page}?q={query}");

                    OnlineBeatMaps beatSaverMaps = JsonConvert.DeserializeObject<OnlineBeatMaps>(json);
                    string[] songsDownloaded = Directory.GetDirectories(songsPath);

                    foreach (OnlineBeatMap song in beatSaverMaps.Maps)
                    {
                        foreach (string directory in Directory.GetDirectories(songsPath))
                            song.IsDownloaded = songsDownloaded.Any(x => new DirectoryInfo(x).Name == song.Hash);
                    }

                    return beatSaverMaps;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<LocalBeatMaps> GetLocalBeatmaps(string songsPath, int page = 0)
        {
            LocalBeatMaps localBeatMaps = new LocalBeatMaps();
            List<string> songs = Directory.GetDirectories(songsPath).ToList();

            foreach (string songFolder in songs)
            {
                string infoFile = $@"{songFolder}\info.dat";
                string key = new DirectoryInfo(songFolder).Name.Split(" ")[0];

                if (!File.Exists(infoFile))
                    continue;

                string json = await File.ReadAllTextAsync(infoFile);
                LocalBeatMap beatMap = JsonConvert.DeserializeObject<LocalBeatMap>(json);

                beatMap.CoverImagePath = $@"{songFolder}\{beatMap.CoverImageFilename}";
                beatMap.Key = key;

                int index = songs.IndexOf(songFolder);
                if (index > 0 && index % 10 == 0)
                {
                    beatMap.Page = index / 10;
                    localBeatMaps.LastPage++;
                }

                if (page > 0)
                    localBeatMaps.PrevPage = page - 1;
                if (page < localBeatMaps.LastPage)
                    localBeatMaps.NextPage = page + 1;

                DifficultyBeatmapSet difficultyBeatmapSet = beatMap.DifficultyBeatmapSets[0];
                if (difficultyBeatmapSet.DifficultyBeatmaps.Any(x => x.Difficulty == "Easy"))
                    beatMap.Easy = true;
                if (difficultyBeatmapSet.DifficultyBeatmaps.Any(x => x.Difficulty == "Normal"))
                    beatMap.Normal = true;
                if (difficultyBeatmapSet.DifficultyBeatmaps.Any(x => x.Difficulty == "Hard"))
                    beatMap.Hard = true;
                if (difficultyBeatmapSet.DifficultyBeatmaps.Any(x => x.Difficulty == "Expert"))
                    beatMap.Expert = true;
                if (difficultyBeatmapSet.DifficultyBeatmaps.Any(x => x.Difficulty == "ExpertPlus"))
                    beatMap.ExpertPlus = true;

                OnlineBeatMap songDetails = await GetBeatmap(key);
                if (songDetails != null)
                    beatMap.Downloads = songDetails.Stats.Downloads;

                localBeatMaps.Maps.Add(beatMap);
            }

            return localBeatMaps;
        }

        public async Task DownloadSong(OnlineBeatMap song)
        {
            string songName = song.Name;
            string levelAuthorName = song.Metadata.LevelAuthorName;

            foreach (string character in excludedCharacters)
            {
                songName = songName.Replace(character, "");
                levelAuthorName = levelAuthorName.Replace(character, "");
            }

            string downloadFilePath = $@"{downloadPath}\{song.Key}.zip";
            string downloadString = $"{beatSaver}{song.DownloadURL}";
            string extractPath = $@"{songsPath}\{song.Key} ({songName} - {levelAuthorName})";

            if (!Directory.Exists(extractPath))
                Directory.CreateDirectory(extractPath);

            using (WebClient webClient = new WebClient())
            {
                webClient.Headers.Add(HttpRequestHeader.UserAgent, "BeatSaverApi");
                song.IsDownloading = true;
                DownloadStarted?.Invoke(this, new DownloadCompletedEventArgs(song));
                await webClient.DownloadFileTaskAsync(new Uri(downloadString), downloadFilePath);
                ZipFile.ExtractToDirectory(downloadFilePath, extractPath);
                File.Delete(downloadFilePath);

                DownloadCompleted?.Invoke(this, new DownloadCompletedEventArgs(song));
            }
        }

        public void DeleteSong(OnlineBeatMap song)
        {
            if (song.IsDownloaded)
            {
                string directory = Directory.GetDirectories(songsPath).FirstOrDefault(x => new DirectoryInfo(x).Name.Split(" ")[0] == song.Key);

                if (!string.IsNullOrEmpty(directory))
                    Directory.Delete(directory, true);

                song.IsDownloaded = false;
            }
        }

        public async Task<OnlineBeatMap> GetBeatmap(string key)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, "BeatSaverApi");
                    string json = await webClient.DownloadStringTaskAsync($"{beatSaverDetailsApi}/{key}");
                    return JsonConvert.DeserializeObject<OnlineBeatMap>(json);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
