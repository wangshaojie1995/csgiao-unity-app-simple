using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using utils;

public class ResurrectPanel : BasePanel {
	//界面开始显示的时间
	private long startTime = 0;
	private bool isClosing = false;
	
	private SpriteRenderer Angel;
	private SpriteRenderer redAngel;
	private SpriteRenderer blueAngel;

	private AnimationCurve showCurve = AnimationCurve.EaseInOut(0,0,1,1);
	private AnimationCurve hideCurve = AnimationCurve.EaseInOut(0,1,1,0);

	//初始化
	public override void OnInit() {
		skinPath = "ResurrectPanel";
		layer = PanelManager.Layer.Tip;
	}
	
	//显示
	public override void OnShow(params object[] args) {
		startTime = TimeUtils.GetMsStamp();
		
		PanelManager.Close("DeadPanel");
		
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
		PanelManager.Close("DeadPanel");
	}

	//定时关闭即可
	public void Update(){
		if(TimeUtils.GetMsStamp() - startTime > ConfigParam.AnimationHoldTime && isClosing == false)
		{
			isClosing = true;
			StartCoroutine(HideAngel(Angel));
		}
		
		if(TimeUtils.GetMsStamp() - startTime > ConfigParam.AnimationHoldTime + 1)
		{
			Close();
		}
	}
	
	
	IEnumerator ShowAngel(SpriteRenderer angel)
	{
		float timer = 0;
		while (timer <= 1)
		{
			Color color = angel.color;
			angel.color= new Color(color.r,color.g,color.b,showCurve.Evaluate(timer)) ;
			
			timer += Time.deltaTime * ConfigParam.AnimationSpeed;
			yield return null;
		}
	}
	
	IEnumerator HideAngel(SpriteRenderer angel)
	{
		float timer = 0;
		while (timer <= 1)
		{
			Color color = angel.color;
			angel.color= new Color(color.r, color.g, color.b, hideCurve.Evaluate(timer) ) ;
			
			timer += Time.deltaTime * ConfigParam.AnimationSpeed;
			yield return null;
		}
	}
}
