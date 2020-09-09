﻿using Newtonsoft.Json;

namespace BeatSaverApi.Entities
{
    public class LocalBeatmap
    {
        [JsonProperty("_version")]
        public string Version { get; set; }
        [JsonProperty("_songName")]
        public string SongName { get; set; }
        [JsonProperty("_songSubName")]
        public string SongSubName { get; set; }
        [JsonProperty("_songAuthorName")]
        public string SongAuthorName { get; set; }
        [JsonProperty("_levelAuthorName")]
        public string LevelAuthorName { get; set; }
        [JsonProperty("_beatsPerMinute")]
        public long BeatsPerMinute { get; set; }
        [JsonProperty("_shuffle")]
        public long Shuffle { get; set; }
        [JsonProperty("_shufflePeriod")]
        public double ShufflePeriod { get; set; }
        [JsonProperty("_previewStartTime")]
        public double PreviewStartTime { get; set; }
        [JsonProperty("_previewDuration")]
        public long PreviewDuration { get; set; }
        [JsonProperty("_songFilename")]
        public string SongFilename { get; set; }
        [JsonProperty("_coverImageFilename")]
        public string CoverImageFilename { get; set; }
        [JsonProperty("_environmentName")]
        public string EnvironmentName { get; set; }
        [JsonProperty("_songTimeOffset")]
        public long SongTimeOffset { get; set; }
        [JsonProperty("_customData")]
        public InfoCustomData CustomData { get; set; }
        [JsonProperty("_difficultyBeatmapSets")]
        public DifficultyBeatmapSet[] DifficultyBeatmapSets { get; set; }
        public bool Easy { get; set; }
        public bool Normal { get; set; }
        public bool Hard { get; set; }
        public bool Expert { get; set; }
        public bool ExpertPlus { get; set; }

        public string Key { get; set; }
        public string CoverImagePath { get; set; }
        public int Page { get; set; }
        public int? Downloads { get; set; }
    }
    public class InfoCustomData
    {
        [JsonProperty("_contributors")]
        public object[] Contributors { get; set; }
    }
    public class DifficultyBeatmapSet
    {
        [JsonProperty("_beatmapCharacteristicName")]
        public string BeatmapCharacteristicName { get; set; }
        [JsonProperty("_difficultyBeatmaps")]
        public DifficultyBeatmap[] DifficultyBeatmaps { get; set; }
    }
    public class DifficultyBeatmap
    {
        [JsonProperty("_difficulty")]
        public string Difficulty { get; set; }
        [JsonProperty("_difficultyRank")]
        public long DifficultyRank { get; set; }
        [JsonProperty("_beatmapFilename")]
        public string BeatmapFilename { get; set; }
        [JsonProperty("_noteJumpMovementSpeed")]
        public long NoteJumpMovementSpeed { get; set; }
        [JsonProperty("_noteJumpStartBeatOffset")]
        public double NoteJumpStartBeatOffset { get; set; }
        [JsonProperty("_customData")]
        public DifficultyBeatmapCustomData CustomData { get; set; }
    }
    public class DifficultyBeatmapCustomData
    {
        [JsonProperty("_difficultyLabel")]
        public string DifficultyLabel { get; set; }
        [JsonProperty("_editorOffset")]
        public long EditorOffset { get; set; }
        [JsonProperty("_editorOldOffset")]
        public long EditorOldOffset { get; set; }
        [JsonProperty("_warnings")]
        public string[] Warnings { get; set; }
        [JsonProperty("_information")]
        public string[] Information { get; set; }
        [JsonProperty("_suggestions")]
        public string[] Suggestions { get; set; }
        [JsonProperty("_requirements")]
        public string[] Requirements { get; set; }
    }
}