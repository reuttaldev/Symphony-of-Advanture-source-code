using Google.Apis.Auth.OAuth2;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class AppDataManager
{ 

    public static string savePath = "Assets/AppData/Researcher Data";
    static byte[] iv = EncryptionManager.GenerateRandomBytes(16); // Generate a 16-byte IV

    public static JsonCredentialParameters GetJsonParameters(DataMigrationSettings settings)
    {
        ResearcherData data1 = settings.researcherData;
        ClientData data2 = settings.clientData;
        JsonCredentialParameters parms = new JsonCredentialParameters();
        parms.ProjectId = EncryptionManager.Decrypt(data1.projectId, data2.clientPreference);
        parms.ClientId = EncryptionManager.Decrypt(data1.clientId, data2.clientPreference);
        parms.ClientEmail = EncryptionManager.Decrypt(data1.clientEmail, data2.clientPreference);
        parms.PrivateKeyId = EncryptionManager.Decrypt(data1.researchersId, data2.clientPreference);
        parms.PrivateKey = EncryptionManager.Decrypt(data1.researchersData, data2.clientPreference);
        parms.Type = EncryptionManager.Decrypt(data1.projectType, data2.clientPreference);
        return parms;
    }

#if UNITY_EDITOR
    public static void LoadJsonCredentials(DataMigrationSettings settings, string path)
    {
        // load the json file into a string
        string jsonString = File.ReadAllText(path);
        // deserialize it, since the credentials original format is json
        JsonDataTemplate temp = JsonUtility.FromJson<JsonDataTemplate>(jsonString);
        // create scriptable objects to save the data in 
        ResearcherData researcherData = ScriptableObject.CreateInstance<ResearcherData>();
        ClientData clientData = ScriptableObject.CreateInstance<ClientData>();
        clientData.clientPreference = EncryptionManager.GenerateRandomBytes(32);
        ProcessCredentials(temp, researcherData, clientData);
        // save the created instances in assets
        CreateAssetFolder(savePath);
        AssetDatabase.CreateAsset(researcherData, Path.Combine(savePath, "Data1.asset"));
        AssetDatabase.CreateAsset(clientData, Path.Combine(savePath, "Data2.asset"));
        // save a reference of the scriptable objects we created in the seetings
        EditorUtility.SetDirty(settings);
        settings.researcherData = researcherData;
        settings.clientData = clientData;
        AssetDatabase.SaveAssets();
    }

    // save the data from the template in the correct scriptable object, after encryption
    static void ProcessCredentials(JsonDataTemplate temp, ResearcherData researcherData, ClientData clientData)
    {
        // create and save password
        researcherData.projectType = EncryptionManager.Encrypt(temp.type, clientData.clientPreference);
        researcherData.projectId = EncryptionManager.Encrypt(temp.project_id, clientData.clientPreference);
        researcherData.clientEmail = EncryptionManager.Encrypt(temp.client_email, clientData.clientPreference);
        researcherData.clientId = EncryptionManager.Encrypt(temp.client_id, clientData.clientPreference);
        researcherData.researchersId = EncryptionManager.Encrypt(temp.private_key_id, clientData.clientPreference);
        researcherData.researchersData = EncryptionManager.Encrypt(temp.private_key, clientData.clientPreference);
    }
    static void CreateAssetFolder(string folderPath) // create a place to save our scriptable object
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Console.WriteLine("Directory created: " + folderPath);
        }
        else // I am deleting everything where our asset will be placed to ensure no old  data remains 
        {

            DirectoryInfo directory = new DirectoryInfo(folderPath);

            // Delete all files in the folder
            foreach (FileInfo file in directory.GetFiles())
            {
                file.Delete();
            }
            // make changes appear in the editor
            AssetDatabase.Refresh();
        }
    }
#endif
}

[Serializable]
public class JsonDataTemplate // this class is used as a deserialize template for the json key (data shall not be saved here, it is temporary). 
{
    public string type;
    public string project_id;
    public string private_key_id;
    public string private_key;
    public string client_email;
    public string client_id;
    public string auth_uri;
    public string token_uri;
    public string auth_provider_x509_cert_url;
    public string client_x509_cert_url;
    public string universe_domain;
}