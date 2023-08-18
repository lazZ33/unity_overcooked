using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections.Generic;

public static class HelperFunc{
    private static System.Random rng = new System.Random();

    public static T dereference<T>(NetworkObjectReference reference){
        if (!reference.TryGet(out NetworkObject networkObject)) { Debug.LogError("failed to dereference " + typeof(T).ToString()); return default(T); }
        T result = networkObject.GetComponent<T>();
        if (result == null) { Debug.LogError("failed to get " + typeof(T).ToString() + " component"); return default(T); }
        return result;
    }

    public static void Shuffle<T>(IList<T> list){
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }

    public static void LogEnumerable(IEnumerable<object> enumerable){
        string msg = "";
        foreach(object element in enumerable){
            msg += element.ToString() + " ,";
        }
        Debug.Log(msg);
    }

}