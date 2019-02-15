using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : SimulatedObject {

    Sim_GameObject sim;
    Transform t;

    public TextMeshProUGUI inputField;

    public List<GameObject> SpawnBlock;
    public List<PopupBox> popups;
    public List<int> collectiblesNeeded;

    public int stage = 0;
    public int collectiblesHeld = 0;

    //string testString = "MoveNorth();\nMoveEast();\nMoveSouth();\nMoveEast();";
    //public string testString = "Player.health = 5;";
    //string testString = "int var4 = var1 + var2 + var3;\nint var5 = var4 + 10;\nMoveNorth();";
    string testString;

    List<Sim_Function> simFunctions = new List<Sim_Function>();
    List<Sim_Variable> simVariables = new List<Sim_Variable>();

    public override Sim_GameObject GetSim()
    {
        return sim;
    }

    // Use this for initialization
    void Start ()
    {
        Init();

       
    }

    void Init()
    {
        t = this.gameObject.GetComponent<Transform>();
        //Debug.Log(t.ToString());

        Sim_Function _MoveNorth = new Sim_Function("MoveNorth", "", "void", "//Moves the player north one space.", MoveNorth);
        simFunctions.Add(_MoveNorth);

        Sim_Function _MoveSouth = new Sim_Function("MoveSouth", "", "void", "//Moves the player south one space.", MoveSouth);
        simFunctions.Add(_MoveSouth);

        Sim_Function _MoveEast = new Sim_Function("MoveEast", "", "void", "//Moves the player east one space.", MoveEast);
        simFunctions.Add(_MoveEast);

        Sim_Function _MoveWest = new Sim_Function("MoveWest", "", "void", "//Moves the player west one space.", MoveWest);
        simFunctions.Add(_MoveWest);

        Sim_Function _NextStage = new Sim_Function("NextStage", "", "void", "//TEMP DEBUG.", NextStage);
        simFunctions.Add(_NextStage);

        Sim_Variable _health = new Sim_Variable("health", "int", "10", "//How much health the player has.");
        simVariables.Add(_health);

        Sim_Variable _var1 = new Sim_Variable("var1", "int", "1", "//H.");
        simVariables.Add(_var1);
        Sim_Variable _var2 = new Sim_Variable("var2", "int", "2", "//H.");
        simVariables.Add(_var2);
        Sim_Variable _var3 = new Sim_Variable("var3", "int", "3", "//H.");
        simVariables.Add(_var3);

        sim = new Sim_GameObject("Player", simFunctions, simVariables);
    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    public void Run()
    {
        Reset();
        testString = inputField.text.ToString();
        Debug.Log(testString);
        StartCoroutine(RunCode(sim));
    }

    public void Reset()
    {
        collectiblesHeld = 0;
        t.position = new Vector3(SpawnBlock[stage].transform.position.x, t.position.y, SpawnBlock[stage].transform.position.z);
        simFunctions.Clear();
        simVariables.Clear();
        Init();
    }

    public void SetupStage()
    {
        Reset();
        popups[stage].gameObject.SetActive(true);
    }







    string MoveNorth(string arg)
    {
        //Debug.Log("Moving North");
        t.position = new Vector3(t.position.x, t.position.y, t.position.z + 1.0f);
        return "null";
    }

    string MoveSouth(string arg)
    {
        //Debug.Log("Moving South");
        t.position = new Vector3(t.position.x, t.position.y, t.position.z - 1.0f);
        return "null";
    }

    string MoveEast(string arg)
    {
        //Debug.Log("Moving East");
        t.position = new Vector3(t.position.x + 1.0f, t.position.y, t.position.z);
        return "null";
    }

    string MoveWest(string arg)
    {
        //Debug.Log("Moving West");
        t.position = new Vector3(t.position.x - 1.0f, t.position.y, t.position.z);
        return "null";
    }



    string NextStage(string arg)
    { 
        stage++;
        Debug.Log(stage);
        return "null";
    }

































    public IEnumerator RunCode(Sim_GameObject sim)
    {
        string[] CodeLines = null;
        CodeLines = testString.Split('\n');
        Debug.Log(CodeLines.Length);

        for (int x = 0; x < CodeLines.Length; x++)
        {
            Debug.Log(CodeLines[x]);
            string firstItem = "", remainder = "";
            string[] subStrings = CodeLines[x].Split('(', ' ');

            if (subStrings.Length == 0)
            {
                Debug.Log("Error");
            }
            else if (subStrings.Length == 1)
            {
                firstItem = subStrings[0];
                Debug.Log(firstItem);
            }
            else
            {
                firstItem = subStrings[0];
                remainder = concatenateSplitString(1, subStrings, " ");
                Debug.Log(firstItem);
                Debug.Log(remainder);
            }

            //Check if the first item references a call to an existing function or method.
            Sim_Function function = FindSimFunction(firstItem, sim.functions);
            if (function != null)
            {
                function.Call(remainder);
            }
            else
            {
                //Check if the first item references an existing variable.
                Sim_Variable variable = FindSimVariable(firstItem, sim.variables);
                if (variable != null)
                {
                    if (variable.type == "int")
                    {
                        performIntOperations(variable, remainder);
                    }
                    else if (variable.type == "bool")
                    {
                        performBoolOperations(variable, remainder);
                    }
                    else if (variable.type == "char")
                    {
                        performCharOperations(variable, remainder);
                    }
                    else if (variable.type == "string")
                    {
                        performStringOperations(variable, remainder);
                    }
                    else
                    {
                        //Debug.Log("Not A Variable");
                    }
                }
                else
                {
                    //Check if first item is attempting to create a new variable.
                    if (firstItem == "int")
                    {
                        //split name and value

                        //create int with name and value
                        createInt(remainder);
                    }
                    else if (firstItem == "bool")
                    {
                        //createBool();
                    }
                    else if (firstItem == "char")
                    {
                        //createChar();
                    }
                    else if (firstItem == "string")
                    {
                        //createString();
                    }
                    else
                    {
                        Debug.Log("Not A Variable");
                    }
                }
            }

            //Temp
            yield return new WaitForSeconds(1.0f);
        }
    }







    Sim_Variable FindSimVariable(string name, List<Sim_Variable> list)
    {
        Sim_Variable var = null;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].name == name)
            {
                var = list[i];
            }
        }
        return var;
    }

    Sim_Function FindSimFunction(string name, List<Sim_Function> list)
    {
        Sim_Function func = null;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].name == name)
            {
                func = list[i];
            }
        }
        return func;
    }

    /*
    Sim_GameObject FindSimGameObject(string name, List<Sim_GameObject> list, Sim_GameObject main)
    {
        Sim_GameObject obj = main;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].name == name)
            {
                obj = list[i];
            }
        }
        return obj;
    }
    */

    string concatenateSplitString(int startIndex, string[] items)
    {
        string returnString = "";
        for (int i = startIndex; i < items.Length; i++)
        {
            returnString += items[i];
        }
        return returnString;
    }

    string concatenateSplitString(int startIndex, string[] items, string splitter)
    {
        string returnString = "";
        for (int i = startIndex; i < items.Length; i++)
        {
            returnString += items[i];
            returnString += splitter;
        }
        return returnString;
    }

    string concatenateSplitString(int startIndex, int endIndex, string[] items, string splitter)
    {
        string returnString = "";
        for (int i = startIndex; i < endIndex; i++)
        {
            returnString += items[i];
            returnString += splitter;
        }
        return returnString;
    }

    string performIntOperations(Sim_Variable var, string remainder)
    {
        // if =
        //variable = recursivecall on remainders
        // if +=
        //variable += recursivecall on remaidners
        //etc

        //if ++
        //add one
        //if --
        //remove one

        string[] subStrings = remainder.Split(' ', ';');
        remainder = concatenateSplitString(1, subStrings, " ");

        if(subStrings[0] == "=")
        {
            Debug.Log(remainder);
            Debug.Log("BEFORE: " + var.value);
            var.value = recursiveIntOperations(remainder).ToString();
            Debug.Log("AFTER: " + var.value);
        }
        else if (subStrings[0] == "+=")
        {
            int temp;
            int.TryParse(var.value, out temp);
            var.value = (temp + recursiveIntOperations(remainder)).ToString();
        }
        else if (subStrings[0] == "-=")
        {
            int temp;
            int.TryParse(var.value, out temp);
            var.value = (temp - recursiveIntOperations(remainder)).ToString();
        }
        else if (subStrings[0] == "*=")
        {
            int temp;
            int.TryParse(var.value, out temp);
            var.value = (temp * recursiveIntOperations(remainder)).ToString();
        }
        else if (subStrings[0] == "/=")
        {
            int temp, temp2;
            int.TryParse(var.value, out temp);
            temp2 = recursiveIntOperations(remainder);
            if (temp2 != 0)
                var.value = (temp / temp2).ToString();
            else
                Debug.Log("Cannot divide by zero!!!");
        }
        else if (subStrings[0] == "++")
        {
            int temp;
            int.TryParse(var.value, out temp);
            var.value = (temp++).ToString();
        }
        else if (subStrings[0] == "--")
        {
            int temp;
            int.TryParse(var.value, out temp);
            var.value = (temp--).ToString();
        }

        return "";
    }

    string performStringOperations(Sim_Variable var, string remainder)
    {

        string[] subStrings = remainder.Split(' ', ';');
        remainder = concatenateSplitString(1, subStrings, " ");

        if (subStrings[0] == "=")
        {
            Debug.Log(remainder);
            Debug.Log("BEFORE: " + var.value);
            var.value = recursiveIntOperations(remainder).ToString();
            Debug.Log("AFTER: " + var.value);
        }
        else if (subStrings[0] == "+=")
        {
            int temp;
            int.TryParse(var.value, out temp);
            var.value = (temp + recursiveIntOperations(remainder)).ToString();
        }
        else if (subStrings[0] == "-=")
        {
            int temp;
            int.TryParse(var.value, out temp);
            var.value = (temp - recursiveIntOperations(remainder)).ToString();
        }
        else if (subStrings[0] == "*=")
        {
            int temp;
            int.TryParse(var.value, out temp);
            var.value = (temp * recursiveIntOperations(remainder)).ToString();
        }
        else if (subStrings[0] == "/=")
        {
            int temp, temp2;
            int.TryParse(var.value, out temp);
            temp2 = recursiveIntOperations(remainder);
            if (temp2 != 0)
                var.value = (temp / temp2).ToString();
            else
                Debug.Log("Cannot divide by zero!!!");
        }
        else if (subStrings[0] == "++")
        {
            int temp;
            int.TryParse(var.value, out temp);
            temp++;
            Debug.Log(temp);
            var.value = (temp).ToString();
        }
        else if (subStrings[0] == "--")
        {
            int temp;
            int.TryParse(var.value, out temp);
            temp--;
            Debug.Log(temp);
            var.value = (temp--).ToString();
        }

        return "";
    }

    string performCharOperations(Sim_Variable var, string remainder)
    {
        string[] subStrings = remainder.Split(' ', ';');
        remainder = concatenateSplitString(1, subStrings, " ");

        if (subStrings[0] == "=")
        {
            Debug.Log(remainder);
            Debug.Log("BEFORE: " + var.value);
            if (remainder == "true")
                var.value = "true";
            else if (remainder == "false")
                var.value = "false";
            else
                Debug.Log(remainder + " is an invalid value for a boolean variable.");

            Debug.Log("AFTER: " + var.value);
        }
        
        return "";
    }

    string performBoolOperations(Sim_Variable var, string remainder)
    {
        string[] subStrings = remainder.Split(' ', ';');
        remainder = concatenateSplitString(1, subStrings, " ");

        if (subStrings[0] == "=")
        {
            Debug.Log(remainder);
            Debug.Log("BEFORE: " + var.value);
            if (remainder == "true")
                var.value = "true";
            else if (remainder == "false")
                var.value = "false";
            else
                Debug.Log(remainder + " is an invalid value for a boolean variable.");

            Debug.Log("AFTER: " + var.value);
        }

        return "";
    }


    //var1 = var2 + 12 + var3 - 6;
    //var1 = recursiveInt( var2 + 12 + var3 - 6;)

    int recursiveIntOperations(string items)
    {
        int returnVal;

        string[] subStrings = items.Split(' ', ';');
        string varID = subStrings[0];

        //get the value of the variable.
        Sim_Variable variable = FindSimVariable(varID, simVariables);
        if (variable != null)
        {
            if (isInt(variable))
            {
                if (int.TryParse(variable.value, out returnVal))
                {

                }
                else
                {
                    //fuck
                }
            }
            else
            {
                Debug.Log("Variable is not an integer");
                return 0;
            }
        }
        else if (int.TryParse(varID, out returnVal))
        {

        }
        else
        {
            //fuck
        }

        if (subStrings.Length > 1)
        {
            string intOperator = subStrings[1];
            items = concatenateSplitString(2, subStrings, " ");

            if (intOperator == "+")
            {
                returnVal += recursiveIntOperations(items);
            }
            else if (intOperator == "-")
            {
                returnVal -= recursiveIntOperations(items);
            }
            else if (intOperator == "*")
            {
                returnVal *= recursiveIntOperations(items);
            }
            else if (intOperator == "/")
            {
                returnVal /= recursiveIntOperations(items);
            }
            else
            {
                //Debug.Log("ERROR: " + intOperator + " is not a valid operator.");
            }
        }

        return returnVal;
    }


    string createInt(string remainder)
    {

        string[] subStrings = remainder.Split(' ');
        remainder = concatenateSplitString(1, subStrings, " ");
        Debug.Log(remainder);
        Sim_Variable newInt = new Sim_Variable(subStrings[0], "int", "0");
        performIntOperations(newInt, remainder);
        Debug.Log(newInt.ToString());

        createInt(newInt.name, newInt.value);

        return "";
    }

    string createBool(string remainder)
    {
        return "";
    }

    string createString(string remainder)
    {
        return "";
    }

    string createChar(string remainder)
    {
        return "";
    }

    string createInt(string name, string value)
    {
        int val;
        if (int.TryParse(value, out val))
        {
            Sim_Variable newInt = new Sim_Variable(name, "int", val.ToString());
            simVariables.Add(newInt);
        }
        else
        {
            Debug.Log("Value is not an integer.");
        }
        return "";
    }

    string createBool(string name, string value)
    {
        if (value == "true" || value == "false")
        {
            Sim_Variable newBool = new Sim_Variable(name, "bool", value);
            simVariables.Add(newBool);
        }
        else
        {
            Debug.Log("Value is not an boolean.");
        }
        return "";
    }

    string createString(string name, string value)
    {
        //if (value == "true" || value == "false")
        //{
        //    Sim_Variable newBool = new Sim_Variable(name, "bool", value);
        //    simVariables.Add(newBool);
        //}
        //else
        //{
        //    Debug.Log("Value is not an boolean.");
        //}
        return "";
    }

    string createChar(string name, string value)
    {
        if (value.Length == 1)
        {
            Sim_Variable newBool = new Sim_Variable(name, "char", value);
            simVariables.Add(newBool);
        }
        else
        {
            Debug.Log("Value is not an character.");
        }
        return "";
    }

    bool isInt(Sim_Variable var)
    {
        return var.type == "int";
    }

    bool isBool(Sim_Variable var)
    {
        return var.type == "bool";
    }

    bool isChar(Sim_Variable var)
    {
        return var.type == "char";
    }

    bool isString(Sim_Variable var)
    {
        return var.type == "string";
    }

}
