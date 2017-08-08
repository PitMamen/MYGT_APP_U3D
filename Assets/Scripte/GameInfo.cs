using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameInfo : MonoBehaviour {

	Image icon;
	Image thumIcon;

	Sprite sprite;
	GameObject GameList;

	// Use this for initialization
	void Start () {
		GameList = GameObject.Find ("GameList");
		icon = transform.Find ("GameList/Image").GetComponent<Image> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}








	Sprite DefaultSprite;
	public void setIcon (byte[] data,Image image)
	{
		Texture2D tex = new Texture2D (2, 2);
		tex.LoadImage (data);

		tex.filterMode = FilterMode.Trilinear;

		sprite = Sprite.Create (tex, new Rect (0, 0, tex.width, tex.height), new Vector2 (0.5f, 0.5f), 100f); 
		Debug.Log ("Icon width=" + tex.width + " height=" + tex.height);
		image.sprite = sprite;
	}


	public void setIcon (byte[] data)
	{
		Texture2D tex = new Texture2D (2, 2);
		tex.LoadImage (data);

		tex.filterMode = FilterMode.Trilinear;

		sprite = Sprite.Create (tex, new Rect (0, 0, tex.width, tex.height), new Vector2 (0.5f, 0.5f), 100f); 
		Debug.Log ("Icon width=" + tex.width + " height=" + tex.height);
		icon.sprite = sprite;
	}


	public bool isIconSetted() {
		return sprite != null;
	}

	public void changTopage(){
		sprite = null;
		icon.sprite = DefaultSprite;

	}

	public void updateAppInfo() {

	}
}
