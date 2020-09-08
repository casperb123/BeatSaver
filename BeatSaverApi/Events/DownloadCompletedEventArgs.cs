using System;
using System.Collections.Generic;
using System.Text;

namespace BeatSaverApi.Events
{
    public class DownloadCompletedEventArgs : EventArgs
    {
        public OnlineBeatMap Song { get; set; }

        public DownloadCompletedEventArgs(OnlineBeatMap song)
        {
            Song = song;
        }
    }
}
