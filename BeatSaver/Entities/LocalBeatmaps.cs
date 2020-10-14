using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BeatSaver.Entities
{
    public class LocalBeatmaps : INotifyPropertyChanged
    {
        private int? prevPage;
        private int? nextPage;

        public List<LocalBeatmap> Maps { get; set; }
        public int LastPage { get; set; }
        public int? PrevPage
        {
            get { return prevPage; }
            set
            {
                prevPage = value;
                OnPropertyChanged(nameof(CurrentPageReal));
            }
        }
        public int? NextPage
        {
            get { return nextPage; }
            set
            {
                nextPage = value;
                OnPropertyChanged(nameof(CurrentPageReal));
            }
        }
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
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
