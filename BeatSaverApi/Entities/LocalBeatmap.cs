﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace BeatSaverApi.Entities
{
    public class LocalBeatmap : INotifyPropertyChanged
    {
        private string songAuthorName;
        private string songSubName;
        private List<LocalBeatmapDetails> details;
        private OnlineBeatmap onlineBeatmap;

        [JsonProperty("_version")]
        public string Version { get; set; }
        [JsonProperty("_songName")]
        public string SongName { get; set; }
        [JsonProperty("_songSubName")]
        public string SongSubName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(songSubName))
                    return null;
                else
                    return songSubName;
            }
            set { songSubName = value; }
        }
        [JsonProperty("_songAuthorName")]
        public string SongAuthorName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(songAuthorName))
                    return null;
                else
                    return songAuthorName;
            }
            set { songAuthorName = value; }
        }
        [JsonProperty("_levelAuthorName")]
        public string LevelAuthorName { get; set; }
        [JsonProperty("_beatsPerMinute")]
        public float BeatsPerMinute { get; set; }
        [JsonProperty("_shuffle")]
        public float Shuffle { get; set; }
        [JsonProperty("_shufflePeriod")]
        public double ShufflePeriod { get; set; }
        [JsonProperty("_previewStartTime")]
        public double PreviewStartTime { get; set; }
        [JsonProperty("_previewDuration")]
        public float PreviewDuration { get; set; }
        [JsonProperty("_songFilename")]
        public string SongFilename { get; set; }
        [JsonProperty("_coverImageFilename")]
        public string CoverImageFilename { get; set; }
        [JsonProperty("_environmentName")]
        public string EnvironmentName { get; set; }
        [JsonProperty("_songTimeOffset")]
        public float SongTimeOffset { get; set; }
        [JsonProperty("_customData")]
        public InfoCustomData CustomData { get; set; }
        [JsonProperty("_difficultyBeatmapSets")]
        public DifficultyBeatmapSet[] DifficultyBeatmapSets { get; set; }

        public bool Easy { get; set; }
        public bool Normal { get; set; }
        public bool Hard { get; set; }
        public bool Expert { get; set; }
        public bool ExpertPlus { get; set; }
        public List<LocalBeatmapDetails> Details
        {
            get { return details; }
            set
            {
                details = value;
                OnPropertyChanged(nameof(Details));
            }
        }

        public string Key { get; set; }
        public string CoverImagePath { get; set; }
        public int Page { get; set; }

        public OnlineBeatmap OnlineBeatmap
        {
            get { return onlineBeatmap; }
            set
            {
                onlineBeatmap = value;
                OnPropertyChanged(nameof(OnlineBeatmap));
            }
        }
        public int? Downloads { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
    public class InfoCustomData
    {
        [JsonProperty("_contributors")]
        public Contributor[] Contributors { get; set; }
    }
    public class Contributor
    {
        [JsonProperty("_role")]
        public string Role;
        [JsonProperty("_name")]
        public string Name;
        [JsonProperty("_iconPath")]
        public string IconPath;
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
        public float DifficultyRank { get; set; }
        [JsonProperty("_beatmapFilename")]
        public string BeatmapFilename { get; set; }
        [JsonProperty("_noteJumpMovementSpeed")]
        public float NoteJumpMovementSpeed { get; set; }
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
        public float EditorOffset { get; set; }
        [JsonProperty("_editorOldOffset")]
        public float EditorOldOffset { get; set; }
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