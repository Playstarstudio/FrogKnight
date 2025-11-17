using UnityEngine;
using System.IO;

/*
public class SaveSystem
{
    private static SaveData _saveData = new SaveData();

    [System.Serializable]
    public struct SaveData
    {
        public PlayerSaveData playerData;
    }

    public static string SaveFileName()
    {
        return Application.persistentDataPath + "/savefile" + ".save";
    }

    public static void Save()
    {
        HandleSaveData();
    }

    private static void HandleSaveData()
    {
        GameManager.Instance.Player.Save(ref _saveData.playerData);
    }
}

 */