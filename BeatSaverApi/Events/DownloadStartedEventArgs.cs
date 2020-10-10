using BeatSaverApi.Entities;
using System;

namespace BeatSaverApi.Events
{
    public class DownloadStartedEventArgs : EventArgs
    {
        public OnlineBeatmap Beatmap { get; private set; }

        public DownloadStartedEventArgs(OnlineBeatmap beatmap)
        {
            Beatmap = beatmap;
        }
    }
}
