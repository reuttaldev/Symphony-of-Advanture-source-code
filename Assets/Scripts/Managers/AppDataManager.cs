using Google.Apis.Auth.OAuth2;
using System;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

public static class AppDataManager
{ 

    public static string savePath = "Assets/Scriptable Objects/Researcher Data";
    static byte[] iv = GenerateRandomBytes(16); // Generate a 16-byte IV

    public static JsonCredentialParameters GetJsonParameters(DataMigrationSettings settings)
    {
        ResearcherData data1 = settings.researcherData;
        ClientData data2 = settings.clientData;
        JsonCredentialParameters parms = new JsonCredentialParameters();
        parms.ProjectId = Decrypt(data1.projectId, data2.clientPreference);
        parms.ClientId = Decrypt(data1.clientId, data2.clientPreference);
        parms.ClientEmail = Decrypt(data1.clientEmail, data2.clientPreference);
        parms.PrivateKeyId = Decrypt(data1.researchersId, data2.clientPreference);
        parms.PrivateKey = Decrypt(data1.researchersData, data2.clientPreference);
        parms.Type = Decrypt(data1.projectType, data2.clientPreference);
        return parms;
    }


    public static string Encrypt(string plainText, byte[] key)
    {
        // Check arguments.
        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException("plainText");

        byte[] encrypted;
        // Create an AesManaged object
        // with the specified key and IV.
        using (Rijndael algorithm = Rijndael.Create())
        {
            algorithm.Key = key;

            // Create a decrytor to perform the stream transform.
            var encryptor = algorithm.CreateEncryptor(algorithm.Key, algorithm.IV);

            // Create the streams used for encryption.
            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        // Write IV first
                        msEncrypt.Write(algorithm.IV, 0, algorithm.IV.Length);
                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        // Return the encrypted bytes from the memory stream.
        return Convert.ToBase64String(encrypted);
    }

#if UNITY_EDITOR
    public static void LoadJsonCredentials(DataMigrationSettings settings, string path)
    {
        // load the json file into a string
        string jsonString = File.ReadAllText(path);
        // deserialize it 
        JsonDataTemplate temp = JsonUtility.FromJson<JsonDataTemplate>(jsonString);
        // create scriptable objects to save the data in 
        ResearcherData researcherData = ScriptableObject.CreateInstance<ResearcherData>();
        ClientData clientData = ScriptableObject.CreateInstance<ClientData>();
        clientData.clientPreference = GenerateRandomBytes(32);
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
        researcherData.projectType = Encrypt(temp.type, clientData.clientPreference);
        researcherData.projectId = Encrypt(temp.project_id, clientData.clientPreference);
        researcherData.clientEmail = Encrypt(temp.client_email, clientData.clientPreference);
        researcherData.clientId = Encrypt(temp.client_id, clientData.clientPreference);
        researcherData.researchersId = Encrypt(temp.private_key_id, clientData.clientPreference);
        researcherData.researchersData = Encrypt(temp.private_key, clientData.clientPreference);
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
    public static string Decrypt(string cipherText, byte[] key)
    {
        // Check arguments.
        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException("cipherText");

        // Declare the string used to hold
        // the decrypted text.
        string plaintext = null;

        // Create an AesManaged object
        // with the specified key and IV.
        using (Rijndael algorithm = Rijndael.Create())
        {
            algorithm.Key = key;

            // Get bytes from input string
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
            {
                // Read IV first
                byte[] IV = new byte[16];
                msDecrypt.Read(IV, 0, IV.Length);

                // Assign IV to an algorithm
                algorithm.IV = IV;

                // Create a decrytor to perform the stream transform.
                var decryptor = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV);

                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        // Read the decrypted bytes from the decrypting stream
                        // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }
        return plaintext;
    }
    public static byte[] GenerateRandomBytes(int length)
    {
        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
        {
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return bytes;
        }
    }
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
#endif