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

        Sim_Function _Move = new Sim_Function("Move", "string", "void", "//Moves the player in the direction passed to the function.", Move);
        simFunctions.Add(_Move);

        Sim_Function _GetCollectiblesHeld = new Sim_Function("GetCollectiblesHeld", "", "int", "//Returns a value equal to the number of collectibles held by the player.", GetCollectiblesHeld);
        simFunctions.Add(_GetCollectiblesHeld);

        Sim_Function _GetMapDirection = new Sim_Function("GetMapDirection", "", "string", "//Returns a string pointing out the direction of the correct path (North, South, East, West).", GetMapDirection);
        simFunctions.Add(_GetMapDirection);
        /*
        Sim_Function _NextStage = new Sim_Function("NextStage", "", "void", "//For Debug.", NextStage);
        simFunctions.Add(_NextStage);
        */
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
        if(stage < stages.Count)
            FullReset();
        else
            s.LoadScene("MainMenu");
    }


    public void DisplayHint()
    {
        SimPrintStatement(stages[stage].GetHint());
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

    string Move(string arg)
    {
        string value = "";
        string[] temp = arg.Split(',', ')');
        //if (temp.Length != 1)
        //{
        //    Debug.Log(temp[1]);
        //    SimPrintError("SYNTAX ERROR: Incorrect number of parameters for function Move(), 1 expected - " + temp.Length + " provided");
            //break error
        //}
        //else
        //{
            arg = temp[0];
        //}

        Debug.Log(arg);

        Sim_Function function = FindSimFunction(arg, simFunctions);
        if (function != null)
        {
            if (function.returnType == "string")
            {
                value = function.Call("");
            }
        }
        else
        {
            //get the value of the variable.
            Sim_Variable variable = FindSimVariable(arg, simVariables);
            if (variable != null)
            {
                if (variable.type == "string")
                {
                    value = variable.value;
                }
                else
                {
                    Debug.Log("Variable is not a string");
                }
            }
            else if (arg[0] == '\"' && arg[arg.Length - 1] == '\"')
            {
                string valString = arg.Trim('\"');
                value = valString;
            }
            else
            {
                Debug.Log(value + " is an invalid value for a string variable.");
                //break error
            }
        }

        value = value.ToUpper();

        if (value == "NORTH")
            t.position = new Vector3(t.position.x, t.position.y, t.position.z + 1.0f);
        else if (value == "SOUTH")
            t.position = new Vector3(t.position.x, t.position.y, t.position.z - 1.0f);
        else if (value == "EAST")
            t.position = new Vector3(t.position.x + 1.0f, t.position.y, t.position.z);
        else if (value == "WEST")
            t.position = new Vector3(t.position.x - 1.0f, t.position.y, t.position.z);
        else
            SimPrintStatement("MINOR LOGIC ERROR: The value passed to Move() is invalid, must correspond to a direction.");
            //break 
        return "null";
    }

    string GetMapDirection(string arg)
    {
        return stages[stage].MapDirection;
    }

    string GetCollectiblesHeld(string arg)
    {
        return collectiblesHeld.ToString();
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
        
        if(CodeLines.Length == 0)
        {
            SimPrintError("SYNTAX ERROR: No code provided to run. Please input code to the code window.");
        }

        for (int x = 0; x < CodeLines.Length; x++)
        {
            //Inform the player that a line of code is being currently run...
            runComplete = false;

            //Split the line of code and determine the first item and remainder of the line...
            string firstItem = "", remainder = "";
            string[] subStrings = CodeLines[x].Split('(', ' ');
            if (subStrings.Length >= 1)
            {
                firstItem = subStrings[0];
            }
            if (subStrings.Length > 1)
            {
                remainder = concatenateSplitString(1, subStrings, " ");
            }

            //Check if the first item references a call to an existing function or method...
            Sim_Function function = FindSimFunction(firstItem, sim.functions);
            if (function != null)
            {
                function.Call(remainder);
            }
            else
            {
                //Check if the first item references an existing variable...
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
                        SimPrintError("SYNTAX ERROR: Unrecognised variable type for variable \"" + variable.name + "\"");
                    }
                }
                else
                {
                    //Check if the first item references an attempt to create a new primitive variable...
                    if (firstItem == "int")
                    {
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
                        //Check if the first item refers to a system function or structure (printf, for, do, while, if, etc.)...
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

                            Debug.Log("Entered sim while");

                            string codeBlock = "";
                            int codeBlockEnd;

                            subStrings = remainder.Split(' ', '(', ')');

                            firstItem = subStrings[0];

                            bool conditionMet = RunSimComparisons(subStrings);

                            subStrings = CodeLines[x + 1].Split(' ', '\n');
                            firstItem = subStrings[0];
                            Debug.Log(firstItem);
                            if (firstItem == "{")
                            {
                                //Debug.Log("{");
                                bool closedBraces = false;
                                int intialLineIndex = x;
                                int startIndex = x + 1;
                                int endIndex = x + 1;
                                int nestedLevel = 0;
                                for (int i = startIndex; i < CodeLines.Length; i++)
                                {
                                    //Debug.Log(CodeLines[i]);
                                    endIndex = i;
                                    subStrings = CodeLines[i].Split(' ', '\n');
                                    firstItem = subStrings[0];
                                    if (firstItem == "{")
                                    {
                                        nestedLevel++;
                                    }

                                    if (firstItem == "}")
                                    {
                                        if (nestedLevel == 1)
                                        {
                                            //Debug.Log("}");
                                            closedBraces = true;
                                            break;
                                        }
                                        else
                                        {
                                            nestedLevel--;
                                        }
                                    }
                                }

                                if (closedBraces)
                                {
                                    codeBlockEnd = endIndex;
                                    for (int i = startIndex; i < endIndex; i++)
                                    {
                                        codeBlock += CodeLines[i] + '\n';
                                    }
                                    Debug.Log(conditionMet);
                                    if (conditionMet)
                                    {
                                        StartCoroutine(RunCode(sim, codeBlock));
                                        yield return new WaitUntil(() => runComplete == true);
                                        x = intialLineIndex;
                                        string loopBlock = CodeLines[x] + '\n' + codeBlock + "}";
                                        Debug.Log(loopBlock);
                                        StartCoroutine(RunCode(sim, loopBlock));
                                        yield return new WaitUntil(() => runComplete == true);
                                    }

                                    x = codeBlockEnd;

                                }
                                else
                                {
                                    SimPrintError("SYNTAX ERROR: 1");
                                }

                            }
                            else
                            {
                                SimPrintError("SYNTAX ERROR: 2");
                            }



                        }
                        else if (firstItem == "if")
                        {

                            Debug.Log("Entered sim if");

                            string codeBlock = "";
                            string elseBlock = "";
                            int codeBlockEnd;

                            subStrings = remainder.Split(' ', '(' , ')');

                            firstItem = subStrings[0];

                            bool conditionMet = RunSimComparisons(subStrings);
   
                            subStrings = CodeLines[x + 1].Split(' ', '\n');
                            firstItem = subStrings[0];
                            Debug.Log(firstItem);
                            if (firstItem == "{")
                            {
                                Debug.Log("{");
                                bool closedBraces = false;
                                int startIndex = x + 1;
                                int endIndex = x + 1;
                                int nestedLevel = 0;
                                for (int i = startIndex; i < CodeLines.Length; i++)
                                {
                                    Debug.Log(CodeLines[i]);
                                    endIndex = i;
                                    subStrings = CodeLines[i].Split(' ', '\n');
                                    firstItem = subStrings[0];
                                    if (firstItem == "{")
                                    {
                                        nestedLevel++;
                                    }

                                    if (firstItem == "}")
                                    {
                                        if (nestedLevel == 1)
                                        {
                                            Debug.Log("}");
                                            closedBraces = true;
                                            break;
                                        }
                                        else
                                        {
                                            nestedLevel--;
                                        }
                                    }
                                }

                                if (closedBraces)
                                {
                                    codeBlockEnd = endIndex;
                                    for (int i = startIndex; i < endIndex; i++)
                                    {
                                        codeBlock += CodeLines[i] + '\n';
                                    }

                                    Debug.Log(codeBlock);

                                    if (CodeLines[codeBlockEnd + 1] == "else")
                                    {
                                        startIndex = codeBlockEnd + 2;
                                        nestedLevel = 0;
                                        closedBraces = false;

                                        for (int i = startIndex; i < CodeLines.Length; i++)
                                        {
                                            Debug.Log(CodeLines[i]);
                                            endIndex = i;
                                            subStrings = CodeLines[i].Split(' ', '\n');
                                            firstItem = subStrings[0];
                                            if (firstItem == "{")
                                            {
                                                nestedLevel++;
                                            }

                                            if (firstItem == "}")
                                            {
                                                if (nestedLevel == 1)
                                                {
                                                    Debug.Log("}");
                                                    closedBraces = true;
                                                    break;
                                                }
                                                else
                                                {
                                                    nestedLevel--;
                                                }
                                            }
                                        }

                                        if (closedBraces)
                                        {
                                            Debug.Log(endIndex);
                                            codeBlockEnd = endIndex;
                                            for (int i = startIndex; i < endIndex; i++)
                                            {
                                                elseBlock += CodeLines[i] + '\n';
                                            }

                                            Debug.Log(elseBlock);

                                            Debug.Log(conditionMet);
                                            if (conditionMet)
                                            {
                                                StartCoroutine(RunCode(sim, codeBlock));
                                                yield return new WaitUntil(() => runComplete == true);
                                            }
                                            else
                                            {
                                                StartCoroutine(RunCode(sim, elseBlock));
                                                yield return new WaitUntil(() => runComplete == true);
                                            }
                                        }
                                        else
                                        {
                                            SimPrintError("SYNTAX ERROR: Missing brace/s around else statement code.");
                                        }
                                    }
                                    else
                                    {
                                        Debug.Log(conditionMet);
                                        if (conditionMet)
                                        {
                                            StartCoroutine(RunCode(sim, codeBlock));
                                            yield return new WaitUntil(() => runComplete == true);
                                        }
                                    }


                                    x = codeBlockEnd;

                                }
                                else
                                {
                                    SimPrintError("SYNTAX ERROR: Missing brace/s around if statement code.");
                                }
                            }
                            else
                            {
                                SimPrintError("SYNTAX ERROR: 2");
                            }
                        }
                        //Check if the first item is whitespace or an unused character...
                        else if (firstItem == "" || firstItem == "{" || firstItem == "}" || firstItem == "//")
                        {
                            Debug.Log("Not an error, just some whitespace.");
                        }
                        //If first item is none of these, it is invalid and an error is thrown...
                        else
                        {
                            SimPrintError("SYNTAX ERROR: Could not recognise symbol \"" + firstItem + "\"");
                        }
                    }
                }
            }

            //If a code line is running, suspend running the next line for effect...
            if(runComplete == false)
                yield return new WaitForSeconds(1.0f);
        }

        //Inform the player that the run is complete then break from the coroutine...
        runComplete = true;
        yield break;
    }





    bool RunSimComparisons(string [] subStrings)
    {
        bool returnVal = false;

        Sim_Variable initial = FindSimVariable(subStrings[0], sim.variables);
        if (initial != null)
        {
            if (initial.type == "int")
            {
                //Debug.Log("Entered sim if 2");
                returnVal = SimulatedIntComparison(subStrings[0], subStrings[1], subStrings[2]);
                //Debug.Log(returnVal);
            }
            else if (initial.type == "bool")
            {
                returnVal = SimulatedBoolComparison(subStrings[0], subStrings[1], subStrings[2]);
            }
            else if (initial.type == "char")
            {
                returnVal = SimulatedCharComparison(subStrings[0], subStrings[1], subStrings[2]);
            }
            else if (initial.type == "string")
            {
                returnVal = SimulatedStringComparison(subStrings[0], subStrings[1], subStrings[2]);
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
        return returnVal;
    }




    void SimPrintf(string remainder)
    {
        string stringToPrint = GetSimPrintString(remainder);
        this.outputString = this.outputString + stringToPrint + '\n';

        password = stringToPrint;
    }

    string GetSimPrintString(string remainder)
    {
        string returnString = "";
        string[] subStrings = remainder.Split('+', ')', ';');

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
                    //SimPrintError("SYNTAX ERROR: Invalid parameters for printf.");
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
        string[] subStrings = name.Split('(', ' ', ')');
        name = subStrings[0];

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
                SimPrintError("LOGIC ERROR: Cannot divide by zero.");
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

        while (remainder[remainder.Length - 1] == ' ')
            remainder = remainder.TrimEnd(' ');

        Debug.Log(remainder);

        if (subStrings[0] == "=")
        {
            string value = "";

            Sim_Function function = FindSimFunction(remainder, simFunctions);
            if (function != null)
            {
                if (function.returnType == "string")
                {
                    value = function.Call("");
                }
            }
            else
            {
                //get the value of the variable.
                Sim_Variable variable = FindSimVariable(remainder, simVariables);
                if (variable != null)
                {
                    if (variable.type == "string")
                    {
                        value = variable.value;
                    }
                    else
                    {
                        Debug.Log("Variable is not a string");
                    }
                }
                else if (remainder[0] == '\"' && remainder[remainder.Length - 1] == '\"')
                {
                    string valString = remainder.Trim('\"');
                    value = valString;
                }
                else
                {
                    Debug.Log(remainder + " is an invalid value for a character variable.");
                    //break error
                }
            }

            Debug.Log(remainder);
            Debug.Log("BEFORE: " + var.value);
            var.value = value;
            Debug.Log("AFTER: " + var.value);

        }

        return "";
    }



    string performCharOperations(Sim_Variable var, string remainder)
    {
        string[] subStrings = remainder.Split(' ', ';');
        remainder = concatenateSplitString(1, subStrings, " ");

        while (remainder[remainder.Length - 1] == ' ')
            remainder = remainder.TrimEnd(' ');

        if (subStrings[0] == "=")
        {
            string value = "";

            Sim_Function function = FindSimFunction(remainder, simFunctions);
            if (function != null)
            {
                if (function.returnType == "char")
                {
                    value = function.Call("");
                }
                else
                {
                    //break error
                }
            }
            else
            {
                //get the value of the variable.
                Sim_Variable variable = FindSimVariable(remainder, simVariables);
                if (variable != null)
                {
                    if (variable.type == "char")
                    {
                        value = variable.value;
                    }
                    else
                    {
                        //break error
                        Debug.Log("Variable is not an character");
                    }
                }
                else if (remainder.Length == 3)
                {
                    if (remainder[0] == '\'' && remainder[0] == '\'')
                    {
                        value = remainder[1].ToString();
                    }
                    else
                    {
                        //break error
                    }
                }
                else
                {
                    Debug.Log(remainder + " is an invalid value for a character variable.");
                    //break error
                }
            }

            Debug.Log(remainder);
            Debug.Log("BEFORE: " + var.value);
            var.value = value;
            Debug.Log("AFTER: " + var.value);

        }
        
        return "";
    }



    string performBoolOperations(Sim_Variable var, string remainder)
    {
        string[] subStrings = remainder.Split(' ', ';');
        remainder = concatenateSplitString(1, subStrings, " ");

        while(remainder[remainder.Length - 1] == ' ')
        {
            remainder = remainder.TrimEnd(' ');
        }

        if (subStrings[0] == "=")
        {
            string value = "";

            Sim_Function function = FindSimFunction(remainder, simFunctions);
            if (function != null)
            {
                if (function.returnType == "bool")
                {
                    value = function.Call("");
                }
                else
                {
                    //break error
                }
            }
            else
            {
                //get the value of the variable.
                Sim_Variable variable = FindSimVariable(remainder, simVariables);
                if (variable != null)
                {
                    if (variable.type == "bool")
                    {
                        value = variable.value;
                    }
                    else
                    {
                        Debug.Log("Variable is not an integer");
                    }
                }
                else if (remainder == "true")
                {
                    value = "true";
                }
                else if (remainder == "false")
                {
                    value = "false";
                }
                else
                {
                    Debug.Log(remainder + " is an invalid value for a boolean variable.");
                    //break error
                }
            }



            Debug.Log(remainder);
            Debug.Log("BEFORE: " + var.value);
            var.value = value;
            Debug.Log("AFTER: " + var.value);
        }
        else
        {
            //break error
        }

        return "";
    }


    //var1 = var2 + 12 + var3 - 6;
    //var1 = recursiveInt( var2 + 12 + var3 - 6;)

    int recursiveIntOperations(string items)
    {
        int returnVal = 0;

        string[] subStrings = items.Split(' ', ';');
        string varID = subStrings[0];

        Sim_Function function = FindSimFunction(varID, simFunctions);
        if (function != null)
        {
            if (function.returnType == "int")
            {
                string funcVal = function.Call("");
                if (!int.TryParse(funcVal, out returnVal))
                {
                    //break error
                }
            }
        }
        else
        { 
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
        remainder = concatenateSplitString(1, subStrings, " ");
        Debug.Log(subStrings[2]);

        Sim_Variable newBool = new Sim_Variable(subStrings[0], "bool", "false");
        Debug.Log(remainder);
        performBoolOperations(newBool, remainder);

        createBool(newBool.name, newBool.value);
        //createBool(subStrings[0], subStrings[2]);
        return "";
    }

    string createString(string remainder)
    {
        string[] subStrings = remainder.Split(' ');
        remainder = concatenateSplitString(1, subStrings, " ");
        //Debug.Log(remainder);
        Sim_Variable newString = new Sim_Variable(subStrings[0], "string", "");
        performStringOperations(newString, remainder);
        //Debug.Log(newInt.ToString());

        createString(newString.name, newString.value);

        /*
        string[] subStrings = remainder.Split(' ', ';');
        Debug.Log(subStrings[2]);

        string name = subStrings[0];
        string value = subStrings[2];
        int length = value.Length;

        if (value[0] == '\"' && value[length-1] == '\"')
        {
            Debug.Log("Before" + value);
            value = value.Trim('\"');
            Debug.Log("After" + value);
            createString(name, value.ToString());
        }
        else
        {
            //error
        }
        */
        return "";
    }

    string createChar(string remainder)
    {
        string[] subStrings = remainder.Split(' ');
        remainder = concatenateSplitString(1, subStrings, " ");
        //Debug.Log(remainder);
        Sim_Variable newChar = new Sim_Variable(subStrings[0], "char", "");
        performCharOperations(newChar, remainder);
        //Debug.Log(newInt.ToString());

        createChar(newChar.name, newChar.value);

        /*
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
        */
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
        Debug.Log(value);
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

    public void SimPrintStatement(string statement)
    {
        this.outputString = this.outputString + statement + '\n';
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





/*Debug.Log("Entered sim if");

                            string codeBlock = "";
int codeBlockEnd;

subStrings = remainder.Split(' ', '(' , ')');

                            firstItem = subStrings[0];

                            bool conditionMet = RunSimComparisons(subStrings);

subStrings = CodeLines[x + 1].Split(' ', '\n');
firstItem = subStrings[0];
                            Debug.Log(firstItem);
                            if(firstItem == "{")
                            {
                                Debug.Log("{");
                                bool closedBraces = false;
int startIndex = x + 1;
int endIndex = x + 1;
int nestedLevel = 0;
                                for (int i = startIndex; i<CodeLines.Length; i++)
                                {
                                    Debug.Log(CodeLines[i]);
                                    endIndex = i;
                                    subStrings = CodeLines[i].Split(' ', '\n');
firstItem = subStrings[0];
                                    if(firstItem == "{")
                                    {
                                        nestedLevel++;
                                    }

                                    if (firstItem == "}")
                                    {
                                        if (nestedLevel == 1)
                                        {
                                            Debug.Log("}");
                                            closedBraces = true;
                                            break;
                                        }
                                        else
                                        {
                                            nestedLevel--;
                                        }
                                    }
                                }

                                if(closedBraces)
                                {
                                    codeBlockEnd = endIndex;
                                    for(int i = startIndex; i<endIndex; i++)
                                    {
                                        codeBlock += CodeLines[i] + '\n';
                                    }
                                    Debug.Log(conditionMet);
                                    if (conditionMet)
                                    {
                                        StartCoroutine(RunCode(sim, codeBlock));
yield return new WaitUntil(() => runComplete == true);         
                                    }

                                    x = codeBlockEnd;

                                }
                                else
                                {
                                    SimPrintError("SYNTAX ERROR: 1");
                                }

                            }
                            else
                            {
                                SimPrintError("SYNTAX ERROR: 2");
                            }
                            */