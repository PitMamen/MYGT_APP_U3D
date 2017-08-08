using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameListProvider : MonoBehaviour
{

	private AndroidJavaObject objectStore;
	private AndroidJavaObject unityplayProvider;
	private AndroidJavaObject providerObject;
	private AndroidJavaObject vo;
	private string[] stringGamePublics;
	private string[] gameIDs;
	private int[] gameDownCount;
	private string[] gameDescinfo;
	private string[] gameName; 
	private string[] gameIcon;






	GameObject TitleList;
	GameObject GameList;


	AndroidJavaObject[] imageinfos;

	public GameObject[] pageImages;
	public	Image[] imageGames;
	public GameInfo[] pageImagesInfos;
	int pageImageCount = 5;
	int currentPage = 0;
	int currenPageImageCount;
	int imageSize;

	Image icon;
	Sprite sprite;

	// Use this for initialization
	void Start ()
	{
		objectStore = new AndroidJavaObject ("com.Szmygt.app.vr.utils.ObjectStore");
//				Debug.Log ("GameListProvider   "+"objectStore==  "+objectStore);
		unityplayProvider =	objectStore.CallStatic<AndroidJavaObject> ("get", "UnityPlayerActivity");
//				Debug.Log ("GameListProvider   "+"unityplayProvider==  "+unityplayProvider);
		providerObject = objectStore.CallStatic<AndroidJavaObject> ("get", "GameMainListProvider");
//				Debug.Log ("GameListProvider   "+"providerObject==  "+providerObject);
		vo = objectStore.CallStatic<AndroidJavaObject> ("get", "GameInfo");
//				Debug.Log ("GameListProvider   "+"providerObject==  "+vo);


		TitleList = GameObject.Find ("TitleList");
//		Debug.Log ("GameListProvider   "+"TitleList=="+TitleList);

		GameList = GameObject.Find ("GameList");
//		Debug.Log ("GameListProvider   "+"VideoList=="+GameList);

		icon = transform.Find ("GameList/Image").GetComponent<Image> ();
//		Debug.Log ("GameListProvider   "+"icon=="+icon);



		imageinfos = providerObject.Call<AndroidJavaObject[]> ("get", currentPage, pageImageCount);
//		Debug.Log ("VideoMainListProvider   " + "imageinfos.lenght==" + imageinfos.Length);


		stringGamePublics = providerObject.Call<string[]> ("getGamePublicity");
		gameName = providerObject.Call<string[]> ("getGameName");
		gameDescinfo = providerObject.Call<string[]> ("getGameDescinfo");
		gameDownCount = providerObject.Call<int[]> ("getGameDownCount");
		gameIcon = providerObject.Call<string[]> ("getGameIcon");




		pageImageCount = GameList.transform.childCount;

		pageImages = new GameObject[pageImageCount];

		pageImagesInfos = new GameInfo[pageImageCount];                //???????????


		bakebeef ();

		for (int i = 0; i < pageImageCount; i++) {
			GameObject pageImage = GameList.transform.GetChild (i).gameObject;
			pageImages [i] = pageImage;
			pageImage.SetActive (true);
			GameInfo info = pageImage.GetComponent<GameInfo> ();      //???????????
			pageImagesInfos [i] = info;                                 //???????????
		}



	}




	List<byte[]> list = new List<byte[]> ();

	void bakebeef ()
	{
//		AndroidJavaObject[] imageinfos = providerObject.Call<AndroidJavaObject[]> ("get", currentPage, pageImageCount);

		for (int i = 0; i < stringGamePublics.Length; i++) {
			byte[]	datas = vo.Call<byte[]> ("loadIcon1", stringGamePublics [i]);  //这里是调用Android端的 方法 将路径转为byte[]
//			Debug.Log ("GameDatas==" + datas.Length);  // 3  15355  15467   17849
			list.Add (datas);  //3
//			Debug.Log ("Gamelist==" + list.Count);  //3
		}

		for (int j = 0; j < imageinfos.Length; j++) {
			setIcon (list [j], imageGames [j]);
		}


	}





	public void setIcon (byte[] data, Image image)
	{
		Texture2D tex = new Texture2D (200, 200);
		tex.LoadImage (data);
		tex.filterMode = FilterMode.Trilinear;

		sprite = Sprite.Create (tex, new Rect (0, 0, tex.width, tex.height), new Vector2 (0.5f, 0.5f)/*, 100f*/); 
//		Debug.Log ("Icon width=" + tex.width + " height=" + tex.height);
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
//		Debug.Log ("imageinfos==" + imageinfos.Length); //4

		imageSize = providerObject.Call<int> ("getListSize");

//		Debug.Log ("imageSize==" + imageSize);   //4

		if (imageinfos.Length == 0 || imageSize == 0) {
			for (int i = 0; i < pageImageCount; i++) {
				GameObject pageImage = GameList.transform.GetChild (i).gameObject;
				pageImages [i] = pageImage;
				pageImage.SetActive (false);
				GameInfo info = pageImage.GetComponent<GameInfo> ();
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





	Image thumIcon;
	Text GameName;
	Text GameDescription;
	Text GameDownLoadCount;

	//点击一张图片 传入path  进入视频详情
	public void EnterPanoramic (int position)
	{
		//这里动态获取视频地址  将获取到的地址赋给 mediaPlayerCtrl.m_strFileName;



		//这里返回父物体的节点,查找整个子物体中的"VideoplayPanel"下的子物体的"VideoIcon",获取它的Image控件。(找到VideoPlayPanel下的子控件)
		thumIcon = this.transform.parent.parent.transform.Find ("GameDetailsPanel").gameObject.transform.Find ("GameIcon").transform.GetComponent<Image> ();
//		Debug.Log ("EnterPanoramic   "+"thumIcon==" + thumIcon);
		GameName = this.transform.parent.parent.transform.Find ("GameDetailsPanel").gameObject.transform.Find ("gameName").transform.GetComponent<Text> ();	
//		Debug.Log ("EnterPanoramic   "+"GameName==" + GameName);
		GameDescription = this.transform.parent.parent.transform.Find ("GameDetailsPanel").gameObject.transform.Find ("gameDeducation").transform.GetComponent<Text> ();
//		Debug.Log ("EnterPanoramic   "+"GameDescription==" + GameDescription);
		GameDownLoadCount = this.transform.parent.parent.transform.Find ("GameDetailsPanel").gameObject.transform.Find ("downCount").transform.GetComponent<Text> ();
//		Debug.Log ("EnterPanoramic   "+"GameDownLoadCount==" + GameDownLoadCount);

		gameName = providerObject.Call<string[]> ("getGameName");
		Debug.Log ("GameDetail"+"gameName==" + gameName.Length);
		gameDescinfo = providerObject.Call<string[]> ("getGameDescinfo");
		Debug.Log ("GameDetail"+"gameDescinfo==" + gameDescinfo.Length);

		gameDownCount = providerObject.Call<int[]> ("getGameDownCount");
		Debug.Log ("GameDetail"+"gameDownCount==" + gameDownCount.Length);

		gameIcon = providerObject.Call<string[]> ("getGameIcon");
		Debug.Log ("GameDetail"+"gameIcon==" + gameIcon.Length);

		if (gameName != null && gameIcon != null && gameDescinfo != null && gameDownCount!=null) {

//			setVideoNameAndDescriptionAndLoadCount (gameName[position],gameDescinfo[position]);
			showImageByPath (gameIcon [position]);


		}
		PanelSwitcher.instance.showGameDetailsPanel(); 


	}


	void showImageByPath (string ImagePath)
	{
		byte[]	byteThumble = vo.Call<byte[]> ("loadIcon1", ImagePath);
		Debug.Log ("gameByteIcon==" + byteThumble.Length);  //17849
		if (byteThumble.Length > 0) {
			setIcon (byteThumble, thumIcon);
		}

	}

	public Text GameNameText;
	public Text GameDescriptionText;
//	public Text GameDownLoadCountText;


	//设置 视频详情页的 视频名称 和 描述
	void setVideoNameAndDescriptionAndLoadCount (string gamename, string gameDescinfo )
	{

		GameNameText.text = gamename;
		GameDescriptionText.text = gameDescinfo;
//		GameDownLoadCountText.text = count;

	}










	
	// Update is called once per frame
	void Update ()
	{

		refresh ();
	
	}
}
