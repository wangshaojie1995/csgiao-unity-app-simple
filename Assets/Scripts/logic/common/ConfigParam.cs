

public class ConfigParam
{
    ////////////////////////////////////////////////////////////////////
    // 服务器
    ////////////////////////////////////////////////////////////////////
    
    //部署相关
    public static int MysqlPort { get; set; } = 3306; // 3306
    public static string MysqlIp { get; set; } = "127.0.0.1"; // "127.0.0.1"
    
    //框架相关
    public static int MaxHp { get; set; } = 100;// 100
    public static int RoomMaxPlayers { get; set; } = 6; // 6
    
    //时间相关
    public static long PointUpdateCd { get; set; } = 1000;// 1000
    public static long VictoryJudgeCd { get; set; } = 500;// 500
    public static long FlagUpdateCd { get; set; } = 100;// 100 ms
    public static long PingCd { get; set; } = 30 * 1000;// 30 * 1000 ms
    public static long PubgUpdateCd { get; set; } = 500;// 500 ms
    public static long PubgNarrowCd { get; set; } = 60 * 1000;// 60 * 1000 ms   //吃鸡模式缩圈的时间
    public static long PubgPoisonCd { get; set; } = 1000;// 1000 ms   //吃鸡模式缩圈的时间

    //模式相关
    public static float FlagRadius { get; set; } = 20; // 20
    public static int FlagVictoryCount { get; set; } = 3; // 3

    public static int PointTotalCount { get; set; } = 100; // 100
    public static int PubgNarrowRadiusRatio { get; set; } = 2; // 100
    public static int PubgPoisonDamage { get; set; } = 2; // 2


    ////////////////////////////////////////////////////////////////////
    //客户端
    ////////////////////////////////////////////////////////////////////
    
    public static long SyncDdl { get; set; } = 20 * 1000; // 20 * 1000 ms  // 别人同步位置的DDL
    public static long SyncAreaCd { get; set; } = 200; //ms  //同步area的Cd时间
    public static long SyncLocationCd { get; set; } = 50; //ms  //同步位置帧率
    public static long LocationCd { get; set; } = 1000; //ms
    public static long SensorCd { get; set; } = 100; // ms
    public static long UsbSerialCd { get; set; } = 30; //ms
    public static long DebugCd { get; set; } = 1000;// 1000 ms

    //基础信息
    public static int DefaultBulletNum { get; set; } = 30;
    public static int DefaultMagNum { get; set; } = 120;
    public static int DefaultBulletMaxNum { get; set; } = 30;
    public static int DefaultDefense { get; set; } = 0;
    
    //CtrlTank
    public static float KeyBoardMoveSpeed { get; set; } = 0.00005f;
    
    //InputManager
    public static float CameraZoomSpeed { get; set; } = 0.1f;
    public static float CameraZoomLowerBound { get; set; } = 0.3f;
    public static float CameraZoomUpperBound { get; set; } = 2.5f;
    public static float FlagDisplayScale { get; set; } = 0.1f;
    //播放动画的Panel
    public static float AnimationSpeed { get; set; } = 2.0f;
    public static float AnimationHoldTime { get; set; } = 2000; // ms
    public static long KillPanelDuring { get; set; } = 100;// 100 ms  

    



}