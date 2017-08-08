using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FocusChange : MonoBehaviour {

    public Sprite Unfocus;
    public Sprite Focus;

    public Color Focusbg;
    public Color Unfocusbg;

    private Image ImageButton;

	// Use this for initialization
	void Start () {
        ImageButton = gameObject.GetComponent<Image>();

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ButtonFocus() {
        if (ImageButton!=null) {
            ImageButton.sprite = Focus;
        }
    }

    public void ButtonUnfocus() {
        if (ImageButton != null)
        {
            ImageButton.sprite = Unfocus;
        }
    }

    public void ColorFocus()
    {
        if (ImageButton != null)
        {
            ImageButton.color = Focusbg;
        }
    }

    public void ColorUnfocus()
    {
        if (ImageButton != null)
        {
            ImageButton.color = Unfocusbg;
        }
    }

}
