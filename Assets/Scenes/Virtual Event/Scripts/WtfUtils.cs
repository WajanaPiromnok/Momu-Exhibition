using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

[System.Serializable]
public class TimeRangeStrings
{
    public string startTimeStr = "hh:mm:ss";
    public string endTimeStr = "hh:mm:ss";
    public DateTime GetStartDateTime() {
        DateTime nowInThai = WtfUtils.GetThaiDateTimeNow();
        return new DateTime(
            nowInThai.Year,
            nowInThai.Month,
            nowInThai.Day,
            startHour(),
            startMin(),
            startSec()
        );
    }
    public DateTime GetEndDateTime() {
        DateTime nowInThai = WtfUtils.GetThaiDateTimeNow();
        return new DateTime(
            nowInThai.Year,
            nowInThai.Month,
            nowInThai.Day,
            endHour(),
            endMin(),
            endSec()
        );
    }

    public int SecondsSinceStart(){
        DateTime nowInThai = WtfUtils.GetThaiDateTimeNow();
        int secondsInThai = nowInThai.Hour*60*60 + nowInThai.Minute*60 + nowInThai.Second;
        int secondsStart = startHour()*60*60 + startMin()*60 + startSec();
        return secondsInThai - secondsStart;
    }

    public bool IsInRange() {
        DateTime currentDateTime = WtfUtils.GetThaiDateTimeNow();
        DateTime startDateTime = GetStartDateTime();
        DateTime endDateTime = GetEndDateTime();

        if(currentDateTime >= startDateTime && currentDateTime < endDateTime)
            return true;

        return false;
    }
    
    private int startHour() { return int.Parse(startTimeStr.Split(new char[]{':'})[0]); }
    private int startMin() { return int.Parse(startTimeStr.Split(new char[]{':'})[1]); }
    private int startSec() { return int.Parse(startTimeStr.Split(new char[]{':'})[2]); }
    private int endHour() { return int.Parse(endTimeStr.Split(new char[]{':'})[0]); }
    private int endMin() { return int.Parse(endTimeStr.Split(new char[]{':'})[1]); }
    private int endSec() { return int.Parse(endTimeStr.Split(new char[]{':'})[2]); }
}

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

     public static void SetLayerRecursively(GameObject go, int layerNumber)
    {
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }

    public static void RevealCursor() {
        Cursor.lockState = CursorLockMode.None;
    }

    public static DateTime GetThaiDateTimeNow() {
        DateTime result = DateTime.UtcNow.AddHours(7);
        // Debug.Log(result.ToString());
        return result;
        // return TimeZoneInfo.ConvertTime (DateTime.Now,
        // TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
    }

    public static bool IsServerBuild() {
        #if UNITY_SERVER
            return true;
        #endif
        return false;
    }

    public static bool IsWebGL() {
        #if UNITY_WEBGL
            return true;
        #endif
        return false;
    }

    public static bool IsEditor() {
        #if UNITY_EDITOR
            return true;
        #endif
        return false;
    }

    public static bool IsOSX() {
        #if UNITY_STANDALONE_OSX
            return true;
        #endif
        return false;
    }

    public static bool IsWindows() {
        #if UNITY_STANDALONE_WIN
            return true;
        #endif
        return false;
    }
}
