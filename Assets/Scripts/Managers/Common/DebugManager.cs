using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class DebugManager : Singleton<DebugManager>
{
    public bool keyDebugs = false;
    private static string GetCurrentMethodName()
    {
        // Obtener el stack trace
        StackTrace stackTrace = new();

        // Obtener el frame de la llamada actual (el frame 0 es el método GetCurrentMethodName,
        // el frame 1 es el método que llama a GetCurrentMethodName, y así sucesivamente)
        StackFrame currentFrame = stackTrace.GetFrame(1);

        // Obtener el método actual
        MethodBase method = currentFrame.GetMethod();

        // Obtener el nombre del método
        string methodName = method.Name;

        // Retornar el nombre del método
        return methodName;
    }

    public static void LogCurrentCallStack()
    {
        // Obtener el seguimiento de la pila de llamadas (stack trace)
        StackTrace stackTrace = new();

        // Recorrer cada marco en el seguimiento de la pila
        for (int i = 0; i < stackTrace.FrameCount; i++)
        {
            // Obtener el marco actual
            StackFrame frame = stackTrace.GetFrame(i);

            // Obtener el método actual en el marco
            MethodBase method = frame.GetMethod();

            // Obtener el nombre del método y la clase a la que pertenece
            string methodName = method.Name;
            string className = method.DeclaringType.FullName;

            // Mostrar el nombre del método y la clase en la consola
            Debug.Log($"Method: {methodName} - Class: {className}");
        }
    }

    public static string CurrentCallStackToString()
    {
        string stackTraceString = "";
        // Obtener el seguimiento de la pila de llamadas (stack trace)
        StackTrace stackTrace = new();

        // Recorrer cada marco en el seguimiento de la pila
        for (int i = 0; i < stackTrace.FrameCount; i++)
        {
            // Obtener el marco actual
            StackFrame frame = stackTrace.GetFrame(i);

            // Obtener el método actual en el marco
            MethodBase method = frame.GetMethod();

            // Obtener el nombre del método y la clase a la que pertenece
            string methodName = method.Name;
            string className = method.DeclaringType.FullName;

            // Mostrar el nombre del método y la clase en la consola
            stackTraceString += $"Method: {methodName} - Class: {className}\n";
        }
        return stackTraceString;
    }

    internal static void DebugDict(Dictionary<object, object> dict)
    {
        foreach (KeyValuePair<object, object> pair in dict)
            Debug.Log($"{pair.Key}:{pair.Value}");
    }

        internal static void DebugDict<TKey, TValue>(Dictionary<TKey, TValue> dict)
        where TKey : UnityEngine.Object
        where TValue : UnityEngine.Object
    {
        foreach (KeyValuePair<TKey, TValue> pair in dict)
            Debug.Log($"Debug => {pair.Key}:{pair.Value}");
    }

    internal static void DebugDict(Dictionary<string, Vector3> dict)
    {
        foreach (KeyValuePair<string, Vector3> pair in dict)
            Debug.Log($"{pair.Key}:{pair.Value}");
    }

    internal static void Log(bool condition, string ifTrue, string ifFalse)
    {
        string log = condition ? ifTrue : ifFalse;
        Debug.Log(log);
    }
    internal static string String(bool condition, string ifTrue, string ifFalse)
    {
        string log = condition ? ifTrue : ifFalse;
        return log;
    }

    internal static void CompareEquals(string s1, string s2)
    {
        Debug.Log($"{s1} Equals {s2} ? {s1.Equals(s2)}");
    }

    internal static void FreeCompare(int i1, string comparator, int i2, bool condition)
    {
        Debug.Log($"{i1} {comparator} {i2} ? {condition}");
    }


    internal static void DebugList(List<SkillBase> list, string separator)
    {
        foreach (UnityEngine.Object obj in list)
            Debug.Log($"{obj}{separator}");
    }
}
