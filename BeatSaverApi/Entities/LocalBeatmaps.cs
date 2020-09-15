using System.Collections.Generic;
using System.Linq;

namespace BeatSaverApi.Entities
{
    public class LocalBeatmaps
    {
        public List<LocalBeatmap> Maps { get; set; }
        public int LastPage { get; set; }
        public int? PrevPage { get; set; }
        public int? NextPage { get; set; }
        public int CurrentPage
        {
            get
            {
                if (!PrevPage.HasValue && !NextPage.HasValue)
                    return 0;

                return NextPage.HasValue ? (NextPage.Value - 1) : (PrevPage.Value + 1);
            }
        }
        public int CurrentPageReal
        {
            get { return CurrentPage + 1; }
        }
        public int LastPageReal
        {
            get { return LastPage + 1; }
        }

        public LocalBeatmaps()
        {
            Maps = new List<LocalBeatmap>();
        }

        public LocalBeatmaps(LocalBeatmaps localBeatmaps)
        {
            Maps = localBeatmaps.Maps.ToList();
            LastPage = localBeatmaps.LastPage;
            PrevPage = localBeatmaps.PrevPage;
            NextPage = localBeatmaps.NextPage;
        }
    }
}
