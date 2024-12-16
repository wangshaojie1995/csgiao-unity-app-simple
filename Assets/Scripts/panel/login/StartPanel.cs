using System.Collections;
using System.Collections.Generic;
using Assist;
using UnityEngine;
using UnityEngine.UI;
//测试

public class StartPanel : BasePanel {
	
	private InputField IpInput; //ip地址输入框
	private InputField PortInput; //密码输入框
	private Button ConnectBtn; //联机按钮
	private Button SingleBtn; //单机按钮


	
	//初始化
	public override void OnInit() {
		skinPath = "StartPanel";
		layer = PanelManager.Layer.Panel;
	}

	//显示
	public override void OnShow(params object[] args) {
		
		//寻找组件
		IpInput = skin.transform.Find("IpInput").GetComponent<InputField>();
		PortInput = skin.transform.Find("PortInput").GetComponent<InputField>();
		ConnectBtn = skin.transform.Find("ConnectBtn").GetComponent<Button>();
		SingleBtn = skin.transform.Find("SingleBtn").GetComponent<Button>();
		
		//监听
		ConnectBtn.onClick.AddListener(OnConnectBtnClick);
		SingleBtn.onClick.AddListener(OnSingleBtnClick);

	}

	//关闭
	public override void OnClose() {
		
	}
	
	
	////////////////////////////////////////////////////
	//// 按键
	////////////////////////////////////////////////////

	//当按下单机按钮
	public void OnSingleBtnClick() {
		AudioManager.Instance.PlaySfx("btn_operate");
		GameMain.onlineFlag = false;
		
#if UNITY_EDITOR
#elif UNITY_ANDROID
		//进入主界面之后默认自动连接USB
		USBSerialManager.InterfaceConnect();
#endif
		
		// ===========================单机================================
		 GameMain.id = "cat";
		 TankInfo tankInfo = new TankInfo();
		 tankInfo.camp = (int)Camp.Red;
		 tankInfo.id = GameMain.id;
		 tankInfo.hp = ConfigParam.MaxHp;
		 BattleManager.GenerateTank(tankInfo);//此时就应该在地图上出现圆圈
		 PanelManager.Open<BattlePanel>();
		 
		 Close();
	}

	//当按下联机按钮
	public void OnConnectBtnClick() {
		AudioManager.Instance.PlaySfx("btn_open");
		GameMain.onlineFlag = true;
		
		//用户名密码为空
		if (IpInput.text == "" || PortInput.text == "") {
			PanelManager.Open<TipPanel>("ip和端口号不能为空");
			return;
		}
		
		//打开登录界面
		PanelManager.Open<TipPanel>("联机模式未开源，请前往立创开源商城下载apk游玩"); // 连接云服务器

	}
}
