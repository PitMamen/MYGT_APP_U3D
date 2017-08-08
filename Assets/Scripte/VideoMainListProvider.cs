using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class VideoMainListProvider : MonoBehaviour {

	private AndroidJavaObject objectStore;
	private AndroidJavaObject unityplayProvider;
	private AndroidJavaObject providerObject;
	private AndroidJavaObject vo;

	GameObject TitleList;
	GameObject VideoList;

	//缩略图
	private string[] stringThumnails;
	//视频名称
	private string[] videoNames;
	//视频描述
	private string[] videoDescriptions;



	public	Image[] imagesVideo;



	public GameObject[] pageImages;
	public VideoInfo[] pageImagesInfos;
	int pageImageCount = 7;
	int currentPage = 0;
	int currenPageImageCount;
	int imageSize;
	Sprite sprite;
	Image icon;

	// Use this for initialization
	void Start () {

		objectStore = new AndroidJavaObject ("com.Szmygt.app.vr.utils.ObjectStore");
//		Debug.Log ("VideoMainListProvider   "+"objectStore==  "+objectStore);
		unityplayProvider =	objectStore.CallStatic<AndroidJavaObject> ("get", "UnityPlayerActivity");
//		Debug.Log ("VideoMainListProvider   "+"unityplayProvider==  "+unityplayProvider);
		providerObject = objectStore.CallStatic<AndroidJavaObject> ("get", "VideoMainListProvider");
//		Debug.Log ("VideoMainListProvider   "+"providerObject==  "+providerObject);
	
		vo = objectStore.CallStatic<AndroidJavaObject> ("get", "vo");
//		Debug.Log ("vo===" + vo);


		providerObject.Call ("testMainVideo",ApiContent.VIDEO_TYPE_VR);



		TitleList = GameObject.Find ("TitleList");
//		Debug.Log ("VideoMainListProvider   "+"TitleList=="+TitleList);

		VideoList = GameObject.Find ("VideoList");
//		Debug.Log ("VideoMainListProvider   "+"VideoList=="+VideoList);

		icon = transform.Find ("VideoList/Image").GetComponent<Image> ();
		Debug.Log ("VideoMainListProvider   "+"icon=="+icon);

	
		stringThumnails = providerObject.Call<string[]> ("getStringArray");
		Debug.Log ("VideoMainListProvider   "+stringThumnails[0]+"  "+stringThumnails[1]+"  "+stringThumnails[2]);
		videoNames = providerObject.Call<string[]> ("getVideoName");
		Debug.Log ("VideoMainListProvider   "+videoNames[0]+"  "+videoNames[1]+"  "+videoNames[2]);
		videoDescriptions = providerObject.Call<string[]> ("getDescription");
		Debug.Log ("VideoMainListProvider   "+videoDescriptions[0]+"  "+videoDescriptions[1]+"  "+videoDescriptions[2]);

		AndroidJavaObject[] imageinfos = providerObject.Call<AndroidJavaObject[]> ("get", currentPage, pageImageCount);
		Debug.Log ("VideoMainListProvider   "+imageinfos.Length);


		pageImageCount = VideoList.transform.childCount;

		pageImages = new GameObject[pageImageCount];

		pageImagesInfos = new VideoInfo[pageImageCount];                //???????????


		bakebeef ();


		for (int i = 0; i < pageImageCount; i++) {
			GameObject pageImage = VideoList.transform.GetChild (i).gameObject;
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

		for (int i = 0; i < stringThumnails.Length; i++) {
			byte[]	datas = vo.Call<byte[]> ("loadIcon1", stringThumnails [i]);  //这里是调用Android端的 方法 将路径转为byte[]
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
				GameObject pageImage = VideoList.transform.GetChild (i).gameObject;
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









	private string URL = "http://123.207.115.76:10199//video/20170427/1493274189317046092.mp4";
	Image thumIcon;

	//点击一张图片 传入path  进入视频详情
	public void EnterPanoramic (int position)
	{
		//这里动态获取视频地址  将获取到的地址赋给 mediaPlayerCtrl.m_strFileName;



		//这里返回父物体的节点,查找整个子物体中的"VideoplayPanel"下的子物体的"VideoIcon",获取它的Image控件。(找到VideoPlayPanel下的子控件)
		thumIcon = this.transform.parent.parent.transform.Find ("VideoPlayPanel").gameObject.transform.Find ("VideoIcon").transform.GetComponent<Image> ();
		VideoName = this.transform.parent.parent.transform.Find ("VideoPlayPanel").gameObject.transform.Find ("VideoName").transform.GetComponent<Text> ();
		VideoDescription = this.transform.parent.parent.transform.Find ("VideoPlayPanel").gameObject.transform.Find ("VideoDescription").transform.GetComponent<Text> ();
		Debug.Log ("thumIcon==" + thumIcon);

		string[] path = providerObject.Call<string[]> ("getStringArray");   //获取缩略图路径
		Debug.Log("EnterPanoramic  "+path.Length);

		videoNames = providerObject.Call<string[]> ("getVideoName");        //获取视频名称
		Debug.Log("EnterPanoramic  "+videoNames.Length);

		videoDescriptions = providerObject.Call<string[]> ("getDescription");  //获取视频描述
		Debug.Log("EnterPanoramic  "+videoDescriptions.Length);

		if (path != null && videoNames != null && videoDescriptions != null) {

			setVideoNameAndDescription ("名称： "+videoNames[position],"视频描述： "+videoDescriptions[position]);
			showImageByPath (path [position]);


		}
		PanelSwitcher.instance.showVideoPlayPanel (); 


	}


	void showImageByPath (string ImagePath)
	{
		byte[]	byteThumble = vo.Call<byte[]> ("loadIcon1", ImagePath);
		Debug.Log ("byteThumble==" + byteThumble.Length);  //17849
		if (byteThumble.Length > 0) {
			setIcon (byteThumble, thumIcon);
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


	// Update is called once per frame
	void Update () {
		refresh ();
	}




	public	Image imageVR;
	public  Image image3D;
	public  Image image2D;


	//请求VR接口
	public void RequestVRData()
	{
		imageVR.color = Color.blue;
		image3D.color = Color.gray;
		image2D.color = Color.gray;
		providerObject.Call ("testMainVideo",ApiContent.VIDEO_TYPE_VR);
		for (int i = 0; i < pageImagesInfos.Length; i++)
		{

			pageImagesInfos[i].changTopage();
		}
		refresh ();
	}




	//请求2D接口

	public void Request2DData()
	{
		image2D.color = Color.blue;
		imageVR.color = Color.gray;
		image3D.color = Color.gray;

		providerObject.Call ("testMainVideo",ApiContent.VIDEO_TYPE_2D);
		for (int i = 0; i < pageImagesInfos.Length; i++)
		{

			pageImagesInfos[i].changTopage();
		}
		refresh ();
	}


	//请求3D接口

	public void Request3DData()
	{
		image3D.color = Color.blue;
		image2D.color = Color.gray;
		imageVR.color = Color.gray;


		providerObject.Call ("testMainVideo",ApiContent.VIDEO_TYPE_3D);
		for (int i = 0; i < pageImagesInfos.Length; i++)
		{

			pageImagesInfos[i].changTopage();
		}
		refresh ();
	}



	//请求VR电影接口

	public void RequestVRFilmData()
	{
		providerObject.Call ("testMainVideo",ApiContent.VIDEO_TYPE_VRFILM);
		for (int i = 0; i < pageImagesInfos.Length; i++)
		{

			pageImagesInfos[i].changTopage();
		}
		refresh ();
	}


	//请求VRm美女接口

	public void RequestVRPrifeData()
	{
		providerObject.Call ("testMainVideo",ApiContent.VIDEO_TYPE_VRPERI);
		for (int i = 0; i < pageImagesInfos.Length; i++)
		{

			pageImagesInfos[i].changTopage();
		}
		refresh ();
	}


}
