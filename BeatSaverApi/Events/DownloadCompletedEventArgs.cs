using System;
using System.Collections.Generic;
using System.Text;

namespace BeatSaverApi.Events
{
    public class DownloadCompletedEventArgs : EventArgs
    {
        public Doc Song { get; set; }

        public DownloadCompletedEventArgs(Doc song)
        {
            Song = song;
        }
    }
}
