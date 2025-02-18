﻿using System.Collections.Generic;

namespace BeatSaver.Entities
{
    public class LocalBeatmapDetails
    {
        public string CharacteristicName { get; set; }
        public List<LocalBeatmapDetail> BeatmapDetails { get; set; }

        public LocalBeatmapDetails(string characteristicName)
        {
            CharacteristicName = characteristicName;
            BeatmapDetails = new List<LocalBeatmapDetail>();
        }
    }
}
