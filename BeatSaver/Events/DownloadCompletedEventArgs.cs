using BeatSaver.Entities;
using System;

namespace BeatSaver.Events
{
    public class DownloadCompletedEventArgs : EventArgs
    {
        public OnlineBeatmap Beatmap { get; private set; }

        public DownloadCompletedEventArgs(OnlineBeatmap beatmap)
        {
            Beatmap = beatmap;
        }
    }
}
