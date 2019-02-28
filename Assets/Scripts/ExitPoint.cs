using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPoint : MonoBehaviour
{
    public Player player;

    // Use this for initialization
    void Start()
    {
        // add isTrigger
        var boxCollider = gameObject.GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("entered");
    //    if (other.tag == "Player")
    //    {
    //        if (player.collectiblesHeld >= player.stages[player.stage].collectibles.Count)
    //        {
    //            if (player.stage < player.stages.Count - 1)
    //            {
    //                player.stage++;
    //                player.SetupStage();
    //            }
    //            else
    //            {
    //                player.gameObject.SetActive(false);
    //            }
    //        }
    //    }
    //}

    void OnTriggerStay(Collider other)
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
        if (player.stage < player.stages.Count - 1)
        {
            player.stage++;
            player.SetupStage();
        }
        else
        {
            player.s.LoadScene("MainMenu");
        }
    }
}

