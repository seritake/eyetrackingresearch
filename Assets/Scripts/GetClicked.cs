using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GetClicked : MonoBehaviour, IPointerClickHandler {

    public GameObject displayPhotoController;
    private Sprite spr;
    private string name;

	// Use this for initialization
	void Start () {
        displayPhotoController = GameObject.Find("DisplayPhotoController");
        spr = gameObject.transform.Find("Image").GetComponent<Image>().sprite;
        name = gameObject.transform.Find("Text").gameObject.GetComponent<Text>().text;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPointerClick(PointerEventData data)
    {
        displayPhotoController.GetComponent<GetPhotos>().setPhotoName(name);
        displayPhotoController.GetComponent<GetPhotos>().setTargetPhoto(spr);
    }
}
