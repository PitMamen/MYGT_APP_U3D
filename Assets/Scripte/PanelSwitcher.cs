using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PanelSwitcher : MonoBehaviour
{
	GameObject Canvas;
	public GameObject HomePanel;
	public GameObject VideoLocalPanel;
	public GameObject MoreVideoPanel;
	public GameObject VRMoviePanel;
	public GameObject GameHomePanel;
	public GameObject GameTypePanel;
	public GameObject GameSoftPanel;
	public GameObject VideoPlayPanel;
	public GameObject GameDetailsPanel;
	public GameObject GamePanel;
	public GameObject PlayPanel;
	public GameObject FilmeOrPrifePanel;

	AndroidJavaObject providerObject;
	AndroidJavaObject objectStore;
	AndroidJavaObject VideoListProvider;


	public static List<GameObject> list;

	public static PanelSwitcher instance;

	private MediaPlayerCtrl mediaPlayerCtrl;  

	void Awake()
	{
		instance = this;
	}


	void Start ()
	{
//		objectStore = new AndroidJavaObject ("com.Szmygt.app.vr.utils.ObjectStore");
//		providerObject = objectStore.CallStatic<AndroidJavaObject> ("get", "UnityPlayerActivity");
//		VideoListProvider = objectStore.CallStatic<AndroidJavaObject> ("get", "VideoListProvider");


		showHomePanel ();
//		showPlayPanel();
		//	2、在Start方法中将首页压入栈中
		list = new List<GameObject>(5);
		//将页面压入堆栈中

		list.Add(HomePanel);
	}



	public void initEasyMovieTextureCtrl()
	{
		mediaPlayerCtrl = this.transform.GetComponent<MediaPlayerCtrl> ();
		mediaPlayerCtrl.m_bAutoPlay = false; 
	}





	//	4、由一个页面跳转到另一个页面  将页面名称压入栈中
	public static void androidNext(GameObject NextGO)
	{
		list.Add(NextGO);

		list[list.Count - 2].gameObject.SetActive(false);
		list[list.Count - 1].gameObject.SetActive(true);
	}
	//	5、安卓手机中在一个页面点击返回按钮时  将此刻的页面名称弹出堆栈
	public static void androidBack()
	{
		if (list.Count > 1)
		{
			list[list.Count -1].gameObject.SetActive(false); //当前界面返回，关系该界面
			list[list.Count - 2].gameObject.SetActive(true); //用户上一步操作界面，显示
			list.RemoveAt(list.Count - 1); //从数组中移除上一步操作
		}
		else return;
		//{
		//    //如果是在首页 按 返回键 则重新加载该场景，可不用
		//    Application.LoadLevel(0); 
		//    list.RemoveAt(0);
		//}
	}






	void OnGUI ()
	{
		if (Event.current.isKey) {
			Debug.Log ("keyevent " + Event.current.ToString ());
		}

	}



	void Update ()
	{

	}


	public void FlashVideo ()
	{
		Debug.Log ("is Flash！！");
	}


	public void ExitApp ()
	{
		
		if(HomePanel.activeSelf==true){
			Application.Quit ();
		}
		androidBack();
	}





	//显示主页
	public void  showHomePanel ()
	{
		HomePanel.SetActive (true);
		VideoPlayPanel.SetActive (false);
		GameDetailsPanel.SetActive (false);
		GameHomePanel.SetActive (false);
		VRMoviePanel.SetActive (false);
		VideoLocalPanel.SetActive (false);
		GameTypePanel.SetActive (false);
		MoreVideoPanel.SetActive (false);
		GameSoftPanel.SetActive (false);
		GamePanel.SetActive (false);
		PlayPanel.SetActive (false);
		FilmeOrPrifePanel.SetActive (false);
	}


	public void showPlayPanel()
	{
//		PlayPanel.SetActive (true);
		GamePanel.SetActive (false);
		GameDetailsPanel.SetActive (false);
		HomePanel.SetActive (false);
		VideoLocalPanel.SetActive (false);
		MoreVideoPanel.SetActive (false);
		VideoPlayPanel.SetActive (false);
		VRMoviePanel.SetActive (false);
		GameHomePanel.SetActive (false);
		GameTypePanel.SetActive (false);
		GameSoftPanel.SetActive (false);
		FilmeOrPrifePanel.SetActive (false);
	}




	//显示本地视频  VideoLocalPanel
	public void showVideoLocalPanel ()
	{
		androidNext(VideoLocalPanel);
		VideoLocalPanel.SetActive (true);
		HomePanel.SetActive (false);
		GameTypePanel.SetActive (false);
		MoreVideoPanel.SetActive (false);
		VRMoviePanel.SetActive (false);
		GameHomePanel.SetActive (false);
		GameSoftPanel.SetActive (false);
		VideoPlayPanel.SetActive (false);
		GameDetailsPanel.SetActive (false);
		GamePanel.SetActive (false);
		PlayPanel.SetActive (false);
		FilmeOrPrifePanel.SetActive (false);
	}

	//显示更多视频   VideoMorePanel
	public void showVideoMorePanel ()
	{
		androidNext(MoreVideoPanel);
		MoreVideoPanel.SetActive (true);
		HomePanel.SetActive (false);
		GameHomePanel.SetActive (false);
		VRMoviePanel.SetActive (false);
		VideoLocalPanel.SetActive (false);
		GameTypePanel.SetActive (false);  
		GameSoftPanel.SetActive (false);
		VideoPlayPanel.SetActive (false);
		GameDetailsPanel.SetActive (false);
		GamePanel.SetActive (false);
		PlayPanel.SetActive (false);
		FilmeOrPrifePanel.SetActive (false);
	}


	//显示电影
	public void showVRMoviePanel ()
	{
		androidNext(VRMoviePanel);
		VRMoviePanel.SetActive (true);
		HomePanel.SetActive (false);
		GameHomePanel.SetActive (false);
		MoreVideoPanel.SetActive (false);
		VideoLocalPanel.SetActive (false);
		VideoPlayPanel.SetActive (false);
		GameTypePanel.SetActive (false);
		GameSoftPanel.SetActive (false);
		GameDetailsPanel.SetActive (false);
		GamePanel.SetActive (false);
		PlayPanel.SetActive (false);
		FilmeOrPrifePanel.SetActive (false);
	}





	// 显示游戏主页
	public void showGameHomePanel ()
	{
		androidNext(GameHomePanel);
		GameHomePanel.SetActive (true);
		HomePanel.SetActive (false);
		VideoPlayPanel.SetActive (false);
		VideoLocalPanel.SetActive (false);
		VRMoviePanel.SetActive (false);
		MoreVideoPanel.SetActive (false);
		GameTypePanel.SetActive (false);
		GameSoftPanel.SetActive (false);
		GameDetailsPanel.SetActive (false);
		GamePanel.SetActive (false);
		PlayPanel.SetActive (false);
		FilmeOrPrifePanel.SetActive (false);
	}


	//显示游戏类型界面

	public void showGameTypePanel ()
	{
		androidNext(GameTypePanel);
		GameTypePanel.SetActive (true);
		HomePanel.SetActive (false);
		VideoPlayPanel.SetActive (false);
		GameHomePanel.SetActive (false);
		VideoLocalPanel.SetActive (false);
		MoreVideoPanel.SetActive (false);
		VRMoviePanel.SetActive (false);
		GameSoftPanel.SetActive (false);
		GameDetailsPanel.SetActive (false);
		GamePanel.SetActive (false);
		PlayPanel.SetActive (false);
		FilmeOrPrifePanel.SetActive (false);

	}


	//显示软件界面
	public void showGameSoftPanel ()
	{
		androidNext(GameSoftPanel);
		GameSoftPanel.SetActive (true);
		HomePanel.SetActive (false);
		GameTypePanel.SetActive (false);
		GameHomePanel.SetActive (false);
		VideoLocalPanel.SetActive (false);
		MoreVideoPanel.SetActive (false);
		VRMoviePanel.SetActive (false);
		GameDetailsPanel.SetActive (false);
		VideoPlayPanel.SetActive (false);
		GamePanel.SetActive (false);
		PlayPanel.SetActive (false);
		FilmeOrPrifePanel.SetActive (false);
	}


	//显示视频播放界面
	public void showVideoPlayPanel()
	{
		androidNext(VideoPlayPanel);
		VideoPlayPanel.SetActive (true);
		VRMoviePanel.SetActive (false);
		GameTypePanel.SetActive (false);
		GameHomePanel.SetActive (false);
		VideoLocalPanel.SetActive (false);
		MoreVideoPanel.SetActive (false);
		GameSoftPanel.SetActive (false);
		HomePanel.SetActive (false);
		GameDetailsPanel.SetActive (false);
		GamePanel.SetActive (false);
		PlayPanel.SetActive (false);
		FilmeOrPrifePanel.SetActive (false);
	}


	//显示游戏下载界面
	public void showGameDetailsPanel()
	{
		androidNext(GameDetailsPanel);
		GameDetailsPanel.SetActive (true);
		HomePanel.SetActive (false);
		VideoLocalPanel.SetActive (false);
		MoreVideoPanel.SetActive (false);
		VideoPlayPanel.SetActive (false);
		VRMoviePanel.SetActive (false);
		GameHomePanel.SetActive (false);
		GameTypePanel.SetActive (false);
		GameSoftPanel.SetActive (false);
		GamePanel.SetActive (false);
		PlayPanel.SetActive (false);
		FilmeOrPrifePanel.SetActive (false);
	}

	//显示游戏界面
	public void showGamePanel()
	{
		androidNext(GamePanel);
		GamePanel.SetActive (true);
		GameDetailsPanel.SetActive (false);
		HomePanel.SetActive (false);
		VideoLocalPanel.SetActive (false);
		MoreVideoPanel.SetActive (false);
		VideoPlayPanel.SetActive (false);
		VRMoviePanel.SetActive (false);
		GameHomePanel.SetActive (false);
		GameTypePanel.SetActive (false);
		GameSoftPanel.SetActive (false);
		PlayPanel.SetActive (false);
		FilmeOrPrifePanel.SetActive (false);
	}





	public void GotoPlay()
	{

		androidNext(PlayPanel);
		PlayPanel.SetActive (true);
//		mediaPlayerCtrl.Play ();
		GamePanel.SetActive (false);
		GameDetailsPanel.SetActive (false);
		HomePanel.SetActive (false);
		VideoLocalPanel.SetActive (false);
		MoreVideoPanel.SetActive (false);
		VideoPlayPanel.SetActive (false);
		VRMoviePanel.SetActive (false);
		GameHomePanel.SetActive (false);
		GameTypePanel.SetActive (false);
		GameSoftPanel.SetActive (false);
		FilmeOrPrifePanel.SetActive (false);
	}



	public void show3DFlimeOr3DPrife()
	{

		androidNext(FilmeOrPrifePanel);
		FilmeOrPrifePanel.SetActive (true);
		GamePanel.SetActive (false);
		GameDetailsPanel.SetActive (false);
		HomePanel.SetActive (false);
		VideoLocalPanel.SetActive (false);
		MoreVideoPanel.SetActive (false);
		VideoPlayPanel.SetActive (false);
		VRMoviePanel.SetActive (false);
		GameHomePanel.SetActive (false);
		GameTypePanel.SetActive (false);
		GameSoftPanel.SetActive (false);
		PlayPanel.SetActive (false);

	}



}
