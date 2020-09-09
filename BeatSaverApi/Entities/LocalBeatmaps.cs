using BeatSaverApi.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

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
        public int Page { get; set; }

        public LocalBeatmaps()
        {
            Maps = new List<LocalBeatmap>();
        }
    }
}
