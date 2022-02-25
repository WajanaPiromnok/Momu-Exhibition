using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using System;

[Serializable]
public class AvatarOutlook {
    public static List<string> keys = new List<string>{"outlook_baseModel", "outlook_top", "outlook_body", "outlook_bottom"};
    public int baseModel = 0;
    public int top = 0;
    public int body = 0;
    public int bottom = 0;
    public AvatarOutlook(){}
    public AvatarOutlook(int baseModel, int top, int body, int bottom){
        this.baseModel = baseModel;
        this.top = top;
        this.body = body;
        this.bottom = bottom;
    }
    public AvatarOutlook(Dictionary<string,UserDataRecord> data) {
        this.baseModel =int.Parse(data["outlook_baseModel"].Value);
        this.top = int.Parse(data["outlook_top"].Value);
        this.body = int.Parse(data["outlook_body"].Value);
        this.bottom = int.Parse(data["outlook_bottom"].Value);
    }

    public static bool operator ==(AvatarOutlook a, AvatarOutlook b){
        if(a is null && b is null) return true;
        if(!(a is null) && b is null) return false;
        if(a is null && !(b is null)) return false;
        return a.baseModel == b.baseModel && a.top == b.top && a.body == b.body && a.bottom == b.bottom;
    }

    public static bool operator !=(AvatarOutlook a, AvatarOutlook b){
        return !(a == b);
    }
}

[Serializable]
public class PlayerLastPosition {
    public static List<string> keys = new List<string>{"position_x", "position_z", "rotation_x", "rotation_y", "rotation_z"};
    public float x = 0;
    public float z =0;
    public float rx = 0;
    public float ry =0;
    public float rz=0;
    public PlayerLastPosition(){}
    public PlayerLastPosition(float x, float z, Vector3 angles){this.x = x; this.z = z; this.rx = angles.x; this.ry = angles.y; this.rz = angles.z;}
    public PlayerLastPosition(Dictionary<string,UserDataRecord> data){
        this.x = float.Parse(data["position_x"].Value);
        this.z = float.Parse(data["position_z"].Value);
        this.rx = float.Parse(data["rotation_x"].Value);
        this.ry = float.Parse(data["rotation_y"].Value);
        this.rz = float.Parse(data["rotation_z"].Value);
    }

    public static bool operator ==(PlayerLastPosition a, PlayerLastPosition b){
        if(a is null && b is null) return true;
        if(!(a is null) && b is null) return false;
        if(a is null && !(b is null)) return false;
        return a.x == b.x && a.z == b.z && a.rx == b.rx && a.ry == b.ry && a.rz == b.rz;
    }

    public static bool operator !=(PlayerLastPosition a, PlayerLastPosition b){
        return !(a == b);
    }

    public bool IsDefault() {
        return x == 0 && z == 0 && rx == 0 && ry == 0 && rz == 0;
    }
}