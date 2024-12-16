using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using utils;


namespace Assist
{
    /// <summary>
    /// 这个类是与底层jar包的蓝牙接口，已全部改成静态
    /// </summary>
    public class USBSerialManager : MonoBehaviour
    {
        private static AndroidJavaClass jc;
        private static AndroidJavaObject jo;

        public static ConcurrentQueue<string> strQueue = new();

        public static bool USBSerialFlag = false;

        // Start is called before the first frame update
        public static void Init()
        {
            // //获得com.unity3d.player.UnityPlayer 下的类，对于扩展的Activity 是一个固定的写法。只要记住就行了
            //  jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            //  //获得 jc 类中的 currentActivity 对象，也是一种固定的写法
            //  jo = jc.GetStatic<AndroidJavaObject>("currentActivity");

            jo = new AndroidJavaObject("com.sytnocui.usbserial.USBSerialManager");
        }
        
        public static void InterfaceInit()
        {
            jo.Call<bool>("USBSerialInit");
        }

        public static bool InterfaceConnect()
        {
            bool _flag = jo.Call<bool>("USBSerialOpen");
            USBSerialFlag = _flag;
            return _flag;
        }

        public static void InterfaceDisconnect()
        {
            if (USBSerialFlag == false)
            {
                return;
            }
            
            jo.Call<bool>("USBSerialClose");
            USBSerialFlag = false;
        }
        
        public static void InterfaceTest()
        {
            if (USBSerialFlag == false)
            {
                return;
            }
            
            bool res = jo.Call<bool>("USBSerialWrite","Hello, World");
            Debug.Log(res);
        }
        
        public static void InterfaceSend(string str)
        {
            if (USBSerialFlag == false)
            {
                return;
            }
            
            jo.Call<bool>("USBSerialWrite", str);
        }
        
        
        /// <summary>
        /// 原生层通过该方法传回信息
        /// </summary>
        /// <param name="content"></param>
        public void FromAndroid(string content)
        {
            strQueue.Enqueue(content);

            Text text =  GameObject.Find("Root/Canvas/DebugText").GetComponent<Text>();
            text.text += content;
            if (text.text.Length >= 500)
            {
                text.text = "";
            }
        }

    }

}