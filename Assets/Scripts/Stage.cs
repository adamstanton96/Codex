using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour {

    public GameObject spawnBlock;
    public PopupBox popupDialog;
    public List<Collectible> collectibles;
    public ExitPoint exitPoint;
    public ListeningDevice listDev;
    public VariableTracker varTrack;
    public string MapDirection = "No Map";

    public string[] hints;
    int hintIndex = 0;

    // Use this for initialization
    void Start ()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public string GetHint()
    {
        string returnString = hints[hintIndex];
        Debug.Log(returnString);
        if(hintIndex < hints.Length - 1)
            hintIndex++;
        return returnString;
    }


    public void RefreshStage()
    {
        hintIndex = 0;
        popupDialog.gameObject.SetActive(true);
        popupDialog.Init();
        ResetCollectibles();
        if(listDev != null)
            listDev.gameObject.SetActive(true);
        if (varTrack != null)
            varTrack.gameObject.SetActive(true);
    }

    public void ResetCollectibles()
    {
        for (int i = 0; i < collectibles.Count; i++)
        {
            collectibles[i].gameObject.SetActive(true);
        }
    }
}
