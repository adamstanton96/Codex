using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.CSharp;
using System;

public class Sim_Function
{

    public Sim_Function(string name, string parameterList, string returnType, string comment, Func<string, string> funcToCall)
    {
        this.name = name;
        this.parameterList = parameterList;
        this.returnType = returnType;
        this.funcToCall = funcToCall;
        this.comment = comment;
    }

    public string name;
    public string parameterList;
    public string returnType;
    public string comment;

    Func<string, string> funcToCall;

    public string Call(string args)
    {
        string returnVal = funcToCall(args);
        return returnVal;
    }

    public override string ToString()
    {
        string returnStr = returnType + " " + name + "(" + parameterList + ")" + "\t" + comment + "\n";
        return returnStr;
    }

}
