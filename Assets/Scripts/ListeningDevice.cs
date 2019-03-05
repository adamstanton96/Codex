using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListeningDevice : MonoBehaviour {

    public Player player;
    public GameObject popup;
    public string listenString = "Hello World";

    // Use this for initialization
    void Start()
    {
        //Debug.Log(listenString);
    }

    // Update is called once per frame
    void Update()
    {
        if(player.GetPassword() == listenString)
        {
            //Debug.Log("done");
            NextStage();
            this.gameObject.SetActive(false);
        }
    }


    void NextStage()
    {
        if (player.stage < player.stages.Count)
        {
            player.stage++;
            if (popup != null)
            {
                popup.SetActive(true);
            }
            else
            {
                Debug.Log("No Popup Found.");
            }
        }
        else
        {
            player.s.LoadScene("MainMenu");
        }
    }
}

