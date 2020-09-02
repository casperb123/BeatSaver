using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaverApi
{
    public class BeatSaver
    {
        private readonly string beatSaverApi;
        private readonly string beatSaverHotApi;
        private readonly string beatSaverRatingApi;
        private readonly string beatSaverLatestApi;
        private readonly string beatSaverDownloadsApi;
        private readonly string beatSaverPlaysApi;
        private readonly string beatSaverSearchApi;

        public BeatSaver()
        {
            beatSaverApi = "https://beatsaver.com/api";
            beatSaverHotApi = $"{beatSaverApi}/maps/hot";
            beatSaverRatingApi = $"{beatSaverApi}/maps/rating";
            beatSaverLatestApi = $"{beatSaverApi}/maps/latest";
            beatSaverDownloadsApi = $"{beatSaverApi}/maps/downloads";
            beatSaverPlaysApi = $"{beatSaverApi}/maps/plays";
            beatSaverSearchApi = $"{beatSaverApi}/search/text";
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

                return JsonConvert.DeserializeObject<BeatSaverMaps>(json);
            }
        }

        public async Task<BeatSaverMaps> GetBeatSaverMaps(string query, int page = 0)
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.Headers.Add(HttpRequestHeader.UserAgent, "BeatSaverApi");
                string json = await webClient.DownloadStringTaskAsync($"{beatSaverSearchApi}/{page}?q={query}");
                return JsonConvert.DeserializeObject<BeatSaverMaps>(json);
            }
        }
    }
}
