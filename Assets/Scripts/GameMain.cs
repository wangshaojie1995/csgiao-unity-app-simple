using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using Assist;
using UnityEngine;
using UnityEngine.Android;
using utils;

public class GameMain : MonoBehaviour {
	public static string id = "cat";
	public static int camp = (int)Camp.NotSet;
	
	// 114.059318, 22.548414  深圳福田区莲花山公园
	// 126.634944, 45.7434444  哈尔滨哈工大三公寓
	
	//TODO: c#  类 readonly 能改变成员变量吗
	public static LatLng latLngMain = new(126.634944, 45.7434444);
	public static float orientationMain = 0.0f;
	
	public static bool locationFlag = false;
	public static bool initMapFlag = true;
	public static bool battleFlag = false;
	public static bool configFlag = false;
	public static bool onlineFlag = false;
	
	//当前运行的平台
	public static RuntimePlatform platform = Application.platform;
	

	//上一次debug信息打印的时间
	private long lastDebugTime = 0;

	// Use this for initialization
	void Start () {
		// AudioManager.Instance.PlayMusic("bgm");
		//网络监听
		//初始化
		PanelManager.Init();
		BattleManager.Init(); //NOTE:必须先初始化这个，因为hit协议的回调函数应该这个先执行，先算好，再在panel上展示
		
		
		// 判断平台，如果是手机就开定位和蓝牙
#if UNITY_EDITOR
		locationFlag = true;
#elif UNITY_ANDROID
		// Debug.Log("android location!");
		// AndroidManager.BlueToothInit(); // 蓝牙初始化
		// AndroidManager.SensorInit(); //方位角初始化

		USBSerialManager.Init();
		USBSerialManager.InterfaceInit();

		Input.location.Start();//定位初始化 unity原生
		StartCoroutine(Locator.GetLocation());
#else
		locationFlag = true;
#endif
		
		
		//===========================联机================================
		PanelManager.Open<StartPanel>(); // 开始界面
		
	}

	// Update is called once per frame
	void Update () {
		if (locationFlag && initMapFlag) //把地图初始化转移到一开始
		{
			initMapFlag = false;
			MapManager.InitMap(latLngMain);
			
			//===========================测试config================================
			// //如果没tank的id，就新建一个tank
			// if (!BattleManager.tanks.ContainsKey(GameMain.id))
			// {
			// 	//生成tank，这个tank在离开时会被清除，Battle界面重新生成一份
			// 	TankInfo tankInfo = new TankInfo
			// 	{
			// 		camp = GameMain.camp,
			// 		id = GameMain.id,
			// 		hp = 100
			// 	};
			// 	BattleManager.GenerateTank(tankInfo);//此时就应该在地图上出现圆圈
			// }
			// PanelManager.Open<ConfigPanel>(id); // 测试阶段，直接打开配置界面
		}
		
		DebugUpdate();
	}
	
	private void DebugUpdate(){
		//cd是否判断
		if(TimeUtils.GetMsStamp() - lastDebugTime < ConfigParam.DebugCd){
			return;
		}
		lastDebugTime = TimeUtils.GetMsStamp();

	}

	//关闭连接
	void OnConnectClose(string err){
		Debug.Log("断开连接");
	}

	
}
