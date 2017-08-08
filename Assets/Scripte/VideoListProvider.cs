using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Collections;

public class VideoListProvider : MonoBehaviour
{

	private AndroidJavaObject objectStore;
	private AndroidJavaObject providerObject;
	private AndroidJavaObject unityplayProvider;
	private AndroidJavaObject vo;

	// byte[] 数据集
	private byte[] videoListData;

	private List<byte[]> listData = new List<byte[]> ();

	// string[] 数据集
	private  string[] videoListArrayData;

	//视频名称
	private string[] videoNames;

	//视频描述

	private string[] videoDescriptions;


	int pageImageCount = 7;
	int currentPage = 0;
	int currenPageImageCount;
	int imageSize;


	public GameObject[] pageImages;
	public VideoInfo[] pageImagesInfos;
	public Image[] imagesVideo;

	GameObject videolist;
	GameObject Normal;
	GameObject VideoIcon;
	private bool isIconReady;

	Sprite sprite;
	Image icon;





	void Start ()
	{

		videolist = GameObject.Find ("VideoList");

		//影视详情panel
		Normal = GameObject.Find ("VideoPlayPanel");
		VideoIcon = GameObject.Find ("VideoIcon");

		objectStore = new AndroidJavaObject ("com.Szmygt.app.vr.utils.ObjectStore");

		icon = transform.Find ("icon").gameObject.GetComponent<Image> ();

		//		initEasyMovieTextureCtrl ();


		//还有一种找法  这种是返回最高级变换(Canvas物体)
		//"VideoPlayPanel/VideoIcon" 根据路径进行查找物体,适合越级查找物体或者物体名字有重复的情况(如果是查找本panel中 去掉root)
		//		thumIcon = this.transform.root.Find("VideoPlayPanel/VideoIcon").transform.GetComponent<Image>();


		providerObject = objectStore.CallStatic<AndroidJavaObject> ("get", "VideoListProvider");

		vo = objectStore.CallStatic<AndroidJavaObject> ("get", "vo");
		Debug.Log ("vo===" + vo);

		unityplayProvider =	objectStore.CallStatic<AndroidJavaObject> ("get", "UnityPlayerActivity");


		videoListArrayData = providerObject.Call<string[]> ("getStringArray");
		Debug.Log ("videoListArrayData===" + videoListArrayData [0] + "    " + videoListArrayData [1] + "     " + videoListArrayData [2]);

	


		bakebeef ();


		pageImageCount = videolist.transform.childCount;

		pageImages = new GameObject[pageImageCount];

		pageImagesInfos = new VideoInfo[pageImageCount];                //???????????



		for (int i = 0; i < pageImageCount; i++) {
			GameObject pageImage = videolist.transform.GetChild (i).gameObject;
			pageImages [i] = pageImage;
			pageImage.SetActive (true);
			VideoInfo info = pageImage.GetComponent<VideoInfo> ();      //???????????
			pageImagesInfos [i] = info;                                 //???????????
		}

	}









	List<byte[]> list = new List<byte[]> ();

	void bakebeef ()
	{
		AndroidJavaObject[] imageinfos = providerObject.Call<AndroidJavaObject[]> ("get", currentPage, pageImageCount);

		for (int i = 0; i < videoListArrayData.Length; i++) {
			byte[]	datas = vo.Call<byte[]> ("loadIcon1", videoListArrayData [i]);  //这里是调用Android端的 方法 将路径转为byte[]
			Debug.Log ("DATAS==" + datas.Length);  // 3  15355  15467   17849
			list.Add (datas);  //3
			Debug.Log ("list==" + list.Count);  //3
		}

		for (int j = 0; j < imageinfos.Length; j++) {
			setIcon (list [j], imagesVideo [j]);
		}


	}


	public void setIcon (byte[] data, Image image)
	{
		Texture2D tex = new Texture2D (200, 200);
		tex.LoadImage (data);
		tex.filterMode = FilterMode.Trilinear;

		sprite = Sprite.Create (tex, new Rect (0, 0, tex.width, tex.height), new Vector2 (0.5f, 0.5f)/*, 100f*/); 
		Debug.Log ("Icon width=" + tex.width + " height=" + tex.height);
		image.sprite = sprite;
	}



	void bakeBeef (AndroidJavaObject info, VideoInfo image)
	{


		isIconReady = info.Get<bool> ("isIconReady");
		Debug.Log ("isIconReady=    " + isIconReady);


		if (isIconReady == true && image.isIconSetted () == false) {
			byte[] data = info.Get<byte[]> ("iconBytes");
			Debug.Log ("data===" + data.Length);


			if (data == null) {
				return;
			}

			image.setIcon (data);




		}

	}


