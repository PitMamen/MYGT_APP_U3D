using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VideoInfo : MonoBehaviour {

	Image icon;
	Image thumIcon;
	Text total_time_lable;
	Text current_time_lable;

	string video_total_time;
	string video_current_time;

	Sprite sprite;
	GameObject GameList;
	GameObject HomeList;

	 VideoInfo instance;

//	void Awake()
//	{
//		instance = this;
//	}


	// Use this for initialization
	void Start () {
		GameList = GameObject.Find ("GameList");
//		HomeList = GameObject.Find ("HomeList");
		icon = transform.Find ("VideoList/Image").GetComponent<Image> ();
//		thumIcon = transform.Find ("VideoIcon").gameObject.GetComponent<Image> ();
//
//		current_time_lable = transform.Find("text_current_time").gameObject.GetComponent<Text> ();
//		total_time_lable = transform.Find("text_total_time").gameObject.GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	//设置视频的  总时间  与  当前时间
	public void setVideotime(string video_total_time,string video_current_time)
	{

		this.video_total_time = video_total_time;
		this.video_current_time = video_current_time;

		current_time_lable.text = video_current_time;
		total_time_lable.text = video_total_time;
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
