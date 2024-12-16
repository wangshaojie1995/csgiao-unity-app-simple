using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// xy到LatLng的翻译器
/// </summary>
public class LatLngTransformer
{
    private const double CONSTANTS_RADIUS_OF_EARTH = 6371000; // 地球半径，单位：米

    public static LatLng TileXYToLatLng(int tileX, int tileY, int zoom, int pixelX = 0, int pixelY = 0)
    {
        double size = Math.Pow(2, zoom);
        double pixelXToTileAddition = pixelX / 256.0;
        double lng = (tileX + pixelXToTileAddition) / size * 360.0 - 180.0;

        double pixelYToTileAddition = pixelY / 256.0;
        double lat = Math.Atan(Math.Sinh(Math.PI * (1 - 2 * (tileY + pixelYToTileAddition) / size))) * 180.0 / Math.PI;
        return new LatLng(lng, lat);
    }
    public static void LatLngToTileXY(LatLng latlng, int zoom, out int tileX, out int tileY, out int pixelX, out int pixelY)
    {
        double size = Math.Pow(2, zoom);                                                    //zoom写死128
        double x = ((latlng.lng + 180) / 360) * size;
        double lat_rad = latlng.lat * Math.PI / 180;
        double y = (1 - Math.Log(Math.Tan(lat_rad) + 1 / Math.Cos(lat_rad)) / Math.PI) / 2;
        y = y * size;

        tileX = (int)x;
        tileY = (int)y;
        pixelX = (int)((x - tileX) * 256);//xy中心点，tilexy左下角点，单位长度为256个像素点，pixel是对应的
        pixelY = (int)((y - tileY) * 256);
    }

    /// <summary>
    /// 这个是自己加的，不再分格子和像素，而是直接转换为浮点数，用于设置transfrom位置
    /// </summary>
    /// <param name="latlng"></param>
    /// <param name="zoom"></param>
    /// <param name="doubleX"></param>
    /// <param name="doubleY"></param>
    public static void LatLngToDoubleXY(LatLng latlng, int zoom, out double doubleX, out double doubleY)
    {
        double size = Math.Pow(2, zoom);                                                    //zoom写死128
        double x = ((latlng.lng + 180) / 360) * size;
        double lat_rad = latlng.lat * Math.PI / 180;
        double y = (1 - Math.Log(Math.Tan(lat_rad) + 1 / Math.Cos(lat_rad)) / Math.PI) / 2;
        y = y * size;

        doubleX = x;
        doubleY = y;
    }

    public static void LatLngToFloatXY(LatLng latlng, int zoom, out float floatX, out float floatY)
    {
        double size = Math.Pow(2, zoom);                                                    //zoom写死128
        double x = ((latlng.lng + 180) / 360) * size;
        double lat_rad = latlng.lat * Math.PI / 180;
        double y = (1 - Math.Log(Math.Tan(lat_rad) + 1 / Math.Cos(lat_rad)) / Math.PI) / 2;
        y = y * size;

        floatX = (float)x;
        floatY = (float)y;
    }

    public static void DoubleXYToLatLng(double doubleX, double doubleY, int zoom, out double lng, out double lat)
    {
        double size = Math.Pow(2, zoom);  // zoom写死128
        double x = doubleX / size * 360 - 180;
        double y = doubleY / size;
        double lat_rad = Math.Atan(Math.Sinh(Math.PI * (1 - 2 * y)));
        lat = lat_rad * 180 / Math.PI;
        lng = x;
    }

    public static void FloatXYToLatLng(float floatX, float floatY, int zoom, out double lng, out double lat)
    {
        double size = Math.Pow(2, zoom);  // zoom写死128
        double x = floatX / size * 360 - 180;
        double y = floatY / size;
        double lat_rad = Math.Atan(Math.Sinh(Math.PI * (1 - 2 * y)));
        lat = lat_rad * 180 / Math.PI;
        lng = x;
    }
    

    /// <summary>
    /// 这是后来加的用来计算距离
    /// </summary>
    // 将GPS经纬度坐标转换为XY坐标 X+为东，Y+为北。
    public static void GpsToMeter(double lat, double lon, double refLat, double refLon, out double x, out double y)
    {
        double latRad = Math.PI * lat / 180.0;
        double lonRad = Math.PI * lon / 180.0;
        double refLatRad = Math.PI * refLat / 180.0;
        double refLonRad = Math.PI * refLon / 180.0;

        double sinLat = Math.Sin(latRad);
        double cosLat = Math.Cos(latRad);
        double refSinLat = Math.Sin(refLatRad);
        double refCosLat = Math.Cos(refLatRad);
        double cosDLon = Math.Cos(lonRad - refLonRad);

        double arg = Math.Max(-1.0, Math.Min(refSinLat * sinLat + refCosLat * cosLat * cosDLon, 1.0));
        double c = Math.Acos(arg);

        double k = (Math.Abs(c) > 0) ? (c / Math.Sin(c)) : 1.0;

        //注：源程序是x+北，y+东，这里微操了一下
        y = k * (refCosLat * sinLat - refSinLat * cosLat * cosDLon) * CONSTANTS_RADIUS_OF_EARTH;
        x = k * cosLat * Math.Sin(lonRad - refLonRad) * CONSTANTS_RADIUS_OF_EARTH;
    }

    // 将XY坐标转换为GPS经纬度坐标
    public static void MeterToGps(double x, double y, double refLat, double refLon, out double lat, out double lon)
    {
        //注：源程序是x+北，y+东，这里微操了一下
        double xRad = y / CONSTANTS_RADIUS_OF_EARTH;
        double yRad = x / CONSTANTS_RADIUS_OF_EARTH;
        double c = Math.Sqrt(xRad * xRad + yRad * yRad);

        double refLatRad = Math.PI * refLat / 180.0;
        double refLonRad = Math.PI * refLon / 180.0;
        double refSinLat = Math.Sin(refLatRad);
        double refCosLat = Math.Cos(refLatRad);

        double latRad, lonRad;
        if (Math.Abs(c) > 0)
        {
            double sinC = Math.Sin(c);
            double cosC = Math.Cos(c);
            latRad = Math.Asin(cosC * refSinLat + (xRad * sinC * refCosLat) / c);
            lonRad = refLonRad + Math.Atan2(yRad * sinC, c * refCosLat * cosC - xRad * refSinLat * sinC);
        }
        else
        {
            latRad = refLatRad;
            lonRad = refLonRad;
        }
        lat = 180.0 * latRad / Math.PI;
        lon = 180.0 * lonRad / Math.PI;
    }


}