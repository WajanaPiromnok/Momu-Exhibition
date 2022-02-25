using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using System;

[Serializable]
public class AvatarOutlook {
    public const string key = "outlook";
    public List<int> values = new List<int>{0,0,0,0,0,0,0,0,0,0}; // 10 numbers
    public AvatarOutlook(){}
    static public AvatarOutlook FromPlayFab(Dictionary<string,UserDataRecord> data) {
        if (String.IsNullOrEmpty(data[key].Value))
            return new AvatarOutlook();

        return JsonUtility.FromJson<AvatarOutlook>(data[key].Value);
    }
    static public bool operator ==(AvatarOutlook a, AvatarOutlook b){
        if(!(a is null) && !(b is null)) {
            return JsonUtility.ToJson(a) == JsonUtility.ToJson(b);
        }
        return a is null && b is null;
    }

    static public bool operator !=(AvatarOutlook a, AvatarOutlook b){
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

[Serializable]
public class PlayerLastPosition {
    public const string key = "position";
    public Vector3 pos;
    public Vector3 rot;
    public PlayerLastPosition(){}
    public PlayerLastPosition(Vector3 p, Vector3 r) { this.pos=p; this.rot=r; }
    static public PlayerLastPosition FromPlayFab(Dictionary<string,UserDataRecord> data){
        if (String.IsNullOrEmpty(data[key].Value))
            return new PlayerLastPosition();
        return JsonUtility.FromJson<PlayerLastPosition>(data[key].Value);
    }

    static public bool operator ==(PlayerLastPosition a, PlayerLastPosition b){
        if(!(a is null) && !(b is null)) {
            return JsonUtility.ToJson(a) == JsonUtility.ToJson(b);
        }
        return a is null && b is null;
    }

    static public bool operator !=(PlayerLastPosition a, PlayerLastPosition b){
        return !(a == b);
    }

    public bool IsDefault() {
        return this == new PlayerLastPosition();
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

[Serializable]
public class ServerDetails {
    public const string key = "serverDetails";
    public List<string> list = new List<string>();

    static public ServerDetails FromPlayFab(Dictionary<string,string> data) {
        // ServerDetails newObj = new ServerDetails();
        // newObj.list.Add("A");
        // newObj.list.Add("B");
        // Debug.Log(JsonUtility.ToJson(newObj));
        // Debug.Log(data[key]);
        // newObj.list = JsonUtility.FromJson<List<string>>(data[key]);
        // return newObj;

        return JsonUtility.FromJson<ServerDetails>(data[key]);
    }

    public int serverCount() {
        return list.Count;
    }

    public string getServerAddress(int i) {
        return list[i].Split(':')[0];
    }

    public ushort getServerPort(int i){
        return ushort.Parse(list[i].Split(':')[1]);
    }
}