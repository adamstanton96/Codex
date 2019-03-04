using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPoint : MonoBehaviour
{
    public Player player;
    public GameObject popup;

    // Use this for initialization
    void Start()
    {
        // add isTrigger
        var boxCollider = gameObject.GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("inside");
        if (other.tag == "Player")
        {
            {
                NextStage();
            }
        }
    }

    void NextStage()
    {
        Debug.Log(player.stage);
        if (player.stage < player.stages.Count)
        {
            player.stage++;
            //player.SetupStage();
            if (popup != null)
            {
                Debug.Log("Making Popup Active");
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

