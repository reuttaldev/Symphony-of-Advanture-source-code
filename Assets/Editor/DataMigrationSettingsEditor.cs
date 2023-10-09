using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using System;

[CustomEditor(typeof(DataMigrationSettings))]
public class DataMigrationSettingsEditor : Editor
{
    public SerializedProperty sheetServiceProvider;
    public SerializedProperty spreadsheetID;
    public SerializedProperty importSheetID;
    public SerializedProperty exportSheetID;
    public SerializedProperty columnsToRead;
    public SerializedProperty saveSOToPath;
    public SerializedProperty loadAudioPath;
    public SerializedProperty sentResultByEmail;
    public SerializedProperty researchersEmail;

    string exportSheetName = "Collected Data";
    public IList<Dictionary<string, string>> pulledData = null;
    public SheetsServiceProvider Provider => sheetServiceProvider.objectReferenceValue as SheetsServiceProvider;
    public void OnEnable()
    {
        sheetServiceProvider = serializedObject.FindProperty("sheetServiceProvider");
        spreadsheetID = serializedObject.FindProperty("spreadsheetID");
        importSheetID = serializedObject.FindProperty("importSheetID");
        exportSheetID = serializedObject.FindProperty("exportSheetID");
        columnsToRead = serializedObject.FindProperty("columnsToRead");
        saveSOToPath = serializedObject.FindProperty("saveSOToPath");
        loadAudioPath = serializedObject.FindProperty("loadAudioPath");
        sentResultByEmail = serializedObject.FindProperty("sentResultByEmail");
        researchersEmail = serializedObject.FindProperty("researchersEmail");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(sheetServiceProvider);

        // display only if google sheet porvided is plugged in
        using (new EditorGUI.DisabledGroupScope(sheetServiceProvider.objectReferenceValue == null))
        {
            EditorGUILayout.PropertyField(spreadsheetID, new GUIContent("Spreadsheet ID"));
            EditorGUILayout.Space();
            // display only is spreadsheet string id is there
            using (new EditorGUI.DisabledGroupScope(string.IsNullOrEmpty(spreadsheetID.stringValue)))
            {
                #region IMPORT SETTINGS
                EditorGUILayout.LabelField("Import Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(importSheetID, new GUIContent("Import Sheet ID"));
                using (new EditorGUI.DisabledGroupScope(importSheetID.intValue == 0))
                {
                    if (GUILayout.Button(new GUIContent("Open Meta Data Table")))
                    {
                        GoogleSheets.OpenSheetInBrowser(spreadsheetID.stringValue, importSheetID.intValue);
                    }
                    if (GUILayout.Button(new GUIContent("Import and save data")))
                    {
                        pulledData = GoogleSheets.PullData(spreadsheetID.stringValue, importSheetID.intValue, true, columnsToRead.intValue);
                        if (pulledData == null)
                        {
                            Debug.LogError("Data was pulled incorrectly");
                        }
                        else
                        {
                            GenerateTracksData.GenerateData(pulledData, (DataMigrationSettings)target);
                        }
                    }
                    EditorGUILayout.PropertyField(columnsToRead, new GUIContent("Columns to Read"));
                    EditorGUILayout.PropertyField(loadAudioPath, new GUIContent("Path to load audio tracks from"));
                    EditorGUILayout.PropertyField(saveSOToPath, new GUIContent("Saved Data Path"));

                }
                #endregion

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                #region EXPORT SETTINGS
                EditorGUILayout.LabelField("Export Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(exportSheetID, new GUIContent("Export Sheet ID"));
                using (new EditorGUI.DisabledGroupScope(importSheetID.intValue == 0))
                {
                    using (new EditorGUI.DisabledGroupScope(exportSheetID.intValue != 0))
                    {
                        if (GUILayout.Button(new GUIContent("Create Collected Data Table")))
                        {
                            int newSheetID = CreateNewSheet(exportSheetName);
                            if (newSheetID != 0)
                                exportSheetID.intValue = newSheetID;
                            exportSheetID.serializedObject.ApplyModifiedProperties();
                            serializedObject.Update();

                        }
                    }
                    using (new EditorGUI.DisabledGroupScope(exportSheetID.intValue == 0))
                    {
                        if (GUILayout.Button(new GUIContent("Open Collected Data Table")))
                        {
                            GoogleSheets.OpenSheetInBrowser(spreadsheetID.stringValue, exportSheetID.intValue);
                        }
                    }
                    EditorGUILayout.PropertyField(sentResultByEmail, new GUIContent("Send Collected Data to Email"));
                    using (new EditorGUI.DisabledGroupScope(!sentResultByEmail.boolValue))
                    {
                        EditorGUILayout.PropertyField(researchersEmail, new GUIContent("Email to send to"));
                    }

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
            return GoogleSheets.AddSheet(spreadsheetID.stringValue, sheetName, Provider.newSheetProperties);
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

}
