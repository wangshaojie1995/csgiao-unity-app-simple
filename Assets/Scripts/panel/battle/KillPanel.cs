using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using utils;

public class KillPanel : BasePanel {
	//界面开始显示的时间
	private long startTime = 0;

	//初始化
	public override void OnInit() {
		skinPath = "KillPanel";
		layer = PanelManager.Layer.Tip;
	}
	
	//显示
	public override void OnShow(params object[] args) {
		startTime = TimeUtils.GetMsStamp();
	}
		
	//关闭
	public override void OnClose() {

	}

	//定时关闭即可
	public void Update(){
		if(TimeUtils.GetMsStamp() - startTime > ConfigParam.KillPanelDuring){
			Close();
		}
	}
}
