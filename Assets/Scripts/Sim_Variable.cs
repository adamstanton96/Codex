using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sim_Variable
{
    public Sim_Variable(string name, string type, string value)
    {
        this.name = name;
        this.type = type;
        this.value = value;
        this.comment = "";
    }

    public Sim_Variable(string name, string type, string value, string comment)
    {
        this.name = name;
        this.type = type;
        this.value = value;
        this.comment = comment;
    }

    public string name;
    public string type;
    public string value;
    public string comment;

    public override string ToString()
    {
        string returnStr = type + " " + name + " = " + value + "\t" + comment + "\n";
        return returnStr;
    }

}
