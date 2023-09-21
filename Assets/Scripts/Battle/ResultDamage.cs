using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class ResultDamage
{
    private int result;
    private bool critic, miss;

    public int Result => result;
    public bool Critic => critic;
    public bool Miss => miss;

    public string DebugToString(){
        string info = $"Miss? = {miss}, Result = {result}, Critical? = {critic}";
        return info;
    }
    
    public override string ToString(){
        return miss ? "miss" : result.ToString();
    }

    internal void setCrit(bool v)
    {
        critic = v;
    }

    internal void setMiss(bool v)
    {
        this.miss = v;
    }

    internal void setResult(int res)
    {
        this.result = res;
    }
}
