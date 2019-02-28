using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ForLoop
{
    int index;
    string codeString;
}

public class Player : SimulatedObject {

    Sim_GameObject sim;
    Transform t;
    public SceneController s;

    public TextMeshProUGUI inputField;
    public TextMeshProUGUI outputField;
    public TextMeshProUGUI infoField;

    public List<Stage> stages;

    public int stage = 0;
    public int collectiblesHeld = 0;

    //string defaultOutputString = "Code Output:\n";
    //string outputString = "Code Output:\n";

    string defaultOutputString = "";
    string outputString = "";
    string password = "";

    //string testString = "MoveNorth();\nMoveEast();\nMoveSouth();\nMoveEast();";
    //public string testString = "Player.health = 5;";
    //string testString = "int var4 = var1 + var2 + var3;\nint var5 = var4 + 10;\nMoveNorth();";
    string testString;

    List<Sim_Function> simFunctions = new List<Sim_Function>();
    List<Sim_Variable> simVariables = new List<Sim_Variable>();

    bool runComplete = false;

    public override Sim_GameObject GetSim()
    {
        return sim;
    }

    // Use this for initialization
    void Start ()
    {
        Init();
        FullReset();
        t.position = new Vector3(stages[stage].spawnBlock.transform.position.x, t.position.y, stages[stage].spawnBlock.transform.position.z);
    }

    void Init()
    {
        t = this.gameObject.GetComponent<Transform>();

        Sim_Function _MoveNorth = new Sim_Function("MoveNorth", "", "void", "//Moves the player north one space.", MoveNorth);
        simFunctions.Add(_MoveNorth);

        Sim_Function _MoveSouth = new Sim_Function("MoveSouth", "", "void", "//Moves the player south one space.", MoveSouth);
        simFunctions.Add(_MoveSouth);

        Sim_Function _MoveEast = new Sim_Function("MoveEast", "", "void", "//Moves the player east one space.", MoveEast);
        simFunctions.Add(_MoveEast);

        Sim_Function _MoveWest = new Sim_Function("MoveWest", "", "void", "//Moves the player west one space.", MoveWest);
        simFunctions.Add(_MoveWest);

        Sim_Function _NextStage = new Sim_Function("NextStage", "", "void", "//For Debug.", NextStage);
        simFunctions.Add(_NextStage);

        sim = new Sim_GameObject("Player", simFunctions, simVariables);
    }

    // Update is called once per frame
    void Update ()
    {
        infoField.text = this.ToString();
        outputField.text = this.outputString;
        //Debug.Log(this.outputString);
    }

    public void Run()
    {
        PartialReset();
        testString = inputField.text.ToString();
        StartCoroutine(RunCode(sim, testString));
    }

    public void FullReset()
    {
        stages[stage].RefreshStage();
        PlayerReset();
    }

    public void PartialReset()
    { 
        stages[stage].ResetCollectibles();
        PlayerReset();
    }

    void PlayerReset()
    {
        password = "";
        outputString = defaultOutputString;
        collectiblesHeld = 0;
        stages[stage].ResetCollectibles();
        t.position = new Vector3(stages[stage].spawnBlock.transform.position.x, t.position.y, stages[stage].spawnBlock.transform.position.z);
        simFunctions.Clear();
        simVariables.Clear();
        Init();
    }

