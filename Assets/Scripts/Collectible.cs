using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{

    public Player player;

	// Use this for initialization
	void Start ()
    {
        // add isTrigger
        var boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player.collectiblesHeld++;
            this.gameObject.SetActive(false);
        }
    }
}
