using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using System;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(DataMigrationSettings))]
public class DataMigrationSettingsEditor : Editor
{
    public SerializedProperty sheetServiceProvider;
    public SerializedProperty spreadsheetID;
    public SerializedProperty importSheetID;
    public SerializedProperty exportSheetID;
    public Task pushTask;
    string exportSheetName = "Collected Data";
    public SheetsServiceProvider Provider => sheetServiceProvider.objectReferenceValue as SheetsServiceProvider;
    public void OnEnable()
    {
        sheetServiceProvider = serializedObject.FindProperty("sheetServiceProvider");
        spreadsheetID = serializedObject.FindProperty("spreadsheetID");
        importSheetID = serializedObject.FindProperty("importSheetID");
        exportSheetID = serializedObject.FindProperty("exportSheetID");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(sheetServiceProvider);

        //EditorGUI.BeginDisabledGroup(sheetServiceProvider.objectReferenceValue == null);
        // display only if google sheet porvided is plugged in
        using (new EditorGUI.DisabledGroupScope(sheetServiceProvider.objectReferenceValue == null))
        {
            EditorGUILayout.PropertyField(spreadsheetID, new GUIContent("Spreadsheet ID"));
            EditorGUILayout.PropertyField(importSheetID, new GUIContent("Import Sheet ID"));
            using (new EditorGUI.DisabledGroupScope(true))
            {
                EditorGUILayout.PropertyField(exportSheetID, new GUIContent("Export Sheet ID"));
            }
            using (new EditorGUI.DisabledGroupScope(importSheetID.intValue == 0))
            {
                if (GUILayout.Button(new GUIContent("Create Collected Data Table")))
                {
                    int newSheetID = CreateNewSheet(exportSheetName);
                    if (newSheetID != 0)
                        exportSheetID.intValue = newSheetID;
                    exportSheetID.serializedObject.ApplyModifiedProperties();
                    serializedObject.Update();

                }
                using (new EditorGUI.DisabledGroupScope(exportSheetID.intValue == 0))
                {
                    if (GUILayout.Button(new GUIContent("Open Collected Data Table")))
                    {

                        GoogleSheets.OpenSheetInBrowser(spreadsheetID.stringValue, exportSheetID.intValue);
                    }
                    if (pushTask != null && pushTask.IsCompleted)
                    {
                        pushTask = null;
                    }
                    using (new EditorGUI.DisabledGroupScope(pushTask != null))
                    {
                        if (GUILayout.Button(new GUIContent("Import data and save")))
                        {

                            var google = GetGoogleSheets();
                            //google.Pull(importSheetID, target.Colums);
                            // Exit GUI to prevent erros due to GUI state changes. (LOC-698)
                            GUIUtility.ExitGUI();
                        }
                    }
                }
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
    GoogleSheets GetGoogleSheets()
    {
        var google = new GoogleSheets(Provider);
        google.SpreadSheetId = spreadsheetID.stringValue;
        return google;
    }
    int CreateNewSheet(string sheetName)
    {
        try
        {
            EditorUtility.DisplayProgressBar("Add Sheet", string.Empty, 0);
            var google = GetGoogleSheets();
            // return the id of the created sheet
            return google.AddSheet(sheetName, Provider.NewSheetProperties);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            // Exit GUI to prevent erros due to GUI state changes. (LOC-698)
        }
        return 0;
}
/*void Push()
{
    var google = GetGoogleSheets(data);
                var target = property.GetActualObjectForSerializedProperty<GoogleSheetsExtension>(fieldInfo);
                var collection = target.TargetCollection as StringTableCollection;
                data.pushTask = google.PushStringTableCollectionAsync(data.m_SheetId.intValue, collection, target.Columns, TaskReporter.CreateDefaultReporter());
                s_PushRequests.Add((collection, data.pushTask));

                // Exit GUI to prevent erros due to GUI state changes. (LOC-698)
                GUIUtility.ExitGUI();
}*/
}
