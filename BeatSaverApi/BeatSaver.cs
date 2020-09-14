﻿using BeatSaverApi.Entities;
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
using System.Runtime.InteropServices;
using System.Text;
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
        private readonly string songsPath;
        private readonly List<string> excludedCharacters;

        public event EventHandler<DownloadStartedEventArgs> DownloadStarted;
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

            excludedCharacters = new List<string>
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

            DownloadCompleted += BeatSaver_DownloadCompleted;
            DownloadStarted += BeatSaver_DownloadStarted;
        }

        private void BeatSaver_DownloadStarted(object sender, DownloadStartedEventArgs e)
        {
            e.Song.IsDownloading = true;
        }

        private void BeatSaver_DownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            e.Song.IsDownloading = false;
            e.Song.IsDownloaded = true;
        }

        public async Task<OnlineBeatmaps> GetOnlineBeatmaps(MapSort mapSort, int page = 0)
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

                    OnlineBeatmaps beatSaverMaps = JsonConvert.DeserializeObject<OnlineBeatmaps>(json);
                    string[] songsDownloaded = Directory.GetDirectories(songsPath);

                    foreach (OnlineBeatmap song in beatSaverMaps.Maps)
                        foreach (string directory in Directory.GetDirectories(songsPath))
                            song.IsDownloaded = songsDownloaded.Any(x => new DirectoryInfo(x).Name.Split(" ")[0] == song.Key);

                    return beatSaverMaps;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<OnlineBeatmaps> GetOnlineBeatmaps(string query, int page = 0)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, "BeatSaverApi");
                    string json = await webClient.DownloadStringTaskAsync($"{beatSaverSearchApi}/{page}?q={query}");

                    OnlineBeatmaps beatSaverMaps = JsonConvert.DeserializeObject<OnlineBeatmaps>(json);
                    string[] songsDownloaded = Directory.GetDirectories(songsPath);

                    foreach (OnlineBeatmap song in beatSaverMaps.Maps)
                    {
                        foreach (string directory in Directory.GetDirectories(songsPath))
                            song.IsDownloaded = songsDownloaded.Any(x => new DirectoryInfo(x).Name == song.Hash);
                    }

                    return beatSaverMaps;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<LocalBeatmaps> GetLocalBeatmaps(string songsPath, LocalBeatmaps cachedLocalBeatmaps = null)
        {
            LocalBeatmaps localBeatmaps = cachedLocalBeatmaps is null ? new LocalBeatmaps() : new LocalBeatmaps(cachedLocalBeatmaps);
            List<string> songs = Directory.GetDirectories(songsPath).ToList();
            foreach (LocalBeatmap beatmap in localBeatmaps.Maps.ToList())
            {
                string song = songs.FirstOrDefault(x => new DirectoryInfo(x).Name.Split(" ")[0] == beatmap.Key);
                if (song != null)
                    songs.Remove(song);
            }

            for (int i = 0; i < songs.Count; i++)
            {
                if (i > 0 && i % 10 == 0)
                    localBeatmaps.LastPage++;
            }

            foreach (string songFolder in songs)
            {
                string infoFile = $@"{songFolder}\info.dat";
                string key = new DirectoryInfo(songFolder).Name.Split(" ")[0];

                if (!File.Exists(infoFile))
                    continue;

                string json = await File.ReadAllTextAsync(infoFile);
                LocalBeatmap beatMap = JsonConvert.DeserializeObject<LocalBeatmap>(json);

                beatMap.CoverImagePath = $@"{songFolder}\{beatMap.CoverImageFilename}";
                beatMap.Key = key;

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

                OnlineBeatmap songDetails = await GetBeatmap(key);
                if (songDetails != null)
                    beatMap.Downloads = songDetails.Stats.Downloads;

                localBeatmaps.Maps.Add(beatMap);
            }

            return RefreshLocalPages(localBeatmaps);
        }

        public LocalBeatmaps ChangeLocalPage(LocalBeatmaps localBeatmaps, int page)
        {
            LocalBeatmaps newLocalBeatmaps = new LocalBeatmaps(localBeatmaps);

            if (page >= 0 && page <= newLocalBeatmaps.LastPage)
            {
                if (page == 0)
                    newLocalBeatmaps.PrevPage = null;
                else
                    newLocalBeatmaps.PrevPage = page - 1;

                if (page == newLocalBeatmaps.LastPage)
                    newLocalBeatmaps.NextPage = null;
                else
                    newLocalBeatmaps.NextPage = page + 1;
            }
            else if (page <= 0)
            {
                page = 0;
                newLocalBeatmaps.PrevPage = null;
                if (page + 1 <= newLocalBeatmaps.LastPage)
                    newLocalBeatmaps.NextPage = page + 1;
                else
                    newLocalBeatmaps.NextPage = null;
            }
            else if (page >= newLocalBeatmaps.LastPage)
            {
                page = newLocalBeatmaps.LastPage;
                newLocalBeatmaps.NextPage = null;
                if (page - 1 >= 0)
                    newLocalBeatmaps.PrevPage = page - 1;
                else
                    newLocalBeatmaps.PrevPage = null;
            }

            return newLocalBeatmaps;
        }

        public LocalBeatmaps RefreshLocalPages(LocalBeatmaps localBeatmaps)
        {
            LocalBeatmaps newLocalBeatmaps = new LocalBeatmaps(localBeatmaps);
            int currentPage = 0;

            foreach (LocalBeatmap localBeatmap in newLocalBeatmaps.Maps)
            {
                int index = newLocalBeatmaps.Maps.IndexOf(localBeatmap);
                if (index > 0 && index % 10 == 0)
                    currentPage++;

                localBeatmap.Page = currentPage;
            }

            newLocalBeatmaps.LastPage = currentPage;
            if (currentPage == 0)
            {
                newLocalBeatmaps.NextPage = null;
                newLocalBeatmaps.PrevPage = null;
            }
            else
            {
                if (newLocalBeatmaps.NextPage is null && newLocalBeatmaps.PrevPage is null)
                {
                    if (currentPage >= 1)
                        newLocalBeatmaps.NextPage = 1;
                }
                else
                {
                    if (newLocalBeatmaps.NextPage != null)
                    {
                        if (newLocalBeatmaps.NextPage > currentPage)
                            newLocalBeatmaps.NextPage = null;
                    }
                    else
                    {
                        if (currentPage - 1 >= 0)
                            newLocalBeatmaps.PrevPage = currentPage - 1;
                        else
                            newLocalBeatmaps.PrevPage = null;
                    }
                }
            }

            return newLocalBeatmaps;
        }

        public async Task DownloadSong(OnlineBeatmap song)
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
                DownloadStarted?.Invoke(this, new DownloadStartedEventArgs(song));
                await webClient.DownloadFileTaskAsync(new Uri(downloadString), downloadFilePath);
                ZipFile.ExtractToDirectory(downloadFilePath, extractPath);
                File.Delete(downloadFilePath);

                DownloadCompleted?.Invoke(this, new DownloadCompletedEventArgs(song));
            }
        }

        public void DeleteSong(OnlineBeatmap song)
        {
            if (song.IsDownloaded)
            {
                string directory = Directory.GetDirectories(songsPath).FirstOrDefault(x => new DirectoryInfo(x).Name.Split(" ")[0] == song.Key);

                if (!string.IsNullOrEmpty(directory))
                    Directory.Delete(directory, true);

                song.IsDownloaded = false;
            }
        }

        public void DeleteSong(LocalBeatmap song)
        {
            string directory = Directory.GetDirectories(songsPath).FirstOrDefault(x => new DirectoryInfo(x).Name.Split(" ")[0] == song.Key);

            if (!string.IsNullOrEmpty(directory))
                Directory.Delete(directory, true);
        }

        public void DeleteSongs(ICollection<LocalBeatmap> songs)
        {
            foreach (LocalBeatmap song in songs)
                DeleteSong(song);
        }

        public async Task<OnlineBeatmap> GetBeatmap(string key)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, "BeatSaverApi");
                    string json = await webClient.DownloadStringTaskAsync($"{beatSaverDetailsApi}/{key}");
                    return JsonConvert.DeserializeObject<OnlineBeatmap>(json);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
