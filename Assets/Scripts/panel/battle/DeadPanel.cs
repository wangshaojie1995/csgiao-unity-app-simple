using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using utils;

public class DeadPanel : BasePanel {
	
	private SpriteRenderer Angel;
	private SpriteRenderer redAngel;
	private SpriteRenderer blueAngel;

	private AnimationCurve showCurve = AnimationCurve.EaseInOut(0,0,1,1);
	private float showAnimationSpeed = ConfigParam.AnimationSpeed;
	
	//初始化
	public override void OnInit() {
		skinPath = "DeadPanel";
		layer = PanelManager.Layer.Tip;
	}
	
	//显示
	public override void OnShow(params object[] args) {
		
		redAngel = skin.transform.Find("RedAngel").GetComponent<SpriteRenderer>();
		blueAngel = skin.transform.Find("BlueAngel").GetComponent<SpriteRenderer>();
		
		//把对面的天使隐藏了
		switch (GameMain.camp)
		{
			case (int)Camp.Blue:
				redAngel.gameObject.SetActive(false);
				Angel = blueAngel;
				break;
			case (int)Camp.Red:
				blueAngel.gameObject.SetActive(false);
				Angel = redAngel;
				break;
			default:
				blueAngel.gameObject.SetActive(false);
				Angel = redAngel;
				break;
		}
		StartCoroutine(ShowAngel(Angel));
		
	}
		
	//关闭
	public override void OnClose() {

	}

	//定时关闭即可
	public void Update(){

	}
	
	IEnumerator ShowAngel(SpriteRenderer angel)
	{
		float timer = 0;
		while (timer <= 1)
		{
			Color color = angel.color;
			angel.color= new Color(color.r,color.g,color.b,showCurve.Evaluate(timer)) ;
			
			timer += Time.deltaTime * showAnimationSpeed;
			yield return null;
		}
	}
}
