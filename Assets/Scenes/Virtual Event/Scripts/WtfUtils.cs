using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;
class WtfUtils {
    static  public string ListToString(List<string> list) {
        return String.Join(", ", list.ToArray());
    }

    static public string GenerateRandomDigitsStr(int n) {
        string result = "";
        for (int i = 0; i < n; i++)
            result += UnityEngine.Random.Range(0, 10).ToString("0");
        return result;
    }

    static public Transform GetRandomSpawnTransform() {
        int index = UnityEngine.Random.Range(0, NetworkManager.startPositions.Count);
        return NetworkManager.startPositions[index];
    }

    public static string GetArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return null;
    }
}
