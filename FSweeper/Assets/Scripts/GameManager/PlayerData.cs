using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public string Name;
    public Dictionary<string, float> TimeUsedDictonary;
    
    public PlayerData(string Name)
    {
        this.Name = Name;
        TimeUsedDictonary = new Dictionary<string, float>();
        TimeUsedDictonary.Add("5x5", 0);
        TimeUsedDictonary.Add("10x10", 0);
        TimeUsedDictonary.Add("15x15", 0);
    }

    public string jsonify()
    {
        return JsonUtility.ToJson(this);
    }

    public static PlayerData Parse(string json)
    {
        return JsonUtility.FromJson<PlayerData>(json);
    }

}
