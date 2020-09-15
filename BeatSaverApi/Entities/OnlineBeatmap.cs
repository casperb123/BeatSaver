using System;
using System.ComponentModel;

namespace BeatSaverApi.Entities
{
    public class OnlineBeatmap : INotifyPropertyChanged
    {
        private bool isDownloaded;
        private bool isDownloading;

        public Metadata Metadata { get; set; }
        public Stats Stats { get; set; }
        public string Description { get; set; }
        public object DeletedAt { get; set; }
        public string Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public Uploader Uploader { get; set; }
        public string Hash { get; set; }
        public DateTime Uploaded { get; set; }
        public string DirectDownload { get; set; }
        public string DownloadURL { get; set; }
        public string CoverURL { get; set; }
        public string RealCoverURL
        {
            get { return $"https://beatsaver.com{CoverURL}"; }
        }
        public bool IsDownloaded
        {
            get { return isDownloaded; }
            set
            {
                isDownloaded = value;
                OnPropertyChanged(nameof(IsDownloaded));
            }
        }
        public bool IsDownloading
        {
            get { return isDownloading; }
            set
            {
                isDownloading = value;
                OnPropertyChanged(nameof(IsDownloading));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class Metadata
    {
        public Difficulties Difficulties { get; set; }
        public int Duration { get; set; }
        public object Automapper { get; set; }
        public Characteristic[] Characteristics { get; set; }
        public string LevelAuthorName { get; set; }
        public string SongAuthorName { get; set; }
        public string SongName { get; set; }
        public string SongSubName { get; set; }
        public float Bpm { get; set; }
    }

    public class Difficulties
    {
        public bool Easy { get; set; }
        public bool Expert { get; set; }
        public bool ExpertPlus { get; set; }
        public bool Hard { get; set; }
        public bool Normal { get; set; }
    }

    public class Characteristic
    {
        public Difficulties1 Difficulties { get; set; }
        public string Name { get; set; }
    }

    public class Difficulties1
    {
        public Easy Easy { get; set; }
        public Expert Expert { get; set; }
        public Expertplus ExpertPlus { get; set; }
        public Hard Hard { get; set; }
        public Normal Normal { get; set; }
    }

    public class Easy
    {
        public float Duration { get; set; }
        public int Length { get; set; }
        public float Njs { get; set; }
        public float NjsOffset { get; set; }
        public int Bombs { get; set; }
        public int Notes { get; set; }
        public int Obstacles { get; set; }
    }

    public class Expert
    {
        public float Duration { get; set; }
        public int Length { get; set; }
        public float Njs { get; set; }
        public float NjsOffset { get; set; }
        public int Bombs { get; set; }
        public int Notes { get; set; }
        public int Obstacles { get; set; }
    }

    public class Expertplus
    {
        public float Duration { get; set; }
        public int Length { get; set; }
        public float Njs { get; set; }
        public float NjsOffset { get; set; }
        public int Bombs { get; set; }
        public int Notes { get; set; }
        public int Obstacles { get; set; }
    }

    public class Hard
    {
        public float Duration { get; set; }
        public int Length { get; set; }
        public float Njs { get; set; }
        public float NjsOffset { get; set; }
        public int Bombs { get; set; }
        public int Notes { get; set; }
        public int Obstacles { get; set; }
    }

    public class Normal
    {
        public float Duration { get; set; }
        public int Length { get; set; }
        public float Njs { get; set; }
        public float NjsOffset { get; set; }
        public int Bombs { get; set; }
        public int Notes { get; set; }
        public int Obstacles { get; set; }
    }

    public class Stats
    {
        public int Downloads { get; set; }
        public int Plays { get; set; }
        public int DownVotes { get; set; }
        public int UpVotes { get; set; }
        public float Heat { get; set; }
        public float Rating { get; set; }
    }

    public class Uploader
    {
        public string Id { get; set; }
        public string Username { get; set; }
    }
}
