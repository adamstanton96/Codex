using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableTracker : MonoBehaviour
{

    public Player player;
    public GameObject popup;

    public int expectedCharCount;
    public int expectedIntCount;
    public int expectedStringCount;
    public int expectedBoolCount;

    // Use this for initialization
    void Start()
    {
        //Debug.Log(listenString);
    }

    // Update is called once per frame
    void Update()
    {
        int charCount = 0;
        int intCount = 0;
        int stringCount = 0;
        int boolCount = 0;

        for(int i = 0; i < player.GetSim().variables.Count; i++)
        {
            if (player.GetSim().variables[i].type == "char")
                charCount++;
            else if (player.GetSim().variables[i].type == "int")
                intCount++;
            else if (player.GetSim().variables[i].type == "string")
                stringCount++;
            else if (player.GetSim().variables[i].type == "bool")
                boolCount++;
        }

        //Debug.Log(charCount + " " + intCount + " " + stringCount + " " + boolCount + " ");

        if (charCount == expectedCharCount && intCount == expectedIntCount && stringCount == expectedStringCount && boolCount == expectedBoolCount)
        {
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