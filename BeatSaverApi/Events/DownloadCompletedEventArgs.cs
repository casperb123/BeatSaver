using BeatSaverApi.Entities;
using System;

namespace BeatSaverApi.Events
{
    public class DownloadCompletedEventArgs : EventArgs
    {
        public OnlineBeatmap Song { get; set; }

        public DownloadCompletedEventArgs(OnlineBeatmap song)
        {
            Song = song;
        }
    }
}
