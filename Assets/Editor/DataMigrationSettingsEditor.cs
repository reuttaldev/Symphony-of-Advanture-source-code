using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using Google.Apis.Sheets.v4;
using System.Security.Cryptography;
using Codice.Client.Common;
using Codice.CM.Common;
using static Codice.Client.Common.Servers.RecentlyUsedServers;
using System.Buffers.Text;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using Unity.VisualScripting.YamlDotNet.Core;
using Unity.VisualScripting;
using Google;
using UnityEngine.UIElements;

#if UNITY_EDITOR
[CustomEditor(typeof(DataMigrationSettings))]
public class DataMigrationSettingsEditor : Editor
{
    public SerializedProperty spreadsheetID;
    public SerializedProperty importSheetID;
    public SerializedProperty exportSheetID;
    public SerializedProperty columnsToRead;
    public SerializedProperty saveSOToPath;
    public SerializedProperty loadAudioPath;
    public SerializedProperty sentResultByEmail;
    public SerializedProperty researchersEmail;
    public SerializedProperty newSheetProperties;
    public SerializedProperty exportSheetName;
    public SerializedProperty tempExportName;
    public SerializedProperty errorMessage;
    public IList<Dictionary<string, string>> pulledData = null;

    // local success messages - local because we want the message to disappear if exiting the scriptable object and coming back. with the error messages- I want them to still pear after re-entering 
    string sucessMessage = "";
    public void OnEnable()
    {
        spreadsheetID = serializedObject.FindProperty("spreadsheetID");
        importSheetID = serializedObject.FindProperty("importSheetID");
        exportSheetID = serializedObject.FindProperty("exportSheetID");
        columnsToRead = serializedObject.FindProperty("columnsToRead");
        saveSOToPath = serializedObject.FindProperty("saveSOToPath");
        loadAudioPath = serializedObject.FindProperty("loadAudioPath");
        sentResultByEmail = serializedObject.FindProperty("sentResultByEmail");
        researchersEmail = serializedObject.FindProperty("researchersEmail");
        newSheetProperties = serializedObject.FindProperty("newSheetProperties");
        exportSheetName = serializedObject.FindProperty("exportSheetName");
        tempExportName = serializedObject.FindProperty("tempExportName");
        errorMessage = serializedObject.FindProperty("errorMessage");
    }
    void ProgressBar(float value, string label)
    {
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (GUILayout.Button(new GUIContent("Load Credentials")))
        {
            // open file explorer and enable to choose a json file 
            var file = EditorUtility.OpenFilePanel("Load the credentials from a json file", "", "json");
            ProcessServiceAccountKey(file);
        }
        // display only if json credentials were already loaded
        using (new EditorGUI.DisabledGroupScope(((DataMigrationSettings)target).researcherData == null))
        {
            #region IMPORT
            EditorGUILayout.PropertyField(spreadsheetID, new GUIContent("Spreadsheet ID"));
            using (new EditorGUI.DisabledGroupScope(string.IsNullOrEmpty(spreadsheetID.stringValue)))
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                // display only is spreadsheet string id is there

                EditorGUILayout.LabelField("Import Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(importSheetID, new GUIContent("Import Sheet ID"));
                using (new EditorGUI.DisabledGroupScope(importSheetID.intValue == 0))
                {
                    if (GUILayout.Button(new GUIContent("Open Meta Data Table")))
                        GoogleSheets.OpenSheetInBrowser(spreadsheetID.stringValue, importSheetID.intValue);
                    if (GUILayout.Button(new GUIContent("Import and save data")))
                        ImportAndSaveData();

                    EditorGUILayout.PropertyField(columnsToRead, new GUIContent("Columns to Read"));
                    EditorGUILayout.PropertyField(loadAudioPath, new GUIContent("Path to load audio tracks from"));
                    EditorGUILayout.PropertyField(saveSOToPath, new GUIContent("Saved Data Path"));
                }
                #endregion

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                HandleHelpBoxes();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                #region EXPORT
                EditorGUILayout.LabelField("Export Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(newSheetProperties, new GUIContent("New spreadsheet properties"));
                EditorGUILayout.PropertyField(exportSheetName, new GUIContent("Table Name"));
               
                
                if (GUILayout.Button(new GUIContent("Create Collected Data Table")))
                {
                    int newSheetID = CreateNewSheet(exportSheetName.stringValue);
                    if (newSheetID != 0)
                    {
                        exportSheetID.intValue = newSheetID;
                        tempExportName.stringValue = exportSheetName.stringValue;
                    }
                }
                EditorGUILayout.PropertyField(exportSheetID, new GUIContent("Export Sheet ID"));
                using (new EditorGUI.DisabledGroupScope(exportSheetID.intValue == 0))
                {
                   // SheetsService service = SheetsServiceProvider.ConnectWithServiceAccountKey((DataMigrationSettings)target);
                    if (GUILayout.Button(new GUIContent("Open Collected Data Table")))
                    {
                        GoogleSheets.OpenSheetInBrowser(spreadsheetID.stringValue, exportSheetID.intValue);
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(sentResultByEmail, new GUIContent("Send Collected Data to Email"));
                using (new EditorGUI.DisabledGroupScope(!sentResultByEmail.boolValue))
                {
                    EditorGUILayout.PropertyField(researchersEmail, new GUIContent("Email to send to"));
                }
                #endregion
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
    int CreateNewSheet(string sheetName)
    {
        try
        {
            EditorUtility.DisplayProgressBar("Add Sheet", string.Empty, 0);
            // return the id of the created sheet
            SheetsService service = SheetsServiceProvider.ConnectWithServiceAccountKey((DataMigrationSettings)target);
            int id= GoogleSheets.AddSheet(service, spreadsheetID.stringValue, sheetName, ((DataMigrationSettings)target).newSheetProperties);
            UpdateHelpBoxMessage("Created new export sheet successfully");
            return id;
        }
        catch (GoogleApiException e)
        {
            UpdateHelpBoxMessage(GoogleSheets.HandleGoogleSheetExceptions(e),true);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
            return 0;
    }

    public void ProcessServiceAccountKey(string path)
    {
        // encrypt it 
        // save it to streaming assets folder so we can access it in runtime in builds
        try
        {
            AppDataManager.LoadJsonCredentials((DataMigrationSettings)target, path);
            UpdateHelpBoxMessage("Processing of service account key was successful.");
        }
        catch (IOException e)
        {
            UpdateHelpBoxMessage($"Service account credential file is not found. Please follow instructions on {SheetsServiceProvider.instructionLocation} and try again.",true);
        }
        catch (Exception e)
        {
            UpdateHelpBoxMessage("Processing of service account key was not successful. " + e.Message,true);
        }
    }

    // this method must be called in every oninspector gui and not just when a button is clicked etc bc then the help box is showen only for the half second when the button is actually being pressed 
    void HandleHelpBoxes()
    {

        if (sucessMessage != "")
        {
            EditorGUILayout.HelpBox(sucessMessage, MessageType.Info);
        }
        if (errorMessage.stringValue != "")
        {
            EditorGUILayout.HelpBox(errorMessage.stringValue, MessageType.Error);
        }
        // only show warning and sucess if there are no errors
        else if (tempExportName.stringValue != "")
        {
            if (tempExportName.stringValue != exportSheetName.stringValue)
                EditorGUILayout.HelpBox("Data will be exported to a sheet with name " + tempExportName.stringValue + ". Please click \"Create Collected Data Table\" to export to a table with the new name.", MessageType.Warning);
        }
    }

    // must have this method so the messages are updates and we are not presenting an error message and success message at the same time- only the one happened the most recently
    void UpdateHelpBoxMessage(string m, bool error=false)
    {
        if(error)
        {
            errorMessage.stringValue = m;
            sucessMessage = "";
        }
        else
        {
            sucessMessage = m;
            errorMessage.stringValue = "";
        }
    }
    void ImportAndSaveData()
    {
        SheetsService service = SheetsServiceProvider.ConnectWithServiceAccountKey((DataMigrationSettings)target);
        try
        {

            pulledData = GoogleSheets.PullData(service, spreadsheetID.stringValue, importSheetID.intValue, true, columnsToRead.intValue);
            GenerateTracksData.GenerateData(pulledData, (DataMigrationSettings)target);
            GameSettings settings = AssetDatabase.LoadAssetAtPath< GameSettings>("Assets/Game Settings.asset");
            if(settings == null)
            {
                Debug.LogError("Game Settings asset is missing");
            }
            settings.SetDefaultTracks();
            UpdateHelpBoxMessage("Data was imported successfully.");
        }
        catch(GoogleApiException ex)
        {
            UpdateHelpBoxMessage(GoogleSheets.HandleGoogleSheetExceptions(ex),true);
        }
        catch (Exception ex) 
        {
            UpdateHelpBoxMessage("Data was pulled incorrectly",true);
            Debug.LogError(ex.Message);
        }
    }
}
#endif