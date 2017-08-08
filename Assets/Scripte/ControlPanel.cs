using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPanel : MonoBehaviour {

	public GameObject PlayInfo;
	public GameObject PlayControl;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Hide()
	{
		if(PlayInfo!=null&&PlayControl!=null)
		{
			PlayControl.SetActive (false);
			PlayInfo.SetActive (false);
		}
	}

	public void Show()
	{
		if(PlayControl!=null&&PlayInfo!=null)
		{
			PlayInfo.SetActive (true);
			PlayControl.SetActive (true);
		}
	}


}
