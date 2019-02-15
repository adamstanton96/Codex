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

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("entered");
        if (other.tag == "Player")
        {
            if (player.collectiblesHeld >= player.collectiblesNeeded[player.stage])
            {
                player.stage++;
                player.SetupStage();
            }
        }
    }
}
