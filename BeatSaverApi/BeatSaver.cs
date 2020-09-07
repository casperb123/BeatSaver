using BeatSaverApi.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
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

        public async Task<BeatSaverMaps> GetBeatSaverMaps(MapSort mapSort, int page = 0)
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

                BeatSaverMaps beatSaverMaps = JsonConvert.DeserializeObject<BeatSaverMaps>(json);
                string[] songsDownloaded = Directory.GetDirectories(songsPath);

                foreach (Doc song in beatSaverMaps.docs)
                    foreach (string directory in Directory.GetDirectories(songsPath))
                        song.IsDownloaded = songsDownloaded.Any(x => new DirectoryInfo(x).Name.Split(" ")[0] == song.key);

                return beatSaverMaps;
            }
        }

        public async Task<BeatSaverMaps> GetBeatSaverMaps(string query, int page = 0)
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.Headers.Add(HttpRequestHeader.UserAgent, "BeatSaverApi");
                string json = await webClient.DownloadStringTaskAsync($"{beatSaverSearchApi}/{page}?q={query}");

                BeatSaverMaps beatSaverMaps = JsonConvert.DeserializeObject<BeatSaverMaps>(json);
                string[] songsDownloaded = Directory.GetDirectories(songsPath);

                foreach (Doc song in beatSaverMaps.docs)
                {
                    foreach (string directory in Directory.GetDirectories(songsPath))
                        song.IsDownloaded = songsDownloaded.Any(x => new DirectoryInfo(x).Name == song.hash);
                }

                return beatSaverMaps;
            }
        }

        public async Task DownloadSong(Doc song)
        {
            string songName = song.name;
            string levelAuthorName = song.metadata.levelAuthorName;

            foreach (string character in excludedCharacters)
            {
                songName = songName.Replace(character, "");
                levelAuthorName = levelAuthorName.Replace(character, "");
            }

            string downloadFilePath = $@"{downloadPath}\{song.key}.zip";
            string downloadString = $"{beatSaver}{song.downloadURL}";
            string extractPath = $@"{songsPath}\{song.key} ({songName} - {levelAuthorName})";

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

        public void DeleteSong(Doc song)
        {
            if (song.IsDownloaded)
            {
                string directory = Directory.GetDirectories(songsPath).FirstOrDefault(x => new DirectoryInfo(x).Name.Split(" ")[0] == song.key);

                if (!string.IsNullOrEmpty(directory))
                    Directory.Delete(directory, true);

                song.IsDownloaded = false;
            }
        }
    }
}