	void bakeImageInfo (AndroidJavaObject[] imageInfos)
	{


		int i;
		for (i = 0; i < imageInfos.Length; i++) {
			pageImages [i].SetActive (true);

		}


		for (; i < pageImageCount; i++) {
			pageImages [i].SetActive (false);
		}
	}



	void refresh ()
	{

		AndroidJavaObject[] imageinfos = providerObject.Call<AndroidJavaObject[]> ("get", currentPage, pageImageCount);
		Debug.Log ("imageinfos==" + imageinfos.Length); //3

		imageSize = providerObject.Call<int> ("getListSize");

		Debug.Log ("imageSize==" + imageSize);   //3


		if (imageinfos.Length == 0 || imageSize == 0) {
			for (int i = 0; i < pageImageCount; i++) {
				GameObject pageImage = videolist.transform.GetChild (i).gameObject;
				pageImages [i] = pageImage;
				pageImage.SetActive (false);
				VideoInfo info = pageImage.GetComponent<VideoInfo> ();
				pageImagesInfos [i] = info;

			}
		}


		if (imageinfos != null && imageinfos.Length > 0) {
			bakeImageInfo (imageinfos);
		}

		for (int i = 0; i < imageinfos.Length; i++) {
			imageinfos [i].Dispose ();
		}

	}



	void Update ()
	{
		refresh ();

	}

	public bool isIconSetted ()
	{
		return sprite != null;
	}

	Sprite DefaultSprite;

	public void changTopage ()
	{
		sprite = null;
		icon.sprite = DefaultSprite;
	
	}

	private string URL = "http://123.207.115.76:10199//video/20170427/1493274189317046092.mp4";
	Image thumIcon;

	//点击一张图片 传入path  进入视频详情
	public void EnterPanoramic (int position)
	{
		//这里动态获取视频地址  将获取到的地址赋给 mediaPlayerCtrl.m_strFileName;

//		providerObject.Call ("testgetVideoById",);

		//这里返回父物体的节点,查找整个子物体中的"VideoplayPanel"下的子物体的"VideoIcon",获取它的Image控件。(找到VideoPlayPanel下的子控件)
		thumIcon = this.transform.parent.parent.transform.Find ("VideoPlayPanel").gameObject.transform.Find ("VideoIcon").transform.GetComponent<Image> ();
		VideoName = this.transform.parent.parent.transform.Find ("VideoPlayPanel").gameObject.transform.Find ("VideoName").transform.GetComponent<Text> ();
		VideoDescription = this.transform.parent.parent.transform.Find ("VideoPlayPanel").gameObject.transform.Find ("VideoDescription").transform.GetComponent<Text> ();
		Debug.Log ("thumIcon==" + thumIcon);

		string[] path = providerObject.Call<string[]> ("getStringArray");   //获取缩略图路径

		videoNames = providerObject.Call<string[]> ("getVideoName");        //获取视频名称

		videoDescriptions = providerObject.Call<string[]> ("getDescription");  //获取视频描述

		string[] videoID = providerObject.Call<string[]> ("getVideoID");            //获取视频ID
		Debug.Log ("videoID==  " + videoID.Length);

		if (path != null && videoNames != null && videoDescriptions != null) {

			setVideoNameAndDescription ("名称： "+videoNames [position], "视频描述： "+videoDescriptions [position]);
			showImageByPath (path [position]);

			providerObject.Call ("testgetVideoById", videoID [position]);            //根据不同的ID 请求单个视频地址的接口

			string videoUrl = providerObject.Call<string> ("getVideoUrl");
			Debug.Log ("videoUrl3==="+videoUrl);

			if (videoUrl != null) {
				MediaPlayerCtrl mediaPlayer = new MediaPlayerCtrl ();
				mediaPlayer.m_strFileName = videoUrl;
				mediaPlayer.Play ();
			}
		}
		PanelSwitcher.instance.showVideoPlayPanel (); 


	}


	void showImageByPath (string ImagePath)
	{
		byte[]	bytesJPG = vo.Call<byte[]> ("loadIcon1", ImagePath);
		Debug.Log ("bytesBMP==" + bytesJPG.Length);  //17849
		if (bytesJPG.Length > 0) {
			setIcon (bytesJPG, thumIcon);
		}

	}

	string videoname;
	string videodescription;
	public Text VideoName;
	public Text VideoDescription;

	//设置 视频详情页的 视频名称 和 描述
	void setVideoNameAndDescription (string videoname, string videoDescription)
	{
		this.videoname = videoname;
		this.videodescription = videoDescription;

		VideoName.text = videoname;
		VideoDescription.text = videoDescription;


	}





}










































