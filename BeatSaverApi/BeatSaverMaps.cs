﻿using System;
using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;

public class BeatSaverMaps
{
    public Doc[] docs { get; set; }
    public int totalDocs { get; set; }
    public int lastPage { get; set; }
    public int? prevPage { get; set; }
    public int? nextPage { get; set; }
    public int currentPage
    {
        get
        {
            return nextPage.HasValue ? (nextPage.Value - 1) : (prevPage.Value + 1);
        }
    }
    public int currentPageReal
    {
        get { return currentPage + 1; }
    }
    public int lastPageReal
    {
        get { return lastPage + 1; }
    }
}

public class Doc : INotifyPropertyChanged
{
    private bool isDownloaded;
    private bool isDownloading;

    public Metadata metadata { get; set; }
    public Stats stats { get; set; }
    public string description { get; set; }
    public object deletedAt { get; set; }
    public string _id { get; set; }
    public string key { get; set; }
    public string name { get; set; }
    public Uploader uploader { get; set; }
    public string hash { get; set; }
    public DateTime uploaded { get; set; }
    public string directDownload { get; set; }
    public string downloadURL { get; set; }
    public string coverURL { get; set; }
    public string realCoverURL
    {
        get { return $"https://beatsaver.com{coverURL}"; }
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
    public Difficulties difficulties { get; set; }
    public int duration { get; set; }
    public object automapper { get; set; }
    public Characteristic[] characteristics { get; set; }
    public string levelAuthorName { get; set; }
    public string songAuthorName { get; set; }
    public string songName { get; set; }
    public string songSubName { get; set; }
    public float bpm { get; set; }
}

public class Difficulties
{
    public bool easy { get; set; }
    public bool expert { get; set; }
    public bool expertPlus { get; set; }
    public bool hard { get; set; }
    public bool normal { get; set; }
}

public class Characteristic
{
    public Difficulties1 difficulties { get; set; }
    public string name { get; set; }
}

public class Difficulties1
{
    public Easy easy { get; set; }
    public Expert expert { get; set; }
    public Expertplus expertPlus { get; set; }
    public Hard hard { get; set; }
    public Normal normal { get; set; }
}

public class Easy
{
    public float duration { get; set; }
    public int length { get; set; }
    public float njs { get; set; }
    public float njsOffset { get; set; }
    public int bombs { get; set; }
    public int notes { get; set; }
    public int obstacles { get; set; }
}

public class Expert
{
    public float duration { get; set; }
    public int length { get; set; }
    public float njs { get; set; }
    public float njsOffset { get; set; }
    public int bombs { get; set; }
    public int notes { get; set; }
    public int obstacles { get; set; }
}

public class Expertplus
{
    public float duration { get; set; }
    public int length { get; set; }
    public float njs { get; set; }
    public float njsOffset { get; set; }
    public int bombs { get; set; }
    public int notes { get; set; }
    public int obstacles { get; set; }
}

public class Hard
{
    public float duration { get; set; }
    public int length { get; set; }
    public float njs { get; set; }
    public float njsOffset { get; set; }
    public int bombs { get; set; }
    public int notes { get; set; }
    public int obstacles { get; set; }
}

public class Normal
{
    public float duration { get; set; }
    public int length { get; set; }
    public float njs { get; set; }
    public float njsOffset { get; set; }
    public int bombs { get; set; }
    public int notes { get; set; }
    public int obstacles { get; set; }
}

public class Stats
{
    public int downloads { get; set; }
    public int plays { get; set; }
    public int downVotes { get; set; }
    public int upVotes { get; set; }
    public float heat { get; set; }
    public float rating { get; set; }
}

public class Uploader
{
    public string _id { get; set; }
    public string username { get; set; }
}
