using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayArea : MonoBehaviour {

    public Player player;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player.SimPrintError("LOGIC ERROR: Player has attempted to leave the game area.");
        }
    }
}
