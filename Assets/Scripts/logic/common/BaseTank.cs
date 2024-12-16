using System.Collections;
using System.Collections.Generic;
using Assist;
using UnityEngine;

/// <summary>
/// TODO:最后加个死亡处理
/// </summary>
public class BaseTank : MonoBehaviour {
	//坦克模型
	public GameObject skin;
 
	//生命值
	public int hp = ConfigParam.MaxHp;
	public int bulletNum = ConfigParam.DefaultBulletNum;
	public int magNum = ConfigParam.DefaultMagNum;
	public int bulletMaxNum = ConfigParam.DefaultBulletMaxNum;
	public int defense = ConfigParam.DefaultDefense;
	
	//属于哪一名玩家
	public string id = "";
	//阵营
	public int camp = (int)Camp.NotSet;

	public LatLng latLng = new(0,0);
	public float orientation = 0.0f;

	//初始化
	public virtual void Init(string skinPath){
		//皮肤
		GameObject skinRes = ResManager.LoadPrefab(skinPath);
		skin = Instantiate(skinRes, this.transform, true);
		
		//这里注意，skin是一个子对象，是模型的对象，而tank是父对象，是tank逻辑的对象，没模型。
		//所以最好让模型的父对象一直是tank，然后在更新的时候让tank和map的位置直接赋值更新到完全一样
		skin.transform.localPosition = Vector3.zero;
		skin.transform.localEulerAngles = Vector3.zero;
	}

	//是否死亡
	public bool IsDie(){
		return hp <= 0;
	}
	
	

	//被攻击
	public void Attacked(int _damage){
		//已经死亡
		if(IsDie()){
			return;
		}
		//扣血
		hp -= (_damage - defense);
	}
	
	//真实伤害，无视护甲
	public void RealAttacked(int _damage){
		//已经死亡
		if(IsDie()){
			return;
		}
		//扣血
		hp -= _damage;
	}
	

	// Use this for initialization
	public void Start () {

	}


	// Update is called once per frame
	public void Update () {

	}
	
	public void ChangeColor (Color color) {
		skin.transform.Find("Vis_object/ColoredCycle").GetComponent<SpriteRenderer>().color = color;
	}
}
