using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using System;
using System.Reflection;

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
        EditorGUI.BeginDisabledGroup(sheetServiceProvider.objectReferenceValue == null);
        EditorGUILayout.PropertyField(spreadsheetID, new GUIContent("Spreadsheet ID"));
        EditorGUILayout.PropertyField(importSheetID, new GUIContent("Import Sheet ID"));
        EditorGUILayout.PropertyField(exportSheetID, new GUIContent("Export Sheet ID"));
        using (new EditorGUI.DisabledGroupScope(string.IsNullOrEmpty(importSheetID.stringValue)))
        {
            if (GUILayout.Button(new GUIContent("Open Import Sheet")))
            {
                GoogleSheets.OpenSheetInBrowser(spreadsheetID.stringValue, importSheetID.intValue);
            }
        }
        if (GUILayout.Button(new GUIContent("Open Export Sheet")))
        {
            // create the export sheet 
            if(string.IsNullOrEmpty(exportSheetID.stringValue))
            {

            }
            GoogleSheets.OpenSheetInBrowser(spreadsheetID.stringValue, exportSheetID.intValue);
        }
        if (pushTask != null && pushTask.IsCompleted)
        {
            pushTask = null;
        }
        using (new EditorGUI.DisabledGroupScope(pushTask != null || string.IsNullOrEmpty(spreadsheetID.stringValue)))
        {
            if (GUILayout.Button(new GUIContent("Pull (import)")))
            {

                var google = GetGoogleSheets();
                var target = GetActualObjectForSerializedProperty<GoogleSheetsExtension>(fieldInfo);
                google.PullIntoStringTableCollection(data.m_SheetId.intValue, target.TargetCollection as StringTableCollection, target.Columns, data.m_RemoveMissingPulledKeys.boolValue, TaskReporter.CreateDefaultReporter(), true);

                // Exit GUI to prevent erros due to GUI state changes. (LOC-698)
                GUIUtility.ExitGUI();
            }
        }
        EditorGUI.EndDisabledGroup();
    }
    GoogleSheets GetGoogleSheets()
    {
        var google = new GoogleSheets(Provider);
        google.SpreadSheetId = spreadsheetID.stringValue;
        return google;
    }
    void CreateNewsheet()
    {
        try
        {
            EditorUtility.DisplayProgressBar("Add Sheet", string.Empty, 0);
            var google = GetGoogleSheets();
            exportSheetID.intValue = google.AddSheet(exportSheetName, Provider.NewSheetProperties);
            exportSheetID.serializedObject.ApplyModifiedProperties();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            EditorUtility.ClearProgressBar();

            // Exit GUI to prevent erros due to GUI state changes. (LOC-698)
            GUIUtility.ExitGUI();
        }
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
