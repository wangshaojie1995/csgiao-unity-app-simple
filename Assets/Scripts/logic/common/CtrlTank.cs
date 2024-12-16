using System.Collections.Generic;
using Assist;
using UnityEngine;
using utils;

/// <summary>
/// TODO:加蓝牙到fire里，换地图定位到move里
/// TODO:设计射速血包等等各种东西
/// </summary>
public class CtrlTank : BaseTank
{
    //上一次发送同步信息的时间
    private long lastSendSyncTime;
    //上一次定位时间
    private long lastLocationTime;
    //上一次定位时间
    private long lastSensorTime;
    //上一次USB时间
    private long lastUSBSerialTime;

    new void Update()
    {
        
        
        base.Update();
        
#if UNITY_EDITOR
        // 获取键盘输入
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        latLng = GameMain.latLngMain;
        latLng.lat += verticalInput * ConfigParam.KeyBoardMoveSpeed;
        latLng.lng += horizontalInput * ConfigParam.KeyBoardMoveSpeed;
        GameMain.latLngMain = latLng;
#elif UNITY_ANDROID
		//移动控制
		MoveUpdate();
        //USB serial数据更新
        USBSerialUpdate();


		// //方位角更新
		// OrientationUpdate();
		// //蓝牙更新
		// BlueToothUpdate();
#else
        // 同上
        // 获取键盘输入
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        latLng = GameMain.latLngMain;
        latLng.lat += verticalInput * ConfigParam.KeyBoardMoveSpeed;
        latLng.lng += horizontalInput * ConfigParam.KeyBoardMoveSpeed;
        GameMain.latLngMain = latLng;
#endif
        //注：区域检查放在服务器，因为区域类型很多，客户端整不明白
        
        //发送同步信息
        SyncUpdate();
    }

    //移动控制
    private void MoveUpdate()
    {
        // if(IsDie()){ //已经死亡
        // 	return;
        // }

        //cd是否判断
        if (TimeUtils.GetMsStamp() - lastLocationTime < ConfigParam.LocationCd)
        {
            return;
        }

        lastLocationTime = TimeUtils.GetMsStamp();

        StartCoroutine(Locator.GetLocation());
        latLng = GameMain.latLngMain;
    }

    private void OrientationUpdate()
    {
        // if(IsDie()){ //已经死亡
        // 	return;
        // }

        //cd是否判断
        if (TimeUtils.GetMsStamp() - lastSensorTime < ConfigParam.SensorCd)
        {
            return;
        }

        lastSensorTime = TimeUtils.GetMsStamp();
        
        // 经考察不需要方位角
        // AndroidManager.getAndroidSensor();
        // orientation = GameMain.orientationMain;
    }

    //发送同步信息
    private void SyncUpdate()
    {

    }


    /////////////////////////////////////////////////////////////////////////////
    ///
    
    //USB更新 
    private void USBSerialUpdate()
    {
        //cd是否判断
        if (TimeUtils.GetMsStamp() - lastUSBSerialTime < ConfigParam.UsbSerialCd)
        {
            return;
        }
        lastUSBSerialTime = TimeUtils.GetMsStamp();
        
        while (USBSerialManager.strQueue.TryDequeue(out string str))
        {
            string[] blueToothArgs = str.Split("|");
            string msgName = blueToothArgs[0]; //协议名： Fire
            string msgBody = blueToothArgs[1]; //协议体： 10,100
            string[] bodyArgs = msgBody.Split(',');
            switch (msgName)
            {
                case "Fire":
                    //开炮
                    FireUpdate(int.Parse(bodyArgs[0]));
                    break;
                case "Hit":
                    //受伤
                    HitUpdate(int.Parse(bodyArgs[0])); //传的是伤害
                    break;
                case "Reload":
                    ReloadUpdate(int.Parse(bodyArgs[0]), int.Parse(bodyArgs[1]));
                    break;
                default:
                    Debug.Log(msgName);
                    break;
            }
        }
    }
    
    public void FireUpdate(int _bulletNum)
    {
        //已经死亡
        if (IsDie()) { return; }

        bulletNum = _bulletNum;
        
        AudioManager.Instance.PlaySfx("fire");

    }

    //受伤
    public void HitUpdate(int _damage)
    {
        //已经死亡
        if (IsDie())
        {
            return;
        }
        
        base.Attacked(_damage);
        
        //受伤后再判断是否死亡
        if (IsDie())
        {
            AudioManager.Instance.PlaySfx("die");
            PanelManager.Open<DeadPanel>();
            USBSerialManager.InterfaceSend("Die");
        }
        else
        {
            AudioManager.Instance.PlaySfx("hit");
        }

    }

    //换弹
    public void ReloadUpdate(int _bulletNum, int _magNum)
    {
        //已经死亡
        if (IsDie())
        {
            return;
        }

        if (_magNum < 0)
        {
            return;
        }

        bulletNum = _bulletNum;
        magNum = _magNum;


        Debug.Log("Reload!");
        AudioManager.Instance.PlaySfx("reload");
        //换弹应该不用同步协议		
    }
    
    
    //复活
    public void Resurrect(){
        hp = ConfigParam.MaxHp; //恢复满血即可
        
#if UNITY_EDITOR
#elif UNITY_ANDROID
		if (USBSerialManager.USBSerialFlag == false)
		{
			PanelManager.Open<TipPanel>("枪支断开连接，异常！！！");
		}
		else
		{
			USBSerialManager.InterfaceSend("Resurrect");
		}
#endif

        AudioManager.Instance.PlaySfx("resurrect");
        PanelManager.Open<ResurrectPanel>();
		
        Debug.Log("Resurrection, my brave warrior!!!");
    }
    
    
}