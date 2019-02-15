using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupBox : MonoBehaviour {

    //public Button back, next;
    public TextMeshProUGUI text;

    public List<string> dialog;

    int index = 0;

	// Use this for initialization
	void Start ()
    {
        text.text = dialog[0];
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void NextPressed()
    {
        if(index < dialog.Count - 1)
        {
            index++;
            text.text = dialog[index];
        }
        else
        {
            /*
            var children = new List<GameObject>();
            foreach (Transform child in transform) children.Add(child.gameObject);
            children.ForEach(child => Destroy(child));
            Destroy(this);
            */
            this.gameObject.SetActive(false);
        }
    }

    public void BackPressed()
    {
        if (index > 0)
        {
            index--;
            text.text = dialog[index];
        }
    }
}
