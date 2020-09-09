using BeatSaverApi.Entities;
using System;
using System.Collections.Generic;
using System.Text;

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
