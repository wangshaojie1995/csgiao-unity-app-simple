using System;
using System.Collections;
using System.Collections.Generic;
using Assist;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// finished
/// </summary>
public class DebugPanel : BasePanel {

	
	private Button Button1;
	private Button Button2;
	private Button Button3;
	private Button Button4;

	//关闭按钮
	private Button btnClose;
	

	//初始化
	public override void OnInit() {
		skinPath = "DebugPanel";
		layer = PanelManager.Layer.Panel;
	}

	//显示
	public override void OnShow(params object[] args) {
		//寻找组件
		Button1 = skin.transform.Find("Button1").GetComponent<Button>();
		Button2 = skin.transform.Find("Button2").GetComponent<Button>();
		Button3 = skin.transform.Find("Button3").GetComponent<Button>();
		Button4 = skin.transform.Find("Button4").GetComponent<Button>();
		btnClose = skin.transform.Find("ButtonClose").GetComponent<Button>();

		
		//监听
		Button1.onClick.AddListener(OnClickButton1);
		Button2.onClick.AddListener(OnClickButton2);
		Button3.onClick.AddListener(OnClickButton3);
		Button4.onClick.AddListener(OnClickButton4);
		btnClose.onClick.AddListener(OnCloseClick);

	}

	//关闭
	public override void OnClose()
	{
	}


	private void OnClickButton1() 
	{
		CtrlTank tank =  BattleManager.GetCtrlTank();
		if (tank == null)
		{
			return;
		}
		
		tank.defense = 10;
		//TODO:目前无法改伤害，只能改护盾
#if UNITY_EDITOR
#elif UNITY_ANDROID
		USBSerialManager.InterfaceSend(
			String.Format("ChangeArmor|{0},{1}",
				tank.defense,   // defense
				0.5f  // HitCD
				)
		);
#endif
		
	}
	
	private void OnClickButton2()
	{
		CtrlTank tank =  BattleManager.GetCtrlTank();
		if (tank == null)
		{
			return;
		}
		
		tank.bulletNum = 100;
		tank.magNum = 300;
		tank.bulletMaxNum = 50;
#if UNITY_EDITOR
#elif UNITY_ANDROID
		USBSerialManager.InterfaceSend(
			String.Format("ChangeGun|{0},{1},{2},{3}",
				tank.bulletNum,   // bulletNum
				tank.magNum,   // mag_num
				tank.bulletMaxNum,  // bullet_max_num
				0.10f  // FireCD
				)
			);
#endif
	}
	
	private void OnClickButton3() 
	{

	}
	
	private void OnClickButton4() 
	{

	}
	
	

	private void OnCloseClick() { //关闭按钮回调
		AudioManager.Instance.PlaySfx("btn_close");
		Close();
	}
}