    public void SetupStage()
    {
        FullReset();
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




    public override string ToString()
    {
        string returnStr = "Functions:\n";
        for (int i = 0; i < simFunctions.Count; i++)
        {
            returnStr += simFunctions[i].ToString();
        }

        returnStr += "\n\nVariables:\n";
        for (int i = 0; i < simVariables.Count; i++)
        {
            returnStr += simVariables[i].ToString();
        }
        return returnStr;
    }




























    public IEnumerator RunCode(Sim_GameObject sim, string codeString)
    {
        string[] CodeLines = null;
        CodeLines = codeString.Split('\n');
        //Debug.Log(CodeLines.Length);

        for (int x = 0; x < CodeLines.Length; x++)
        {
            runComplete = false;

            //Debug.Log(CodeLines[x]);
            string firstItem = "", remainder = "";
            string[] subStrings = CodeLines[x].Split('(', ' ');

            if (subStrings.Length == 0)
            {
                Debug.Log("Error no arguments found.");
            }
            else if (subStrings.Length == 1)
            {
                firstItem = subStrings[0];
                //Debug.Log(firstItem);
            }
            else
            {
                firstItem = subStrings[0];
                remainder = concatenateSplitString(1, subStrings, " ");
                //Debug.Log(firstItem);
                //Debug.Log(remainder);
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
                        createBool(remainder);
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
                        //Debug.Log("Not A Variable");
                        if(firstItem == "printf")
                        {
                            SimPrintf(remainder);
                        }
                        else if(firstItem == "for")
                        {
                            //Break down the contents of the brackets.
                            //Concatenate all code within the braces, store as string.
                            //Loop the coroutine run and yield based on values deciphered.

                            string newString = "printf(\"this is a for loop\");\nprintf(\"this is a for loop2\")\nprintf(\"this is a for loop3\")";

                            StartCoroutine(RunCode(sim, newString));
                            yield return new WaitUntil(() => runComplete == true);

                        }
                        else if (firstItem == "while")
                        {
                            //Break down the contents of the brackets.
                            //Concatenate all code within the braces, store as string.
                            //Loop the coroutine run and yield based on values deciphered.

                            string newString = "printf(\"this is a for loop\");\nprintf(\"this is a for loop2\")\nprintf(\"this is a for loop3\")";

                            StartCoroutine(RunCode(sim, newString));
                            yield return new WaitUntil(() => runComplete == true);

                        }
                        else if (firstItem == "if")
                        {
                            //Break down the contents of the brackets.
                            //Concatenate all code within the braces, store as string.
                            //Loop the coroutine run and yield based on values deciphered.

                            //for(int i = 0; i <= 5; i++)

                            subStrings = remainder.Split(';');
                            remainder = concatenateSplitString(1, subStrings, " ");

                            string newString = "printf(\"this is a for loop\");\nprintf(\"this is a for loop2\")\nprintf(\"this is a for loop3\")";

                            StartCoroutine(RunCode(sim, newString));
                            yield return new WaitUntil(() => runComplete == true);

                        }
                        else if (firstItem == "")
                        {
                            Debug.Log("Not an error, just some whitespace.");
                        }
                        else
                        {
                            Debug.Log("Error");
                            yield break;
                        }
                    }
                }
            }

            if(runComplete == false)
                yield return new WaitForSeconds(1.0f);
        }

        runComplete = true;
        yield break;
    }


    void SimPrintf(string remainder)
    {
        //Debug.Log("PRINTF");
        string stringToPrint = GetSimPrintString(remainder);
        //Debug.Log(stringToPrint);
        this.outputString = this.outputString + stringToPrint + '\n';
        //Debug.Log(outputString);
        password = stringToPrint;
    }

    string GetSimPrintString(string remainder)
    {
        string returnString = "";
        string[] subStrings = remainder.Split('+', ')');

        for (int i = 0; i < subStrings.Length; i++)
        {
            string[] temps = subStrings[i].Split(' ');
            string temp = concatenateSplitString(0, temps);
            Sim_Variable variable = FindSimVariable(temp, sim.variables);
            if (variable != null)
            {
                returnString += variable.value;
            }
            else
            {
                string[] subSubStrings = subStrings[i].Split('\"');
                if(subSubStrings.Length != 3)
                {
                    //Debug.Log("Error in printf, string count " + subSubStrings.Length);
                }
                else
                {
                    returnString += subSubStrings[1];
                }
            }
        }

        return returnString;
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
        string[] subStrings = remainder.Split(' ', ';');
        remainder = concatenateSplitString(1, subStrings, " ");
        Debug.Log(subStrings[2]);
        createBool(subStrings[0], subStrings[2]);

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









    public string GetPassword()
    {
        return this.password;
    }

    public string GetOutputString()
    {
        return this.outputString;
    }
}
