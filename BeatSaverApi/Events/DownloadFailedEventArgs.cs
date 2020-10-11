using BeatSaverApi.Entities;
using System;

namespace BeatSaverApi.Events
{
    public class DownloadFailedEventArgs : EventArgs
    {
        public OnlineBeatmap Beatmap { get; private set; }
        public Exception Exception { get; set; }

        public DownloadFailedEventArgs(OnlineBeatmap beatmap, Exception exception)
        {
            Beatmap = beatmap;
            Exception = exception;
        }
    }
}
