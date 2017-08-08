using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class ImageProvider : MonoBehaviour
{

    AndroidJavaObject objectStore;
    AndroidJavaObject providerObject;
	AndroidJavaObject info;
    GameObject imagePanel;
    int pageImageCount = 8;
    public GameObject[] pageImages;
    public ImageInfo[] pageImagesInfos;
    int currentPage = 0;
    int currenPageImageCount;
    int imageSize;
    int maxPage;
    int currentPageimageCount;
    Sprite sprite;
    public Texture texture;
 
    GameObject panoramicSpanel;
    GameObject Normal;
    GameObject contorlPanel;
    GameObject Canvas;
    GameObject ImageListPanel;
    public int position;

    private string currentData;
    private string currentImage;
    private bool isbmp;
    private bool isIconReady;
    private byte[] bytesBMP;
    private byte[] aysnbytes;
    private bool isChanged;

    private bool isProviderReady = false;

    public PanelSwitcher mPanelSwitcher;

    // Use this for initialization
    void Start()
    {


        contorlPanel = GameObject.Find("Control");

        imagePanel = GameObject.Find("Image");
        ImageListPanel = GameObject.Find("ImageListPanel");


        panoramicSpanel = GameObject.Find("pano");

        Canvas = GameObject.Find("Canvas");

        Normal = GameObject.Find("Normal");
        //Normal.SetActive (false);



        if (panoramicSpanel == null)
        {
            Debug.Log("Pano not find!!");
        }

        //Debug.Log("Provider  start!!");
        //Debug.Log("==================");
        objectStore = new AndroidJavaObject("com.dlighttech.vr.apps.imageview.utils.ObjectStore");
        //        info = new AndroidJavaObject("com.dlighttech.apps.vr.imageview.bean.ImageInfo");

        providerObject = objectStore.CallStatic<AndroidJavaObject>("get", "ImageProvider");



        pageImageCount = imagePanel.transform.childCount;

        pageImages = new GameObject[pageImageCount];
        pageImagesInfos = new ImageInfo[pageImageCount];   



        for (int i = 0; i < pageImageCount; i++)
        {
            GameObject pageImage = imagePanel.transform.GetChild(i).gameObject;
            pageImages[i] = pageImage;
            pageImage.SetActive(true);
            ImageInfo info = pageImage.GetComponent<ImageInfo>();
            pageImagesInfos[i] = info;

        }

    }



    void bakeBeef(AndroidJavaObject info, ImageInfo image)
    {

        string imageName = info.Get<string>("imageName");
        // image = info.Get<ImageInfo>("image");
        string path = info.Get<string>("path");

        isbmp = info.Call<bool>("isBMP");





        image.setImageName(imageName.ToString());

        //  Debug.Log("imageName=="+ imageName);

        isIconReady = info.Get<bool>("isIconReady");
        if (isIconReady == true && image.isIconSetted() == false)
        {
            byte[] data = info.Get<byte[]>("iconBytes");
            //Debug.Log("data===" + data.Length);


            if (data == null)
            {
                return;
            }

            image.setIcon(data);




        }

    }


    void bakeImageInfo(AndroidJavaObject[] imageInfos)
    {


        int i;
        for (i = 0; i < imageInfos.Length; i++)
        {
            pageImages[i].SetActive(true);

            bakeBeef(imageInfos[i], pageImagesInfos[i]);


        }
        for (; i < pageImageCount; i++)
        {
            pageImages[i].SetActive(false);
        }
    }


    void refresh()
    {

        AndroidJavaObject[] imageinfos = providerObject.Call<AndroidJavaObject[]>("get", currentPage, pageImageCount);
        //  Debug.Log("imageinfos==" + imageinfos.Length); //12


        imageSize = providerObject.Call<int>("getSize");

        if (imageinfos.Length == 0 || imageSize == 0)
        {

            for (int i = 0; i < pageImageCount; i++)
            {
                GameObject pageImage = imagePanel.transform.GetChild(i).gameObject;
                pageImages[i] = pageImage;
                pageImage.SetActive(false);
                //contorlPanel.SetActive (false);
                ImageInfo info = pageImage.GetComponent<ImageInfo>();
                pageImagesInfos[i] = info;

            }

        }

        Debug.Log("imageSize===" + imageSize);



        maxPage = imageSize / pageImageCount + ((imageSize % pageImageCount == 0) ? 0 : 1) - 1;

        if (imageinfos != null && imageinfos.Length > 0)
        {
            bakeImageInfo(imageinfos);
        }

        for (int i = 0; i < imageinfos.Length; i++)
        {
            imageinfos[i].Dispose();
        }

    }

    public void nextPage()
    {


        currentPage++;

        if (currentPage > maxPage)
        {
            currentPage = maxPage;
        }
        for (int i = 0; i < pageImagesInfos.Length; i++)
        {

            pageImagesInfos[i].changTopage();
        }
        refresh();



    }



    public void prevPage()
    {

        currentPage--;
        if (currentPage < 0)
        {
            currentPage = 0;
        }


        for (int i = 0; i < pageImagesInfos.Length; i++)
        {
            pageImagesInfos[i].changTopage();
        }

        refresh();

        Debug.Log("currentPage===" + currentPage);

    }


    public void backButton()
    {

        Application.Quit();
    }



	//点击一张图片 传入path  进入全景图模式
    public void EnterPanoramic(int position)
    {

        AndroidJavaObject[] imageinfos = providerObject.Call<AndroidJavaObject[]>("get", currentPage, pageImageCount);


        string imagepath = @imageinfos[position].Get<string>("path");


        Debug.Log("iamgePath==" + imagepath);

        if (imagepath == null)
        {

            return;
        }

        showImageByPath(imagepath);

    }



    public void showImageByPath(string ImagePath) {
        info = new AndroidJavaObject("com.dlighttech.vr.apps.imageview.bean.ImageInfo");
        bytesBMP = info.Call<byte[]>("BitmapBytes", ImagePath);
        //Debug.Log ("bytesBMP=="+bytesBMP.Length);
        if (isbmp == true)
        {
            //isChanged = info.Get<bool> ("isChange");
            //Debug.Log ("isChanged---"+isChanged);
            //if(isChanged==true)
            //{
            //	aysnbytes = info.Get<byte[]> ("bytesAYSN");

            int width = 130;
            int height = 130;
            Texture2D text = new Texture2D(width, height, TextureFormat.BGRA32, false);
            text.LoadImage(bytesBMP);
            Normal.transform.localScale = new Vector3(-text.width / 2, -text.height / 2, 3);
            Renderer renderDst = panoramicSpanel.GetComponent<Renderer>();
            Renderer renderEst = Normal.GetComponent<Renderer>();
            renderDst.materials[0].SetTexture("_MainTex", text);
            renderEst.materials[0].SetTexture("_MainTex", text);
            //}

        }
        else if (isbmp == false)
        {

            WWW www = new WWW("file://" + ImagePath);
            //yield return www;

            //获取Texture
            Texture2D texture1 = www.texture;


            Normal.transform.localScale = new Vector3(-texture1.width / 2, -texture1.height / 2, 3);

            Renderer renderDst = panoramicSpanel.GetComponent<Renderer>();
            Renderer renderEst = Normal.GetComponent<Renderer>();

            // Texture tex = renderSrc.materials[0].GetTexture("_MainTex");
            renderDst.materials[0].SetTexture("_MainTex", texture1);
            renderEst.materials[0].SetTexture("_MainTex", texture1);
        }
    }

    public void HandleActivityIntent(string imagepath) {
        if (imagepath!=null&& imagepath.Length>0) {
            showImageByPath(imagepath);
//            mPanelSwitcher.showImageItemPanelFromList();
        }
    }

    void Update()
    {


        if (Time.frameCount % 30 == 0)
        {
            if (isProviderReady == false)
            {
                refresh();

                isProviderReady = providerObject.Get<bool>("isProviderReady");
            }

            string FilePath = objectStore.CallStatic<string>("get", "FilePath");
            
            if (FilePath != null && FilePath.Length > 0)
            {
                Debug.Log("ImageProvider FilePath==" + FilePath);
                HandleActivityIntent(FilePath);
                objectStore.CallStatic("put", "FilePath", "");
            }

        }



        //		isChanged = info.Get<bool> ("isChange");
        //		Debug.Log ("isChange=="+isChanged);
        //
        //		if(isChanged==true)
        //		{
        //		 aysnbytes = info.Get<byte[]> ("bytesAYSN");
        //			Debug.Log ("aysnbytes==="+aysnbytes);
        //
        //
        //		}



        //  refresh();

    }
}
