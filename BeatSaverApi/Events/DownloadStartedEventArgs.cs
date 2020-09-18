using BeatSaverApi.Entities;
using System;

namespace BeatSaverApi.Events
{
    public class DownloadStartedEventArgs : EventArgs
    {
        public OnlineBeatmap Song { get; set; }

        public DownloadStartedEventArgs(OnlineBeatmap song)
        {
            Song = song;
        }
    }
}
