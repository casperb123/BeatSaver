using BeatSaver.Entities;
using System;

namespace BeatSaver.Events
{
    public class OnlineBeatmapDeletedEventArgs : EventArgs
    {
        public OnlineBeatmap Beatmap { get; private set; }

        public OnlineBeatmapDeletedEventArgs(OnlineBeatmap beatmap)
        {
            Beatmap = beatmap;
        }
    }
}
