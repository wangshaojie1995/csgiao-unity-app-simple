using System;
using System.Collections;
using System.Collections.Generic;
using Assist;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// finished
/// </summary>
public class SerialPanel : BasePanel {


	//连接按钮
	private Button btnConnect;
	private Button btnDisconnect;
	//测试按钮
	private Button btnTest;

	//关闭按钮
	private Button btnClose;
	
	//初始化按钮
	private Button btnInit;
	
	
	private Text textInfo;
	

	//初始化
	public override void OnInit() {
		skinPath = "SerialPanel";
		layer = PanelManager.Layer.Panel;
	}

	//显示
	public override void OnShow(params object[] args) {
		//寻找组件
		btnConnect = skin.transform.Find("ButtonConnect").GetComponent<Button>();
		btnDisconnect = skin.transform.Find("ButtonDisconnect").GetComponent<Button>();
		btnTest = skin.transform.Find("ButtonTest").GetComponent<Button>();
		btnClose = skin.transform.Find("ButtonClose").GetComponent<Button>();
		btnInit = skin.transform.Find("ButtonInit").GetComponent<Button>();
		
		textInfo = skin.transform.Find("TextInfo").GetComponent<Text>();
		
		//监听
		btnConnect.onClick.AddListener(OnConnectClick);
		btnDisconnect.onClick.AddListener(OnDisconnectClick);
		btnTest.onClick.AddListener(OnTestClick);
		btnClose.onClick.AddListener(OnCloseClick);
		btnInit.onClick.AddListener(OnInitClick);

	}

	//关闭
	public override void OnClose() {

	}


	
	private void OnConnectClick() //连接按钮回调
	{
		AudioManager.Instance.PlaySfx("btn_operate");


		if (!USBSerialManager.InterfaceConnect())
		{
			return;
		}

		BaseTank tank = BattleManager.GetCtrlTank();
		if (tank == null)
		{
			return;
		}
		
		//如果已经有 CtrlTank（大概率是断线重连），就把状态同步给枪
		USBSerialManager.InterfaceSend(
			String.Format("UpdateData|{0},{1},{2},",
			tank.hp,
			tank.bulletNum,
			tank.magNum)
			);
	}
	
	private void OnDisconnectClick() //断开按钮回调
	{
		AudioManager.Instance.PlaySfx("btn_operate");
		USBSerialManager.InterfaceDisconnect();
	}
	
	private void OnTestClick() //测试发送按钮
	{
		AudioManager.Instance.PlaySfx("btn_operate");
		USBSerialManager.InterfaceTest();
		
	}
	
	private void OnInitClick() //测试发送按钮
	{
		AudioManager.Instance.PlaySfx("btn_operate");
		USBSerialManager.InterfaceInit();
		
	}

	private void OnCloseClick() { //关闭按钮回调
		AudioManager.Instance.PlaySfx("btn_close");
		Close();
		
	}

	private void Update()
	{
		if (USBSerialManager.strQueue.TryPeek(out string str))
		{
			textInfo.text = str;
		}
	}
}
