using BeatSaverApi.Entities;
using BeatSaverApi.Events;
using Newtonsoft.Json;
using NReco.VideoInfo;
using System;
using System.Collections.Generic;
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
        private readonly string beatSaverDetailsKeyApi;
        private readonly string beatSaverDetailsHashApi;
        private readonly string downloadPath;
        private readonly string[] excludedCharacters;
        private readonly FFProbe ffProbe;

        public string SongsPath;

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
            beatSaverDetailsKeyApi = $"{beatSaverApi}/maps/detail";
            beatSaverDetailsHashApi = $"{beatSaverApi}/maps/by-hash";
            SongsPath = songsPath;

            string runningPath = AppDomain.CurrentDomain.BaseDirectory;
#if DEBUG
            string ffmpegPath = $@"{Path.GetFullPath(Path.Combine(runningPath, @"..\..\..\..\..\"))}BeatSaverApi\BeatSaverApi\ffmpeg";
#else
            string ffmpegPath = $@"{runningPath}\ffmpeg";
#endif
            ffProbe = new FFProbe
            {
                ToolPath = ffmpegPath
            };

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            downloadPath = $@"{appData}\BeatSaverApi";

            if (!Directory.Exists(downloadPath))
                Directory.CreateDirectory(downloadPath);

            excludedCharacters = new string[]
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
                    string[] songsDownloaded = Directory.GetDirectories(SongsPath);

                    foreach (OnlineBeatmap song in beatSaverMaps.Maps)
                    {
                        song.Metadata.DurationTimeSpan = new TimeSpan(0, 0, song.Metadata.Duration);
                        string folder = songsDownloaded.FirstOrDefault(x => new DirectoryInfo(x).Name.Split(" ")[0] == song.Key || new DirectoryInfo(x).Name.Split(" ")[0] == song.Hash);

                        foreach (Characteristic characteristic in song.Metadata.Characteristics)
                        {
                            if (characteristic.Difficulties.Easy != null)
                                SetOnlineNoteInformation(song, characteristic.Difficulties.Easy);
                            if (characteristic.Difficulties.Normal != null)
                                SetOnlineNoteInformation(song, characteristic.Difficulties.Normal);
                            if (characteristic.Difficulties.Hard != null)
                                SetOnlineNoteInformation(song, characteristic.Difficulties.Hard);
                            if (characteristic.Difficulties.Expert != null)
                                SetOnlineNoteInformation(song, characteristic.Difficulties.Expert);
                            if (characteristic.Difficulties.ExpertPlus != null)
                                SetOnlineNoteInformation(song, characteristic.Difficulties.ExpertPlus);
                        }

                        if (!string.IsNullOrEmpty(folder))
                        {
                            song.IsDownloaded = true;
                            song.Metadata.FolderPath = folder;
                        }
                    }

                    return beatSaverMaps;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<OnlineBeatmaps> GetOnlineBeatmaps(string query, MapSort mapSort, int page = 0)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, "BeatSaverApi");
                    OnlineBeatmaps beatSaverMaps = null;

                    if (mapSort == MapSort.Search)
                    {
                        string json = await webClient.DownloadStringTaskAsync($"{beatSaverSearchApi}/{page}?q={query}");
                        beatSaverMaps = JsonConvert.DeserializeObject<OnlineBeatmaps>(json);
                    }
                    else if (mapSort == MapSort.SearchKey)
                    {
                        string json = await webClient.DownloadStringTaskAsync($"{beatSaverDetailsKeyApi}/{query}");
                        beatSaverMaps = new OnlineBeatmaps
                        {
                            Maps = new OnlineBeatmap[]
                            {
                                JsonConvert.DeserializeObject<OnlineBeatmap>(json)
                            }
                        };
                    }
                    else if (mapSort == MapSort.SearchHash)
                    {
                        string json = await webClient.DownloadStringTaskAsync($"{beatSaverDetailsHashApi}/{query}");
                        beatSaverMaps = new OnlineBeatmaps
                        {
                            Maps = new OnlineBeatmap[]
                            {
                                JsonConvert.DeserializeObject<OnlineBeatmap>(json)
                            }
                        };
                    }

                    if (beatSaverMaps != null)
                    {
                        string[] songsDownloaded = Directory.GetDirectories(SongsPath);

                        foreach (OnlineBeatmap song in beatSaverMaps.Maps)
                        {
                            song.Metadata.DurationTimeSpan = new TimeSpan(0, 0, song.Metadata.Duration);
                            string folder = songsDownloaded.FirstOrDefault(x => new DirectoryInfo(x).Name.Split(" ")[0] == song.Key || new DirectoryInfo(x).Name.Split(" ")[0] == song.Hash);

                            foreach (Characteristic characteristic in song.Metadata.Characteristics)
                            {
                                if (characteristic.Difficulties.Easy != null)
                                    SetOnlineNoteInformation(song, characteristic.Difficulties.Easy);
                                if (characteristic.Difficulties.Normal != null)
                                    SetOnlineNoteInformation(song, characteristic.Difficulties.Normal);
                                if (characteristic.Difficulties.Hard != null)
                                    SetOnlineNoteInformation(song, characteristic.Difficulties.Hard);
                                if (characteristic.Difficulties.Expert != null)
                                    SetOnlineNoteInformation(song, characteristic.Difficulties.Expert);
                                if (characteristic.Difficulties.ExpertPlus != null)
                                    SetOnlineNoteInformation(song, characteristic.Difficulties.ExpertPlus);
                            }

                            if (!string.IsNullOrEmpty(folder))
                            {
                                song.IsDownloaded = true;
                                song.Metadata.FolderPath = folder;
                            }
                        }
                    }

                    return beatSaverMaps;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void SetOnlineNoteInformation(OnlineBeatmap onlineBeatmap, IDifficulty difficulty)
        {
            float secondEquivalentOfBeat = 60 / onlineBeatmap.Metadata.BeatsPerMinute;
            float minHalfJumpDuration = 1f;
            float jumpSpeedThreshold = 18f;
            float halfJumpDuration = 4f;

            while (difficulty.NoteJumpMovementSpeed * secondEquivalentOfBeat * halfJumpDuration > jumpSpeedThreshold)
                halfJumpDuration /= 2f;

            halfJumpDuration += difficulty.NoteJumpStartBeatOffset;

            if (halfJumpDuration < minHalfJumpDuration)
                halfJumpDuration = minHalfJumpDuration;

            difficulty.HalfJumpDuration = halfJumpDuration;
            difficulty.JumpDistance = difficulty.NoteJumpMovementSpeed * (secondEquivalentOfBeat * (halfJumpDuration * 2));
        }

        public async Task<LocalBeatmaps> GetLocalBeatmaps(LocalBeatmaps cachedLocalBeatmaps = null)
        {
            LocalBeatmaps localBeatmaps = cachedLocalBeatmaps is null ? new LocalBeatmaps() : new LocalBeatmaps(cachedLocalBeatmaps);
            List<string> songs = Directory.GetDirectories(SongsPath).ToList();

            foreach (LocalBeatmap beatmap in localBeatmaps.Maps.ToList())
            {
                string song = songs.FirstOrDefault(x => new DirectoryInfo(x).Name.Split(" ")[0] == beatmap.Identifier.Value);

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
                string[] folderName = new DirectoryInfo(songFolder).Name.Split(" ");
                LocalIdentifier identifier = folderName.Length == 1 ? new LocalIdentifier(false, folderName[0]) : new LocalIdentifier(true, folderName[0]);

                if (!File.Exists(infoFile))
                    continue;

                string json = await File.ReadAllTextAsync(infoFile);
                LocalBeatmap beatmap = JsonConvert.DeserializeObject<LocalBeatmap>(json);
                if (File.Exists($@"{songFolder}\{beatmap.CoverImageFilename}"))
                    beatmap.CoverImagePath = $@"{songFolder}\{beatmap.CoverImageFilename}";

                beatmap.Identifier = identifier;
                beatmap.FolderPath = songFolder;

                if (File.Exists($@"{songFolder}\{beatmap.SongFilename}"))
                {
                    MediaInfo mediaInfo = ffProbe.GetMediaInfo($@"{beatmap.FolderPath}\{beatmap.SongFilename}");
                    beatmap.Duration = mediaInfo.Duration;
                }

                foreach (DifficultyBeatmapSet difficultyBeatmapSet in beatmap.DifficultyBeatmapSets)
                {
                    if (!beatmap.Easy && difficultyBeatmapSet.DifficultyBeatmaps.Any(x => x.Difficulty == "Easy"))
                        beatmap.Easy = true;
                    if (!beatmap.Normal && difficultyBeatmapSet.DifficultyBeatmaps.Any(x => x.Difficulty == "Normal"))
                        beatmap.Normal = true;
                    if (!beatmap.Hard && difficultyBeatmapSet.DifficultyBeatmaps.Any(x => x.Difficulty == "Hard"))
                        beatmap.Hard = true;
                    if (!beatmap.Expert && difficultyBeatmapSet.DifficultyBeatmaps.Any(x => x.Difficulty == "Expert"))
                        beatmap.Expert = true;
                    if (!beatmap.ExpertPlus && difficultyBeatmapSet.DifficultyBeatmaps.Any(x => x.Difficulty == "ExpertPlus"))
                        beatmap.ExpertPlus = true;
                }

                _ = Task.Run(async () => beatmap.OnlineBeatmap = await GetBeatmap(identifier));

                _ = Task.Run(async () =>
                {
                    List<LocalBeatmapDetails> localBeatmapDetails = await GetLocalBeatmapDetails(beatmap, beatmap.DifficultyBeatmapSets);
                    beatmap.Details = localBeatmapDetails;
                });

                localBeatmaps.Maps.Add(beatmap);
            }

            return RefreshLocalPages(localBeatmaps);
        }

        private async Task<List<LocalBeatmapDetails>> GetLocalBeatmapDetails(LocalBeatmap localBeatmap, DifficultyBeatmapSet[] beatmapSets)
        {
            List<LocalBeatmapDetails> localBeatmapDetails = new List<LocalBeatmapDetails>();

            foreach (DifficultyBeatmapSet difficultyBeatmapSet in beatmapSets)
            {
                LocalBeatmapDetails beatmapDetails = new LocalBeatmapDetails(difficultyBeatmapSet.BeatmapCharacteristicName);

                foreach (DifficultyBeatmap difficultyBeatmap in difficultyBeatmapSet.DifficultyBeatmaps)
                {
                    float secondEquivalentOfBeat = 60 / localBeatmap.BeatsPerMinute;
                    float minHalfJumpDuration = 1f;
                    float jumpSpeedThreshold = 18f;
                    float halfJumpDuration = 4f;

                    while (difficultyBeatmap.NoteJumpMovementSpeed * secondEquivalentOfBeat * halfJumpDuration > jumpSpeedThreshold)
                        halfJumpDuration /= 2f;

                    halfJumpDuration += difficultyBeatmap.NoteJumpStartBeatOffset;

                    if (halfJumpDuration < minHalfJumpDuration)
                        halfJumpDuration = minHalfJumpDuration;

                    string filePath = $@"{localBeatmap.FolderPath}\{difficultyBeatmap.BeatmapFilename}";
                    string json = await File.ReadAllTextAsync(filePath);
                    LocalBeatmapDetail beatmapDetail = JsonConvert.DeserializeObject<LocalBeatmapDetail>(json);
                    beatmapDetail.HalfJumpDuration = halfJumpDuration;
                    beatmapDetail.JumpDistance = difficultyBeatmap.NoteJumpMovementSpeed * (secondEquivalentOfBeat * (halfJumpDuration * 2));
                    beatmapDetail.DifficultyBeatmap = difficultyBeatmap;
                    beatmapDetails.BeatmapDetails.Add(beatmapDetail);
                }

                localBeatmapDetails.Add(beatmapDetails);
            }

            return localBeatmapDetails;
        }

        public void ChangeLocalPage(LocalBeatmaps localBeatmaps, int page)
        {
            if (page >= 0 && page <= localBeatmaps.LastPage)
            {
                if (page == 0)
                    localBeatmaps.PrevPage = null;
                else
                    localBeatmaps.PrevPage = page - 1;

                if (page == localBeatmaps.LastPage)
                    localBeatmaps.NextPage = null;
                else
                    localBeatmaps.NextPage = page + 1;
            }
            else if (page <= 0)
            {
                page = 0;
                localBeatmaps.PrevPage = null;
                if (page + 1 <= localBeatmaps.LastPage)
                    localBeatmaps.NextPage = page + 1;
                else
                    localBeatmaps.NextPage = null;
            }
            else if (page >= localBeatmaps.LastPage)
            {
                page = localBeatmaps.LastPage;
                localBeatmaps.NextPage = null;
                if (page - 1 >= 0)
                    localBeatmaps.PrevPage = page - 1;
                else
                    localBeatmaps.PrevPage = null;
            }
        }

        public LocalBeatmaps RefreshLocalPages(LocalBeatmaps localBeatmaps)
        {
            LocalBeatmaps newLocalBeatmaps = new LocalBeatmaps(localBeatmaps);
            int lastPage = 0;

            foreach (LocalBeatmap localBeatmap in newLocalBeatmaps.Maps)
            {
                int index = newLocalBeatmaps.Maps.IndexOf(localBeatmap);
                if (index > 0 && index % 10 == 0)
                    lastPage++;

                localBeatmap.Page = lastPage;
            }

            newLocalBeatmaps.LastPage = lastPage;
            if (lastPage == 0)
            {
                newLocalBeatmaps.NextPage = null;
                newLocalBeatmaps.PrevPage = null;
            }
            else
            {
                if (newLocalBeatmaps.NextPage is null && newLocalBeatmaps.PrevPage is null)
                {
                    if (lastPage >= 1)
                        newLocalBeatmaps.NextPage = 1;
                }
                else
                {
                    if (newLocalBeatmaps.NextPage is null)
                    {
                        if (newLocalBeatmaps.PrevPage < lastPage)
                        {
                            if (newLocalBeatmaps.PrevPage + 2 <= lastPage)
                                newLocalBeatmaps.NextPage = newLocalBeatmaps.PrevPage + 2;
                            else
                                newLocalBeatmaps.PrevPage = lastPage - 1;
                        }
                        else
                            newLocalBeatmaps.PrevPage = lastPage - 1;
                    }
                    else
                    {
                        if (newLocalBeatmaps.NextPage > lastPage)
                        {
                            newLocalBeatmaps.NextPage = null;
                            if (lastPage - 1 >= 0)
                                newLocalBeatmaps.PrevPage = lastPage - 1;
                        }
                    }
                }
            }

            return newLocalBeatmaps;
        }

        public async Task DownloadSong(OnlineBeatmap song)
        {
            try
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
                string extractPath = $@"{downloadPath}\{song.Key}";
                string songFolderPath = $@"{SongsPath}\{song.Key} ({songName} - {levelAuthorName})";

                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, "BeatSaverApi");
                    song.IsDownloading = true;
                    DownloadStarted?.Invoke(this, new DownloadStartedEventArgs(song));
                    await webClient.DownloadFileTaskAsync(new Uri(downloadString), downloadFilePath);
                    if (!Directory.Exists(extractPath))
                        Directory.CreateDirectory(extractPath);

                    ZipFile.ExtractToDirectory(downloadFilePath, extractPath);
                    File.Delete(downloadFilePath);
                    Directory.Move(extractPath, songFolderPath);

                    song.Metadata.FolderPath = songFolderPath;
                    song.IsDownloading = false;
                    song.IsDownloaded = true;
                    DownloadCompleted?.Invoke(this, new DownloadCompletedEventArgs(song));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteSong(OnlineBeatmap song)
        {
            if (song.IsDownloaded)
            {
                string directory;
                if (Directory.Exists($@"{SongsPath}\{song.Hash}"))
                    directory = Directory.GetDirectories(SongsPath).FirstOrDefault(x => new DirectoryInfo(x).Name.Split(" ")[0] == song.Hash);
                else
                    directory = Directory.GetDirectories(SongsPath).FirstOrDefault(x => new DirectoryInfo(x).Name.Split(" ")[0] == song.Key);

                if (!string.IsNullOrEmpty(directory))
                    Directory.Delete(directory, true);

                song.IsDownloaded = false;
            }
        }

        public void DeleteSong(LocalBeatmap song)
        {
            string directory = Directory.GetDirectories(SongsPath).FirstOrDefault(x => new DirectoryInfo(x).Name.Split(" ")[0] == song.Identifier.Value);

            if (!string.IsNullOrEmpty(directory))
                Directory.Delete(directory, true);
        }

        public void DeleteSongs(ICollection<LocalBeatmap> songs)
        {
            foreach (LocalBeatmap song in songs)
                DeleteSong(song);
        }

        public async Task<OnlineBeatmap> GetBeatmap(LocalIdentifier identifier)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, "BeatSaverApi");
                    string api = identifier.IsKey ? $"{beatSaverDetailsKeyApi}/{identifier.Value}" : $"{beatSaverDetailsHashApi}/{identifier.Value}";
                    string json = await webClient.DownloadStringTaskAsync(api);
                    return JsonConvert.DeserializeObject<OnlineBeatmap>(json);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<OnlineBeatmap> GetBeatmap(string key)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, "BeatSaverApi");
                    string api = $"{beatSaverDetailsKeyApi}/{key}";
                    string json = await webClient.DownloadStringTaskAsync(api);
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
