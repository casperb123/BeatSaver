using BeatSaver.Entities;
using System;

namespace BeatSaver.Events
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
