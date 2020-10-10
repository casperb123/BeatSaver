using BeatSaverApi.Entities;
using System;

namespace BeatSaverApi.Events
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
