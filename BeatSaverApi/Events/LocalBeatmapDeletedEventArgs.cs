using BeatSaverApi.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatSaverApi.Events
{
    public class LocalBeatmapDeletedEventArgs : EventArgs
    {
        public LocalBeatmap Beatmap { get; private set; }

        public LocalBeatmapDeletedEventArgs(LocalBeatmap beatmap)
        {
            Beatmap = beatmap;
        }
    }
}
