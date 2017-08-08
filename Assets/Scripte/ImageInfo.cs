using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ImageInfo : MonoBehaviour {

	string imageName;
    string path;

	Text label;
	Image icon;

	Sprite sprite;

	AndroidJavaObject appHelperObject;
    GameObject Pano;

	// Use this for initialization
	void Start () {
		label = transform.Find("label").gameObject.GetComponent<Text> ();
		icon = transform.Find("icon").gameObject.GetComponent<Image> ();
        Pano = GameObject.Find("Pano");
		//Debug.Log ("label text is " + label.text);
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    

    //public void setPackageName(string packageName) {
    //	this.packageName = packageName;
    //}

    public void setImageName(string imageName) {
		this.imageName = imageName;
		label.text = imageName;

	}

//    public void setPath(string path,byte[] data)
//    {
//        this.path = path;
//        setIcon(data);
//    
//    }
//


	public Sprite DefaultSprite;
	public void setIcon(byte[] data) {
		Texture2D tex = new Texture2D (2, 2);
	//	data = tex.EncodeToPNG ();
		tex.LoadImage (data);

		tex.filterMode = FilterMode.Trilinear;

		sprite = Sprite.Create (tex, new Rect (0, 0, tex.width, tex.height), new Vector2 (0.5f, 0.5f), 100f); 
		//Debug.Log("Icon width=" + tex.width + " height=" + tex.height);
		icon.sprite = sprite;
	}

	public void setAppHelper(AndroidJavaObject appHelper) {
		appHelperObject = appHelper;
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

   

 //   public void startApp() {
	//	appHelperObject.Call ("startApp", packageName);
	//}

//	public static implicit operator GameObject(VideoInfo v)
//    {
//        throw new NotImplementedException();
//    }


    
}
