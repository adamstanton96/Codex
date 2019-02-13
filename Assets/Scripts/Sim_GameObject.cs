using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sim_GameObject
{
    public Sim_GameObject(string name, List<Sim_Function> functions, List<Sim_Variable> variables)
    {
        this.name = name;
        this.functions = functions;
        this.variables = variables;
    }

    public string name;
    public List<Sim_Function> functions;
    public List<Sim_Variable> variables;

    public override string ToString()
    {
        string returnStr = "Name: " + name + "\n\nFunctions:\n";
        for (int i = 0; i < functions.Count; i++)
        {
            returnStr += functions[i].ToString();
        }

        returnStr += "\n\nVariables:\n";
        for (int i = 0; i < variables.Count; i++)
        {
            returnStr += variables[i].ToString();
        }
        return returnStr;
    }

    public void CallAll()
    {
        functions[0].Call("");
    }

}

