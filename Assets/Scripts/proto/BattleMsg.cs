//坦克信息
//finished 只需要同步这些东西

[System.Serializable]
public class TankInfo{
    public string id = "";	//玩家id
    public int camp = 0;	//阵营
    public int hp = 0;		//生命值

    public LatLng latLng = new(0,0);
    public float ez = 0;
	
}