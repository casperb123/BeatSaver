using Newtonsoft.Json;
using System;
using System.Linq;

namespace BeatSaverApi.Entities
{
    public class LocalBeatmapDetail
    {
        public DifficultyBeatmap DifficultyBeatmap { get; set; }

        [JsonProperty("_version")]
        public string Version { get; set; }
        [JsonProperty("_customData")]
        public Customdata CustomData { get; set; }
        [JsonProperty("_events")]
        public Events[] Events { get; set; }
        [JsonProperty("_notes")]
        public Notes[] Notes { get; set; }
        [JsonProperty("_obstacles")]
        public Obstacles[] Obstacles { get; set; }

        public int NotesCount
        {
            get { return Notes.Where(x => x.Type != 3).Count(); }
        }
        public int BombsCount
        {
            get { return Notes.Where(x => x.Type == 3).Count(); }
        }
        public int ObstaclesCount
        {
            get { return Obstacles.Length; }
        }
        public int EventsCount
        {
            get { return Events.Length; }
        }
        public double HalfJumpDuration { get; set; }
        public float JumpDistance { get; set; }
    }

    public class Customdata
    {
        [JsonProperty("_time")]
        public float Time { get; set; }
        [JsonProperty("_BPMChanges")]
        public object[] BPMChanges { get; set; }
        [JsonProperty("_bookmarks")]
        public object[] Bookmarks { get; set; }
    }

    public class Events
    {
        [JsonProperty("_time")]
        public float Time { get; set; }
        [JsonProperty("_type")]
        public int Type { get; set; }
        [JsonProperty("_value")]
        public int Value { get; set; }
    }

    public class Notes
    {
        [JsonProperty("_time")]
        public float Time { get; set; }
        [JsonProperty("_lineIndex")]
        public int LineIndex { get; set; }
        [JsonProperty("_version")]
        public int _lineLayer { get; set; }
        [JsonProperty("_type")]
        public int Type { get; set; }
        [JsonProperty("_cutDirection")]
        public int CutDirection { get; set; }
    }

    public class Obstacles
    {
        [JsonProperty("_time")]
        public float Time { get; set; }
        [JsonProperty("_lineIndex")]
        public int LineIndex { get; set; }
        [JsonProperty("_type")]
        public int Type { get; set; }
        [JsonProperty("_duration")]
        public float Duration { get; set; }
        [JsonProperty("_width")]
        public int Width { get; set; }
    }
}
