using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assist;
using UnityEngine;
using UnityEngine.UI;
using utils;
using Object = UnityEngine.Object;

/// <summary>
/// finished
/// </summary>
public class BattleManager : MonoBehaviour{
	//战场中的坦克
	public static Dictionary<string, BaseTank> tanks = new();

	
	
	//初始化
	public static void Init() {

	}
	

	//添加坦克
	private static void AddTank(string id, BaseTank tank){
		tanks[id] = tank;
	}

	//删除坦克
	private static void RemoveTank(string id){
		if (tanks[id].gameObject != null)
		{
			Object.Destroy(tanks[id].gameObject);
		}
		
		tanks.Remove(id);
	}

	//获取坦克
	private static BaseTank GetTank(string id) {
		if(tanks.TryGetValue(id, out var tank)){
			return tank;
		}
		return null;
	}

	//获取玩家控制的坦克
	public static CtrlTank GetCtrlTank() {
		return (CtrlTank)GetTank(GameMain.id);
	}

	//重置战场
	public static void Reset() {

	}
	
	public static void StartBattle()
	{
		//打开界面
		PanelManager.Open<BattlePanel>();
	}
	
	//产生坦克
	public static void GenerateTank(TankInfo tankInfo){
		//GameObject
		string objName = "Tank_" + tankInfo.id;
		GameObject tankObj = new GameObject(objName);
		//AddComponent
		BaseTank tank;

		tank = tankObj.AddComponent<CtrlTank>();

		//属性
		tank.camp = tankInfo.camp;
		tank.id = tankInfo.id;
		tank.hp = tankInfo.hp;
		//pos rotation
		tank.latLng = tankInfo.latLng;
		tank.orientation = tankInfo.ez;

		//TODO：之后还能同步每个人的头像，但是先不要管了。
		tank.Init("tank");
		if(tankInfo.id == GameMain.id)
		{
			tank.ChangeColor(Color.green);
		}
		else if(tankInfo.camp == GameMain.camp){
			tank.ChangeColor(Color.blue);
		}
		else{
			tank.ChangeColor(Color.red);
		}
		//列表
		AddTank(tankInfo.id, tank);
	}
	
	
	
	//////////////////////////////////////////////////////////////////
	///  下面关于mode的接口
	//////////////////////////////////////////////////////////////////

	
	
	
}
