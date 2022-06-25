using System.IO;
using System.Collections.Generic;
using UnityEngine;

// A Class Which Holds Information About Characters.
[System.Serializable]
public class CharacterInfo
{
    public string DisplayName;
    public int Index;
    public bool DisableImageFiltering;
    public bool UseCustomDimentions;
    [System.NonSerialized]
    public static readonly Vector3 DefaultDimentions = new Vector3(1f, 1f, 1.5f);
    public float[] CustomDimentions;

    public Vector3 GetDimentions()
    {
        Vector3 Custom = new Vector3(CustomDimentions[0], CustomDimentions[2], CustomDimentions[1]);
        return (UseCustomDimentions) ? Custom : DefaultDimentions;
    }

    public void SaveJSON()
    {
        string JSONstring = JsonUtility.ToJson(this);
        File.WriteAllText(Application.dataPath + "/CharInf.txt", JSONstring);
    }
    public static CharacterInfo LoadJSON(string CharPath)
    {
        string Path = CharPath + "/CharInf.txt";
        string JSONstring = File.ReadAllText(Path);
        CharacterInfo LoadedInfo = JsonUtility.FromJson<CharacterInfo>(JSONstring);
        return LoadedInfo;
    }
}
