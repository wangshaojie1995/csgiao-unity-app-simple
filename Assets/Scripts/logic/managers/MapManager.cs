using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Networking;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// TODO：要加上用户行走带来的地图位置变化
/// </summary>
public class MapManager : MonoBehaviour
{
    private const float BOUNDARY = 1.3f; //
    
    private static int x_tank, y_tank; //当前tank位置的  高德地图瓦片坐标 数量级很大
    private static int x_pixel, y_pixel; // 当前tank位置的  高德地图 瓦片内的像素位置， 量纲：0-256，瓦片正中间即 pixelx = 128
    public static float x_origin, y_origin; // 初次定位的 tank 的 高德瓦片坐标的xy值，浮点数，（int x与int pixelx的组合即为float的x）
    
    private static int x_pos = 0;
    private static int y_pos = 0;
    
    public static Vector2 pixelDrift;

    private static Dictionary<string, GameObject> maps = new();

    private static int gridExpansion = 3; //膨胀的栅格数目，必须是奇数。

    public static GameObject _map;
    public static GameObject map_prefeb;

    public void Start()
    {
        _map = GameObject.Find("map");
        map_prefeb = ResManager.LoadPrefab("map");
    }


    static public void InitMap(LatLng latLng)
    {
        if (GameMain.locationFlag == false)
        {
            Debug.Log("没有定位！！！");
            return;
        }
        
        //这里的xy都是类的成员变量
        LatLngTransformer.LatLngToTileXY(latLng, 16, out x_tank, out y_tank, out x_pixel, out y_pixel);
        LatLngTransformer.LatLngToFloatXY(latLng, 16, out x_origin, out y_origin);
        pixelDrift.x = (x_pixel - 128.0f) / 256.0f * 2.56f;
        pixelDrift.y = -(y_pixel - 128.0f) / 256.0f * 2.56f;
        InputManager.CameraDrift.x = pixelDrift.x;
        InputManager.CameraDrift.y = pixelDrift.y;
        int init_ix, init_iy;
        for (init_ix = -gridExpansion/2 ; init_ix < gridExpansion/2 + 1 ; init_ix++)
        {
            for (init_iy = -gridExpansion/2; init_iy < gridExpansion/2 + 1 ; init_iy++)
            {
                GameObject new_map = Instantiate(map_prefeb, _map.transform);
                new_map.transform.position = new Vector3(2.56f * init_ix, 2.56f * init_iy, 0);
                new_map.name = "map_" + init_ix.ToString() + "_" + init_iy.ToString();
                maps.Add(new_map.name,new_map);
                int x = x_tank + init_ix;
                int y = y_tank - init_iy;
                _map.GetComponent<MapManager>().StartCoroutine(set_one_map(x, y, new_map));
            }
        }
    }
    public void Update()
    {
        if (!GameMain.locationFlag)
        {
            return;
        }
        
        //先改变所有玩家相对map的坐标位置
        //只显示友方角色
        BaseTank baseTank = BattleManager.GetCtrlTank();
        if (baseTank == null)
        {
            return;
        }
        
        foreach(BaseTank tank in BattleManager.tanks.Values)
        {
            // //如果是本地玩家，放置于屏幕中间
            // 注意：这里必须改变的是摄像头的位置，如果改变map会与下面的移位逻辑冲突
            if (tank.id == GameMain.id)
            {
                //本地玩家多写一句，直接把latLngMain赋值，防止tank初始化的时候Latlng是0.引起摄像头位置突变
                tank.transform.position = LatLng2Tf(GameMain.latLngMain); 
                Camera.main.transform.position = new Vector3(
                    tank.transform.position.x - pixelDrift.x + InputManager.CameraDrift.x + InputManager.CameraDriftStart.x, 
                    tank.transform.position.y - pixelDrift.y + InputManager.CameraDrift.y + InputManager.CameraDriftStart.y, 
                    -10);
                continue;
            }
            //到这里的都是syncTank
            //通过经纬度算出该显示的坐标。
            tank.transform.position = LatLng2Tf(tank.latLng);
            tank.transform.localEulerAngles  = new Vector3(0, 0, tank.orientation);
            
        }

        CheckShift();
    }

