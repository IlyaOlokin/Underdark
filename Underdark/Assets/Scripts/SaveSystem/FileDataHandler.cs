using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";
    private string metaDataFileName = "";
    
    private bool useEncryption = false;
    private readonly string encryptionCodeWord = "encryptionCodeWord";

    public FileDataHandler(string dataDirPath, string dataFileName, string metaDataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.metaDataFileName = metaDataFileName;
        this.useEncryption = useEncryption;
    }

    public void Save(GameData data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }
            
            ClientSend.SaveReceived(DataLoader.metaGameData.Email, dataToStore);
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file:" + fullPath + "\n" + e);
        }
    }
    
    public void MetaSave(MetaGameData data)
    {
        string fullPath = Path.Combine(dataDirPath, metaDataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file:" + fullPath + "\n" + e);
        }
    }

    public GameData Load(string dataToLoad)
    {
        GameData loadedData = null;
        
        if (useEncryption)
        {
            dataToLoad = EncryptDecrypt(dataToLoad);
        }

        loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
        
        return loadedData;
    }
    
    public MetaGameData MetaLoad()
    {
        string fullPath = Path.Combine(dataDirPath, metaDataFileName);
        MetaGameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                
                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                loadedData = JsonUtility.FromJson<MetaGameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load data from file:" + fullPath + "\n" + e);
            }
        }

        return loadedData;
    }

    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char) (data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }

        return modifiedData;
    }
}