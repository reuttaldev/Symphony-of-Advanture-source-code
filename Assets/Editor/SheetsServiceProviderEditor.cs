using System.IO;
using UnityEngine;
using UnityEditor;
using System;

#if UNITY_EDITOR

[CustomEditor(typeof(SheetsServiceProvider))]
class SheetsServiceProviderEditor : Editor
{
    class Styles
    {
        public static readonly GUIContent authorize = EditorGUIUtility.TrTextContent("Authorize...", "Authorize the service. This is not required however the first time a connection to a Google sheet is required then authorization will be required.");
        public static readonly GUIContent authentication = EditorGUIUtility.TrTextContent("Authentication");
        public static readonly GUIContent cancel = EditorGUIUtility.TrTextContent("Cancel Authentication");
        public static readonly GUIContent noCredentials = EditorGUIUtility.TrTextContent("No Credentials Selected");
        public static readonly GUIContent loadCredentials = EditorGUIUtility.TrTextContent("Load Credentials...", "Load the credentials from a json file");
    }
    public SerializedProperty newSheetProperties;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (GUILayout.Button(Styles.loadCredentials))
        {
            // open file explorer and enable to choose a json file 
            var file = EditorUtility.OpenFilePanel(Styles.loadCredentials.text, "", "json");

            if (!string.IsNullOrEmpty(file))
            {
                ProcessServiceAccountKey(file);
            }
            else
                Debug.LogError("No credential json file was selected");
        }

    }
    public void ProcessServiceAccountKey(string path)
    {
        // encrypt it 
        // save it to streaming assets folder so we can access it in runtime in builds
        try
        {
            SaveJsonCredentials(path);
            Debug.Log("Processing of service account key was successful.");
        }
        catch (IOException e)
        {
            Debug.LogError($"Service account credential file is not found. Please follow instructions on {SheetsServiceProvider.instructionLocation} and try again.");
            Debug.LogError(e.Message);
        }
        catch (Exception e)
        {
            Debug.LogError("Processing of service account key was not successful. " + e.Message);
        }
    }
    private void SaveJsonCredentials(string path)
    {
        string jsonString = File.ReadAllText(path);
        Template temp = JsonUtility.FromJson<Template>(jsonString);
        ResearcherData researcherData =ScriptableObject.CreateInstance<ResearcherData>();
        researcherData.LoadData(temp.type, temp.project_id, temp.client_email, temp.client_id, temp.private_key, temp.private_key_id);
        string saveName = "Data1.asset";
        CreateFolder(SheetsServiceProvider.savePath);
        AssetDatabase.CreateAsset(researcherData, Path.Combine(SheetsServiceProvider.savePath, saveName));
        AssetDatabase.SaveAssets();

    }
    public static void CreateFolder(string folderPath)
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
}

[Serializable]
public class Template // this class is used as a deserialize template for the json key (data shall not be saved here, it is temporary). 
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
