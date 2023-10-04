using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

public static class SaveSystem 
{
    public static void Save(GameObject Player, int SaveSlot)
    {
        
        BinaryFormatter f = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.SaveFile" + SaveSlot.ToString();       
        PlayerData d = new PlayerData(Player);
        FileStream s = new FileStream(path, FileMode.Create);        
        f.Serialize(s, d);
        s.Close();
        
        

        /*
        string path = "C:/Users/USER/Desktop/For Class" + "/player.SaveFile" + SaveSlot.ToString() + ".txt";
        PlayerData d = new PlayerData(Player);
        string dJson = JsonUtility.ToJson(d);
        File.WriteAllText(path, dJson); 
        */
        
        
        /*
        string path = "C:/Users/USER/Desktop/For Class" + "/player.SaveFile" + SaveSlot.ToString() + ".xml";
        PlayerData d = new PlayerData(Player);
        XmlSerializer f = new XmlSerializer(typeof(PlayerData));
        FileStream s = new FileStream(path, FileMode.Create);
        f.Serialize(s, d);
        s.Close();
        */
    }
    public static PlayerData Load(int SaveSlot)
    {
        string path = Application.persistentDataPath + "/player.SaveFile" + SaveSlot.ToString();
        //string path = "C:/Users/USER/Desktop/For Class" + "/player.SaveFile" + SaveSlot.ToString() + ".txt";
        //string path = "C:/Users/USER/Desktop/For Class" + "/player.SaveFile" + SaveSlot.ToString() + ".xml";

        if (File.Exists(path))
        {
            
            BinaryFormatter f = new BinaryFormatter();
            FileStream s = new FileStream(path, FileMode.Open);
            PlayerData d = f.Deserialize(s) as PlayerData;
            s.Close();
            
            

            /*
            string dJson = File.ReadAllText(path);
            PlayerData d = JsonUtility.FromJson<PlayerData>(dJson);
            */
            

            /*
            XmlSerializer f = new XmlSerializer(typeof(PlayerData));
            FileStream s = new FileStream(path, FileMode.Open);
            PlayerData d = f.Deserialize(s) as PlayerData;
            s.Close();
            */

            return d;
        }
        else
        {
            Debug.LogError($"Savefile not found in {path}");
            return null;
        }
    }
}
