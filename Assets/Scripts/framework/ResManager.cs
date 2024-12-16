using UnityEngine;

public class ResManager : MonoBehaviour {

	//加载预设
	public static GameObject LoadPrefab(string path){
		return Resources.Load<GameObject>(path);
	}
	
	//加载图片精灵
	public static Sprite LoadSprite(string path){
		return Resources.Load<Sprite>(path);
	}
}
