using System;
using System.Collections;
using System.Collections.Generic;
using Assist;
using UnityEngine;
using UnityEngine.UI;

//TODO：这个只是PointMode的面板，之后的夺旗模式和吃鸡模式都要新做面板
public class BattlePanel : BasePanel
{

	private Image redBar;
	private Image blueBar;
	
	private Button buttonHit;
	private Button buttonFire;
	private Button buttonReload;
	private Button ResurrectButton;

	private CtrlTank ctrlTank;

	private Text redCampText;
	private Text blueCampText;
	private Text defenseText;
	private Text hpText;
	private Text bulletText;
	private Text magText;

	private bool initflag = false;

	//初始化
	public override void OnInit()
	{
		skinPath = "BattlePanel";
		layer = PanelManager.Layer.Panel;
	}

	//显示
	public override void OnShow(params object[] args)
	{
		GameMain.battleFlag = true;

		redBar = skin.transform.Find("Panel/CampInfo/RedInfo/RedBar").GetComponent<Image>();
		blueBar = skin.transform.Find("Panel/CampInfo/BlueInfo/BlueBar").GetComponent<Image>();
		
		redCampText = skin.transform.Find("Panel/CampInfo/RedInfo/RedText").GetComponent<Text>();
		blueCampText = skin.transform.Find("Panel/CampInfo/BlueInfo/BlueText").GetComponent<Text>();
		defenseText = skin.transform.Find("Panel/DefenseBar/DefenseText/Text").GetComponent<Text>();
		hpText = skin.transform.Find("Panel/HpBar/HpText/Text").GetComponent<Text>();
		bulletText = skin.transform.Find("Panel/Bullet/Bullet/Text").GetComponent<Text>();
		magText = skin.transform.Find("Panel/Bullet/Mag/Text").GetComponent<Text>();

		//只有单机模式需要用到复活按钮
		ResurrectButton = skin.transform.Find("ResurrectButton").GetComponent<Button>();
		ResurrectButton.onClick.AddListener(() =>
		{
			if (!BattleManager.GetCtrlTank().IsDie())
			{
				return;
			}
			BattleManager.GetCtrlTank().Resurrect();
		});
		
		if (GameMain.onlineFlag == true)
		{
			ResurrectButton.gameObject.SetActive(false);
		}
		
		buttonHit = skin.transform.Find("ButtonHit").GetComponent<Button>();
		buttonFire = skin.transform.Find("ButtonFire").GetComponent<Button>();
		buttonReload = skin.transform.Find("ButtonReload").GetComponent<Button>();
#if UNITY_EDITOR
		buttonHit.onClick.AddListener(OnButtonHitClick);
		buttonFire.onClick.AddListener(OnButtonFireClick);
		buttonReload.onClick.AddListener(OnButtonReloadClick);
#elif UNITY_ANDROID
		buttonHit.gameObject.SetActive(false);
		buttonFire.gameObject.SetActive(false);
		buttonReload.gameObject.SetActive(false);
#endif


		
		
#if UNITY_EDITOR
#elif UNITY_ANDROID
		//USB发送给枪械开战信息
		if (USBSerialManager.USBSerialFlag == false)
		{
			PanelManager.Open<TipPanel>("未连接枪械，异常！！！");
		}
		else
		{
			USBSerialManager.InterfaceSend("EnterBattle");
		}
#endif
		
		ctrlTank = (CtrlTank)BattleManager.GetCtrlTank();

		initflag = true;
	}

	//关闭
	public override void OnClose()
	{
	}


	private void Update()
	{
		if (ctrlTank == null)
		{
			return;
		}
		
		if (initflag == false)
		{
			return;
		}

		ReflashHp(ctrlTank.hp);
		ReflashBullet(ctrlTank.bulletNum);
		ReflashMag(ctrlTank.magNum);
		ReflashDefense(ctrlTank.defense);

	}
	
	/////////////////////////////////////////////////////////////////////
	///// 网络协议回调
	/////////////////////////////////////////////////////////////////////


	

	/////////////////////////////////////////////////////////////////////
	///// 按钮回调
	/////////////////////////////////////////////////////////////////////
	
	public void OnButtonHitClick()
	{
		ctrlTank.HitUpdate(15);
	}
	
	public void OnButtonFireClick()
	{
		// PanelManager.Open<KillPanel>();
		ctrlTank.FireUpdate(ctrlTank.bulletNum - 1);
	}
	
	public void OnButtonReloadClick()
	{
		ctrlTank.ReloadUpdate(ctrlTank.bulletMaxNum,ctrlTank.magNum - ctrlTank.bulletMaxNum);
	}


	/////////////////////////////////////////////////////////////////////////
	// ui 相关
	/////////////////////////////////////////////////////////////////////////


	//更新hp
	public void ReflashHp(int hp)
	{
		hpText.text = hp.ToString();
		
	}

	//更新子弹数
	public void ReflashBullet(int bullet)
	{
		bulletText.text = bullet.ToString();
	}

	//更新弹夹子弹数
	public void ReflashMag(int mag)
	{
		magText.text = mag.ToString();
	}

	//更新防御值
	public void ReflashDefense(int shield)
	{
		defenseText.text = shield.ToString();
	}

	//更新红方进度条
	public void ReflashRedBar(int _redBar)
	{
		redBar.fillAmount = _redBar / 100f;
		redCampText.text = _redBar.ToString();
	}

	//更新蓝方进度条
	public void ReflashBlueBar(int _blueBar)
	{
		blueBar.fillAmount = _blueBar / 100f;
		blueCampText.text = _blueBar.ToString();
	}



}
