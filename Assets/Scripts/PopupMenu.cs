using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupMenu : MonoBehaviour {

    bool isActive;
    public GameObject menu;

	// Update is called once per frame
	void Update ()
    {
		if(menu.active == true)
        {
            isActive = true;
        }
        else
        {
            isActive = false;
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isActive)
            {
                menu.SetActive(false);
            }
            else
            {
                menu.SetActive(true);
            }
        }
	}
}
