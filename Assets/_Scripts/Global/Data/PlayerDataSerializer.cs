using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class PlayerDataSerializer
{
	public static void BinarySave(PlayerData data, string fileName)
	{	
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + Path.DirectorySeparatorChar + fileName);

		bf.Serialize (file, data);
		file.Close (); 
	}

	public static PlayerData BinaryLoad(string fileName)
	{
		if (File.Exists (Application.persistentDataPath + Path.DirectorySeparatorChar + fileName)) 
		{
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + Path.DirectorySeparatorChar + fileName, FileMode.Open);

			PlayerData data = (PlayerData)bf.Deserialize (file); 
			file.Close (); 

			return data;
		}
		return null;
	}
	public static void JsonSave(PlayerData data, string fileName)
	{
		if (data != null) {
			string jsonString = JsonUtility.ToJson (data, true);
			string filePath = Application.persistentDataPath + Path.DirectorySeparatorChar + fileName;
			StreamWriter writer = new StreamWriter (filePath, false);
			writer.WriteLine (jsonString);
			writer.Close (); 
		}
	}

	public static PlayerData JsonLoad(string fileName)
	{
		string jsonString = "";
		string filePath = Application.persistentDataPath + Path.DirectorySeparatorChar + fileName;
		if (File.Exists (filePath)) 
		{
			StreamReader reader = new StreamReader (filePath);
			jsonString = reader.ReadToEnd ();
			reader.Close ();

			//TODO: add some more handling for evaluating the data string for valid json 
			// and valid player data
			return JsonUtility.FromJson<PlayerData>(jsonString); 
		}
		return null;
	}


}