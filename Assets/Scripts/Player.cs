﻿using System.Collections;
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

    string defaultOutputString = "";
    string outputString = "";
    string password = "";

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
        StartCoroutine(RunCode(sim, inputField.text.ToString()));
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





    //////////////////////////////////////
    //////////////////////////////////////
    // Real Functions for Sim Functions //
    //////////////////////////////////////
    //////////////////////////////////////

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






    ////////////////////////////
    ////////////////////////////
    // Code Running Coroutine //
    ////////////////////////////
    ////////////////////////////

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
                        createChar(remainder);
                    }
                    else if (firstItem == "string")
                    {
                        createString(remainder);
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

                        }
                        else if (firstItem == "if")
                        {

                            Debug.Log("Entered sim if");

                            string codeBlock = "";
                            int codeBlockEnd;

                            subStrings = remainder.Split(' ', '(' , ')');

                            firstItem = subStrings[0];

                            bool conditionMet = false;

                            Sim_Variable initial = FindSimVariable(firstItem, sim.variables);
                            if(initial != null)
                            {
                                if(initial.type == "int")
                                {
                                    Debug.Log("Entered sim if 2");
                                    conditionMet = SimulatedIntComparison(firstItem, subStrings[1], subStrings[2]);
                                }
                                else if (initial.type == "bool")
                                {
                                    conditionMet = SimulatedBoolComparison(firstItem, subStrings[1], subStrings[2]);
                                }
                                else if (initial.type == "char")
                                {
                                    conditionMet = SimulatedCharComparison(firstItem, subStrings[1], subStrings[2]);
                                }
                                else if (initial.type == "string")
                                {
                                    conditionMet = SimulatedStringComparison(firstItem, subStrings[1], subStrings[2]);
                                }
                                else
                                {
                                    SimPrintError("SYNTAX ERROR: Unrecognised variable type for variable \"" + initial.name + "\"");
                                }
                            }
                            else
                            {
                                //determine is int, or other stuff
                            }

                            subStrings = CodeLines[x + 1].Split(' ', '\n');
                            firstItem = subStrings[0];
                            if(firstItem == "{")
                            {
                                bool closedBraces = false;
                                int startIndex = x + 1;
                                int endIndex = x + 1;
                                for (int i = startIndex; i < CodeLines.Length; i++)
                                {
                                    endIndex = i;
                                    subStrings = CodeLines[i].Split(' ', '\n');
                                    firstItem = subStrings[0];
                                    if (firstItem == "}")
                                    {
                                        Debug.Log(conditionMet);
                                        closedBraces = true;
                                        break;
                                    }
                                }

                                if(closedBraces)
                                {
                                    codeBlockEnd = endIndex;
                                    for(int i = startIndex; i < endIndex; i++)
                                    {
                                        codeBlock += CodeLines[i] + '\n';
                                    }

                                    if (conditionMet)
                                    {
                                        StartCoroutine(RunCode(sim, codeBlock));
                                        yield return new WaitUntil(() => runComplete == true);         
                                    }

                                    x = codeBlockEnd;

                                }
                                else
                                {
                                    //break error
                                }

                            }
                            else
                            {
                                //break error
                            }
 

                        }
                        else if (firstItem == "" || firstItem == "{" || firstItem == "}")
                        {
                            Debug.Log("Not an error, just some whitespace.");
                        }
                        else
                        {
                            SimPrintError("SYNTAX ERROR: Could not recognise symbol \"" + firstItem + "\"");
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






    IEnumerator runSimIf(string remainder, string[] CodeLines, int x)
    {
        Debug.Log("Entered sim if");

        string codeBlock = "";
        int codeBlockEnd;

        string[] subStrings = remainder.Split(' ', '(', ')');

        string firstItem = subStrings[0];

        bool conditionMet = false;

        Sim_Variable initial = FindSimVariable(firstItem, sim.variables);
        if (initial != null)
        {
            if (initial.type == "int")
            {
                Debug.Log("Entered sim if 2");
                conditionMet = SimulatedIntComparison(firstItem, subStrings[1], subStrings[2]);
            }
            else if (initial.type == "bool")
            {
                conditionMet = SimulatedBoolComparison(firstItem, subStrings[1], subStrings[2]);
            }
            else if (initial.type == "char")
            {
                conditionMet = SimulatedCharComparison(firstItem, subStrings[1], subStrings[2]);
            }
            else if (initial.type == "string")
            {
                conditionMet = SimulatedStringComparison(firstItem, subStrings[1], subStrings[2]);
            }
            else
            {
                SimPrintError("SYNTAX ERROR: Unrecognised variable type for variable \"" + initial.name + "\"");
            }
        }
        else
        {
            //determine is int, or other stuff
        }

        subStrings = CodeLines[x + 1].Split(' ', '\n');
        firstItem = subStrings[0];
        if (firstItem == "{")
        {
            bool closedBraces = false;
            int startIndex = x + 1;
            int endIndex = x + 1;
            for (int i = startIndex; i < CodeLines.Length; i++)
            {
                endIndex = i;
                subStrings = CodeLines[i].Split(' ', '\n');
                firstItem = subStrings[0];
                if (firstItem == "}")
                {
                    Debug.Log(conditionMet);
                    closedBraces = true;
                    break;
                }
            }

            if (closedBraces)
            {
                codeBlockEnd = endIndex;
                for (int i = startIndex; i < endIndex; i++)
                {
                    codeBlock += CodeLines[i] + '\n';
                }

                if (conditionMet)
                {
                    StartCoroutine(RunCode(sim, codeBlock));
                    yield return new WaitUntil(() => runComplete == true);
                }

                x = codeBlockEnd;
            }
            else
            {
                //break error
            }

        }
        else
        {
            //break error
        }
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











    /////////////////////////////
    /////////////////////////////
    // Sim Variable Operations //
    /////////////////////////////
    /////////////////////////////

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
            //Debug.Log(remainder);
            //Debug.Log("BEFORE: " + var.value);
            var.value = recursiveIntOperations(remainder).ToString();
            //Debug.Log("AFTER: " + var.value);
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
                if (!int.TryParse(variable.value, out returnVal))
                {
                    //break error
                }
            }
            else
            {
                Debug.Log("Variable is not an integer");
                return 0;
            }
        }
        else if (!int.TryParse(varID, out returnVal))
        {
            //break error
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





    /////////////////////////////////////
    /////////////////////////////////////
    // Sim Variable Creation Functions //
    /////////////////////////////////////
    /////////////////////////////////////

    string createInt(string remainder)
    {

        string[] subStrings = remainder.Split(' ');
        remainder = concatenateSplitString(1, subStrings, " ");
        //Debug.Log(remainder);
        Sim_Variable newInt = new Sim_Variable(subStrings[0], "int", "0");
        performIntOperations(newInt, remainder);
        //Debug.Log(newInt.ToString());

        createInt(newInt.name, newInt.value);

        return "";
    }

    string createBool(string remainder)
    {
        string[] subStrings = remainder.Split(' ', ';');
        Debug.Log(subStrings[2]);
        createBool(subStrings[0], subStrings[2]);
        return "";
    }

    string createString(string remainder)
    {
        string[] subStrings = remainder.Split(' ', ';');
        Debug.Log(subStrings[2]);

        string name = subStrings[0];
        string value = subStrings[2];
        int length = value.Length;

        if (value[0] == '\"' && value[length-1] == '\"')
        {
            Debug.Log("Before" + value);
            value.Trim('"');
            Debug.Log("After" + value);
            createString(name, value.ToString());
        }
        else
        {
            //error
        }

        return "";
    }

    string createChar(string remainder)
    {
        string[] subStrings = remainder.Split(' ', ';');
        //remainder = concatenateSplitString(1, subStrings, " ");
        Debug.Log(subStrings[2]);

        string name = subStrings[0];
        string value = subStrings[2];

        if (value.Length == 3)
        {
            if (value[0] == '\'' && value[2] == '\'')
            {
                createChar(name, value[1].ToString());
            }
            else
            {
                //error
            }
        }
        else
        {
            //error
        }

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
        Sim_Variable newString = new Sim_Variable(name, "string", value);
        simVariables.Add(newString);
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
    




    //////////////////////////////
    //////////////////////////////
    // Sim Variable Comparisons //
    //////////////////////////////
    //////////////////////////////

    bool SimulatedIntComparison(string val1, string comparisonOperator, string val2)
    {
        int int1, int2;

        Sim_Variable var1 = FindSimVariable(val1, sim.variables);
        if (var1 != null)
        {
            if (!int.TryParse(var1.value, out int1))
            {
                //should not be possible
            }
        }
        else
        {
            if (int.TryParse(val1, out int1))
            {

            }
            else
            {
                //break error
            }
        }

        Sim_Variable var2 = FindSimVariable(val2, sim.variables);
        if (var2 != null)
        {
            if (!int.TryParse(var2.value, out int2))
            {
                //should not be possible
            }

        }
        else
        {
            if (int.TryParse(val2, out int2))
            {

            }
            else
            {
                //break error
            }
        }

        if (comparisonOperator == "==")
        {
            if (int1 == int2)
                return true;
        }
        else if (comparisonOperator == "<")
        {
            if (int1 < int2)
                return true;
        }
        else if (comparisonOperator == ">")
        {
            if (int1 > int2)
                return true;
        }
        else if (comparisonOperator == "<=" || comparisonOperator == "=<")
        {
            if (int1 <= int2)
                return true;
        }
        else if (comparisonOperator == ">=" || comparisonOperator == "=>")
        {
            if (int1 >= int2)
                return true;
        }
        else
        {
            Debug.Log("Errrror");
        }

        return false;
    }



    bool SimulatedBoolComparison(string val1, string comparisonOperator, string val2)
    {

        Sim_Variable var1 = FindSimVariable(val1, sim.variables);
        if (var1 != null)
        {
            val1 = var1.value;
        }
        else
        {
            if (val1 != "true" || val1 != "false")
            {
                //break error
            }
        }

        Sim_Variable var2 = FindSimVariable(val2, sim.variables);
        if (var2 != null)
        {
            val2 = var2.value;
        }
        else
        {
            if (val2 != "true" || val2 != "false")
            {
                //break error
            }
        }

        if (comparisonOperator == "==")
        {
            if (val1 == val2)
                return true;
        }
        else
        {
            Debug.Log("Errrror");
        }

        return false;
    }



    bool SimulatedCharComparison(string val1, string comparisonOperator, string val2)
    {

        Sim_Variable var1 = FindSimVariable(val1, sim.variables);
        if (var1 != null)
        {
            val1 = var1.value;
        }
        else
        {
            //if the char is contained by ' '
                //remove them
                //check length
            //else
                //break error

            if (val1.Length != 1)
            {
                //break error
            }
        }

        Sim_Variable var2 = FindSimVariable(val2, sim.variables);
        if (var2 != null)
        {
            val2 = var2.value;
        }
        else
        {
            if (val2 != "true" || val2 != "false")
            {
                //break error
            }
        }

        if (comparisonOperator == "==")
        {
            if (val1 == val2)
                return true;
        }
        else
        {
            Debug.Log("Errrror");
        }

        return false;
    }



    bool SimulatedStringComparison(string val1, string comparisonOperator, string val2)
    {

        Sim_Variable var1 = FindSimVariable(val1, sim.variables);
        if (var1 != null)
        {
            val1 = var1.value;
        }
        else
        {
            //if the string is contained by " "
            //remove them
            //else
            //break error

        }

        Sim_Variable var2 = FindSimVariable(val2, sim.variables);
        if (var2 != null)
        {
            val2 = var2.value;
        }
        else
        {
            if (val2 != "true" || val2 != "false")
            {
                //break error
            }
        }

        if (comparisonOperator == "==")
        {
            if (val1 == val2)
                return true;
        }
        else
        {
            Debug.Log("Errrror");
        }

        return false;
    }





    ////////////////////
    ////////////////////
    // Misc Functions //
    ////////////////////
    ////////////////////

    public string GetPassword()
    {
        return this.password;
    }

    public string GetOutputString()
    {
        return this.outputString;
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

    public void SimPrintError(string error)
    {
        this.outputString = this.outputString + error + '\n';
        StopAllCoroutines();
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

}





