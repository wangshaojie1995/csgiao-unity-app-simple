using System;

public class ConstStr
{
    public const string RedBase = "redBase";
    public const string BlueBase = "blueBase";
    
    public const string PubgMap = "PubgMap";
    public const string PubgPoison = "PubgPoison";

    public const string SimpleMode = "SimpleMode";
    public const string FlagMode = "FlagMode";
    public const string PointMode = "PointMode";
    public const string PubgMode = "PUBGMode";
}

public enum Mode
{
    SimpleMode = 0,
    FlagMode = 1,
    PointMode = 2,
    PubgMode = 3
}

public enum Camp
{
    NotSet = 0,
    Blue = 1,
    Red = 2,
}

public enum Result
{
    Success = 0,
    Fail = 1,
}