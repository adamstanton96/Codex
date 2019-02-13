using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public string testString = "Player.MoveNorth()";
    public List<SimulatedObject> objects;
    public List<Sim_GameObject> simulatedObjects;

    // Use this for initialization
    void Start()
    {
        /*
        //for (int i = 0; i < objects.Count; i++)
        //{
        //    if (objects[i].)
        //        simulatedObjects.Add(objects[i].GetComponent<Sim_GameObject>());
        //}

        for (int i = 0; i < objects.Count; i++)
            simulatedObjects.Add(objects[i].GetSim());

        for (int i = 0; i < objects.Count; i++)
        {
           Debug.Log(simulatedObjects[i].ToString());
        }

        string firstItem = "", remainder = "";

        char[] delimiterChars = { ' ', '.' };

        string[] subStrings = testString.Split(delimiterChars);

        if (subStrings.Length == 0)
        {
            Debug.Log("Error");
        }
        else if (subStrings.Length == 1)
        {
            firstItem = subStrings[0];
        }
        else
        {
            firstItem = subStrings[0];
            string remainderSS = "";
            for (int i = 1; i < subStrings.Length; i++)
            {
                remainderSS += subStrings[i];
            }
            remainder = remainderSS;
        }

        Sim_GameObject target = null;

        for (int i = 0; i < simulatedObjects.Count; i++)
        {
            if (simulatedObjects[i].name == firstItem)
            {
                target = simulatedObjects[i];
            }
        }

        subStrings = remainder.Split('(', ' ', '=', '+', '-', '*', '/');
        if (subStrings.Length == 0)
        {
            Debug.Log("Error");
        }
        else if (subStrings.Length == 1)
        {
            firstItem = subStrings[0];
        }
        else
        {
            firstItem = subStrings[0];
            string remainderSS = "";
            for (int i = 1; i < subStrings.Length; i++)
            {
                remainderSS += subStrings[i];
            }
            remainder = remainderSS;
        }

        if (target != null)
        {
            for (int i = 0; i < target.functions.Count; i++)
            {
                if (target.functions[i].name == firstItem)
                {
                    target.functions[i].Call(remainder);
                }
            }
        }

        /*
        remainder = subStrings[1];
        std::cout << firstItem << std::endl << remainder << std::endl;

        sim_gameObject* target = nullptr;
        //If not one of these statements, check if system or a valid object...
        for (int i = 0; i < sim_gameObject_list.size(); i++)
        {
            if (firstItem == sim_gameObject_list[i]->name)
            {
                target = sim_gameObject_list[i];
            }
        }

        if (target != nullptr)
        {
            std::string secondItem;
            std::string thirdItem;
            std::istringstream iss2(remainder);
            std::getline(iss2, secondItem, '(');
            std::getline(iss2, thirdItem);
            //Get the second item
            for (int i = 0; i < target->functionList.size(); i++)
            {
                if (secondItem == target->functionList[i].name)
                {
                    //Get the third item, inside brackets
                    //std::cout << thirdItem <<std::endl;
                    std::cout << target->functionList[i].function_ref(thirdItem);
                    //ensure the parameters mtahc up
                    //call the actual function using the parameters
                }
            }
        }
        else
        {
            std::string firstItem, remainder;

            std::istringstream issss(testLine);
            std::getline(issss, firstItem, ' ');
            std::getline(issss, remainder);

            //check if a variable
            for (int i = 0; i < sim_variable_list.size(); i++)
            {
                if (firstItem == sim_variable_list[i]->name)
                {
                    std::cout << "oh lawd a variable with value " << sim_variable_list[i]->content;
                }
            }
            
        }*/
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CallCode()
    {
        //Break down code into lines using \n or some other format, store as a queue/array/list of strings.
        //Move through the list, testing each line to...
        //Determine if the code is:
        //A: A conditional
        //Generate the condition and determine if true or false
        //If true call the code
        //B: A for loop
        //C: A conditional do loop
        //D: A conditional while loop
        //E: A system function call
        //F: A class function call
        //
        //G: A variable operation
        //
        //H: The creation of a new variable.
        //I: The creation of a new function.


    }
}
