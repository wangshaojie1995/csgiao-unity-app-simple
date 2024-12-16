using System.Collections;
using UnityEngine;
using utils;

namespace Assist
{
    public static class Locator
    {

        public static IEnumerator GetLocation()
        {
            while (Input.location.status == LocationServiceStatus.Initializing)
            {
                Debug.Log("Initing");
                yield return new WaitForSeconds(1);
            } 
            
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                Debug.LogError("Unable to determine device location");
                yield break;
            }

            // Debug.Log("Location Successfully!");
            GameMain.locationFlag = true;
            
            
            GpsUtils.WGS84_to_GCJ02(Input.location.lastData.latitude, Input.location.lastData.longitude,
                out GameMain.latLngMain.lat,  out GameMain.latLngMain.lng);

            // GameMain.latLngMain.Lat = Input.location.lastData.latitude;
            // GameMain.latLngMain.Lng = Input.location.lastData.longitude;
            
            // Debug.Log(GameMain.latLngMain.lat + ", "+ GameMain.latLngMain.lng);

        }
    }
}