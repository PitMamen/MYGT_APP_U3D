using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIMovieEasyMovieTexture : MonoBehaviour {

	public Text totalTime;  
	public Text current_time;    
	public Slider movieSlider;  
	public Material renderMaterial;  
	public int movieSpeed = 1000;  

	private MediaPlayerCtrl mediaPlayerCtrl;  
	private RawImage movieRawImage;  
	private Vector2 reducteOffsetMax;  
	private bool isFinish = false;  
	private bool isPlay = false;  
	private bool isCtrlMovie = false;  
	private int fastbackTemp = 0;  
	private int totaltime = 0;  
	// Use this for initialization  
	void Start () {  
		Init();  
		mediaPlayerCtrl.OnEnd += OnEnd;  
	}  
	/// <summary>  
	/// 初始化函数  
	/// </summary>  
	public void Init()  
	{  
		mediaPlayerCtrl = this.transform.GetComponent<MediaPlayerCtrl>();  
//		movieRawImage = this.transform.GetComponent<RawImage>();  
		mediaPlayerCtrl.m_bAutoPlay = false;  
//		if (movieRawImage.material == null)  
//			movieRawImage.material = renderMaterial;  
//		reducteOffsetMax = movieRawImage.rectTransform.offsetMax;  
	}  

	// Update is called once per frame  
	void Update ()  
	{  
		UpdateMovieSlider();  
		UpdateMovieTime();  
		UpdateTotalTime();  
	}  
	/// <summary>  
	/// 更新视频进度条  
	/// </summary>  
	public void UpdateMovieSlider()  
	{  
		if (mediaPlayerCtrl.GetSeekPosition() != 0 && !isCtrlMovie)  
		{  
			float slider = (float) mediaPlayerCtrl.GetSeekPosition()/(float) mediaPlayerCtrl.GetDuration();   //通过进度条的总值 计算当前进度条的值
			movieSlider.value = slider;  
		}  
	}  
	/// <summary>  
	/// 更新视频总时间  
	/// </summary>  
	public void UpdateTotalTime()  
	{  
		if (this.totalTime.text != null && isPlay)  
		{  
			if (totaltime != mediaPlayerCtrl.GetDuration() / 1000)  
			{  
				totaltime = mediaPlayerCtrl.GetDuration() / 1000;  
				int min = totaltime / 60;  
				int seconds = totaltime - min * 60;  
				this.totalTime.text = " / " + min + ":" + seconds;  
			}  
		}   
	}  
	/// <summary>  
	/// 更新视频时间  (视频当前时间)
	/// </summary>  
	public void UpdateMovieTime()  
	{  
		if (this.current_time.text != null && isPlay)  
		{  
			int movieTime = mediaPlayerCtrl.GetSeekPosition() / 1000;  
			int min = movieTime / 60;  
			int seconds = movieTime - min * 60;  
			this.current_time.text = min + ":" + seconds;  
		}  
	}  
	/// <summary>  
	/// 视频播放  
	/// </summary>  
	public void MoviePlay()  
	{  
		mediaPlayerCtrl.Play();  
		isPlay = true;  
		isFinish = false;  
	}  
	/// <summary>  
	/// 视频暂停  
	/// </summary>  
	public void MoviePause()  
	{  
		isPlay = false;  
		mediaPlayerCtrl.Pause();  
	}  
	/// <summary>  
	/// 视频停止  
	/// </summary>  
	public void MovieStop()  
	{  
		isPlay = false;  
		mediaPlayerCtrl.Stop();  
	}  
	/// <summary>  
	/// 读取视频  
	/// </summary>  
	/// <param name="namePtah">读取视频的路径</param>  
	public void MovieLoad(string namePtah)  
	{  
		mediaPlayerCtrl.Load(namePtah);  
		mediaPlayerCtrl.m_bAutoPlay = false;  
		isPlay = false;  
		isFinish = false;  
	}  
	/// <summary>  
	/// 视频设置循环播放  
	/// </summary>  
	public void MovieLoop()  
	{  
		mediaPlayerCtrl.m_bLoop = !mediaPlayerCtrl.m_bLoop;  
	}  

	/// <summary>  
	/// 视频全屏显示  
	/// </summary>  
	public void OnClickFullScreen()  
	{  
		if (mediaPlayerCtrl.m_bFullScreen)  
		{  
			mediaPlayerCtrl.m_bFullScreen = false;  
//			movieRawImage.rectTransform.offsetMax = reducteOffsetMax;  
//			movieRawImage.rectTransform.offsetMin = -reducteOffsetMax;  
		}  
		else  
		{  
			mediaPlayerCtrl.m_bFullScreen = true;  
//			movieRawImage.rectTransform.offsetMax = new Vector2(0, 0);  
//			movieRawImage.rectTransform.offsetMin = new Vector2(0, 0);  
		}     
	}  

	/// <summary>  
	/// 关闭视频界面  
	/// </summary>  
	public void OnClickClose()  
	{  
		mediaPlayerCtrl.Stop();  
//		movieRawImage.gameObject.SetActive(false);  
	}  
	/// <summary>  
	/// 移动视频进度条  
	/// </summary>  
	public void OnDragSlider()  
	{  
		if (!isPlay)  
			return;  
		int seekPos = (int)(movieSlider.value * mediaPlayerCtrl.GetDuration());  
		mediaPlayerCtrl.SeekTo(seekPos);  
	}  
	/// <summary>  
	/// 按下视频进度条  
	/// </summary>  
	public void OnPoniterDownSlider()  
	{  
		if (!isPlay)  
			return;  
		MoviePause();  
		isCtrlMovie = true;  
		int seekPos = (int)(movieSlider.value * mediaPlayerCtrl.GetDuration());  
		mediaPlayerCtrl.SeekTo(seekPos);  
	}  
	/// <summary>  
	/// 弹起视频进度条  
	/// </summary>  
	public void OnPoniterUpSlider()  
	{  
		if(!isPlay)  
			return;  
		MoviePlay();  
		isCtrlMovie = false;  
	}  

	public bool GetIsFullScreen()  
	{  
		return mediaPlayerCtrl.m_bFullScreen;  
	}  

	public bool GetIsLoop()  
	{  
		return mediaPlayerCtrl.m_bLoop;  
	}  
	void OnEnd()  
	{  
		isFinish = true;  
	}  
}
