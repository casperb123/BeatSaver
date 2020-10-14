using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BeatSaver.Entities
{
    public class LocalBeatmap : INotifyPropertyChanged
    {
        private string environtmentName;
        private string levelAuthorName;
        private string songAuthorName;
        private string songSubName;
        private string songName;
        private List<LocalBeatmapDetails> details;
        private OnlineBeatmap onlineBeatmap;

        [JsonProperty("_version")]
        public string Version { get; set; }
        [JsonProperty("_songName")]
        public string SongName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(songName))
                    return null;
                else
                    return songName;
            }
            set { songName = value; }
        }
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
        public string LevelAuthorName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(levelAuthorName))
                    return null;
                else
                    return levelAuthorName;
            }
            set { levelAuthorName = value; }
        }
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
        public string EnvironmentName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(environtmentName))
                    return null;
                else
                    return environtmentName;
            }
            set { environtmentName = value; }
        }
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

        public string FullSongName
        {
            get { return $"{SongName} {SongSubName}"; }
        }
        public LocalIdentifier Identifier { get; set; }
        public string FolderPath { get; set; }
        public string CoverImagePath { get; set; }
        public TimeSpan? Duration { get; set; }
        public int Page { get; set; }

        public List<string> Errors { get; set; }

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

    public class LocalIdentifier
    {
        public bool IsKey { get; set; }
        public string Value { get; set; }

        public LocalIdentifier(bool isKey, string value)
        {
            IsKey = isKey;
            Value = value;
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
        public float NoteJumpStartBeatOffset { get; set; }
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

        [JsonIgnore]
        public List<string> RequiredMods
        {
            get
            {
                List<string> requiredMods = new List<string>();

                if (Requirements != null)
                    foreach (string mod in Requirements)
                        requiredMods.Add(mod);
                if (Suggestions != null)
                    foreach (string mod in Suggestions)
                        requiredMods.Add(mod);

                return requiredMods;
            }
        }
    }
}