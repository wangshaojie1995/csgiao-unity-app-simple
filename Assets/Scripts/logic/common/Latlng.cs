using UnityEngine.Serialization;

/// <summary>
/// 经纬度
/// </summary>
[System.Serializable]
public class LatLng
{
    public double lng; //经度
    public double lat; //纬度
    
    public LatLng(double longitude, double latitude)
    {
        lng = longitude;
        lat = latitude;
    }
}