using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{

    public Player player;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player.collectiblesHeld++;
            this.gameObject.SetActive(false);
        }
    }
}