    //超出当前地图碎片边界范围后更新
    private void CheckShift()
    {
        if (Camera.main.transform.position.x  >= x_pos * 2.56f + BOUNDARY + InputManager.CameraDriftCheck.x)
        {
            x_pos++;
            Update_Map();
            
        }

        if (Camera.main.transform.position.x  <= x_pos * 2.56f - BOUNDARY + InputManager.CameraDriftCheck.x)
        {
            x_pos--;
            Update_Map();
        }

        if (Camera.main.transform.position.y  >= y_pos * 2.56f + BOUNDARY + InputManager.CameraDriftCheck.y)
        {
            y_pos++;
            Update_Map();
        }

        if (Camera.main.transform.position.y  <= y_pos * 2.56f + BOUNDARY + InputManager.CameraDriftCheck.y)
        {
            y_pos--;
            Update_Map();
        }
    }

    //检查当前周围3x3范围地图，并补充缺少部分
    private void Update_Map()
    {
        int ix, iy;
        int xtest, ytest;
        string stest;
        for(ix = -gridExpansion/2; ix < gridExpansion/2+1 ; ix++)
        {
            for (iy = -gridExpansion/2; iy < gridExpansion/2+1; iy++)
            {
                xtest = x_pos + ix;
                ytest = y_pos + iy;
                stest = "map_" + xtest.ToString() + "_" +  ytest.ToString();
                if(!maps.ContainsKey(stest))
                {
                    Set_New_Map(xtest,ytest);
                }
            }
        }
        
    }
    
    //根据地图的相对位置编号生成新地图
    private void Set_New_Map(int map_x, int map_y)
    {
        GameObject new_map = Instantiate(map_prefeb, _map.transform);
        new_map.transform.position = new Vector3(2.56f * map_x, 2.56f * map_y, 0);
        new_map.name = "map_" + map_x.ToString() + "_" + map_y.ToString();
        maps.Add(new_map.name,new_map);
        int x = x_tank + map_x;
        int y = y_tank - map_y; 
        StartCoroutine(set_one_map(x, y, new_map));
    }

    //获取地图碎片覆盖在单个地图上
    private static IEnumerator set_one_map(int x,int y,GameObject s)
    {
        string url = string.Format("https://webrd01.is.autonavi.com/appmaptile?x={0}&y={1}&z=16&lang=zh_cn&size=1&scale=1&style=8", x, y);
        var webRequest = UnityWebRequestTexture.GetTexture(url);

        yield return webRequest.SendWebRequest();

        // 检查是否有网络错误
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("网络请求出错: " + webRequest.error);
        }
        else
        {
            // 请求成功，可以处理响应数据
            var texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
            Sprite sprite = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), new Vector2(0.5f, 0.5f));
            SpriteRenderer spriteRenderer = s.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            bool responseData = webRequest.downloadHandler.isDone;
            yield return responseData;
        }
    }
    
    ////////////////////////////////////////////////////////////////////////
    // 工具函数
    ////////////////////////////////////////////////////////////////////////
    
    //换算单位 meter 到 unity坐标
    public static double Meter2Tf(double meter)
    {
        //这里墨卡托投影，一格代表多少米是跟纬度有关系的，需要乘个cos纬度
        return meter * 2.56 / (611.4962263  * Math.Cos(GameMain.latLngMain.lat * Math.PI / 180.0)); 
    }
    
    //换算单位 unity坐标 到 meter
    public static double Tf2Meter(double tf)
    {
        //这里墨卡托投影，一格代表多少米是跟纬度有关系的，需要乘个cos纬度
        return tf * (611.4962263  * Math.Cos(GameMain.latLngMain.lat * Math.PI / 180.0)) / 2.56;
    }

    //经纬度 转 Unity坐标
    public static Vector3 LatLng2Tf(LatLng latLng)
    {
        LatLngTransformer.LatLngToFloatXY(latLng, 16, out float rx, out float ry);
        return new Vector3(
            _map.transform.position.x + (rx - x_origin) * 2.56f + pixelDrift.x,
            _map.transform.position.y - (ry - y_origin) * 2.56f + pixelDrift.y,
            -1);
    }
    //Unity坐标 转 经纬度
    public static LatLng Tf2LatLng(Vector3 position)
    {
        LatLngTransformer.FloatXYToLatLng(
            (position.x - _map.transform.position.x - pixelDrift.x) / 2.56f + x_origin,
            -(position.y - _map.transform.position.y - pixelDrift.y) / 2.56f + y_origin, 16,
            out double lng, out double lat);
        return new LatLng(lng, lat);
    }
}


