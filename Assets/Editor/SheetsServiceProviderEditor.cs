using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using UnityEngine;
using UnityEditor;
using UnityEditor.MPE;
using Google.Apis.Sheets.v4.Data;
using UnityEditor.VersionControl;

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

    private void OnEnable()
    {
        newSheetProperties = serializedObject.FindProperty("newSheetProperties");

    }

    static Task<UserCredential> authorizeTask;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (GUILayout.Button(Styles.loadCredentials))
        {
            // open file explorer and enable to choose a json file 
            var file = EditorUtility.OpenFilePanel(Styles.loadCredentials.text, "", "json");

            if (!string.IsNullOrEmpty(file))
            {
                var provider = target as SheetsServiceProvider;
                provider.LoadServiceAccountKey(file);
            }
            else
                Debug.LogError("No credential json file was selected");
        }
        EditorGUILayout.PropertyField(newSheetProperties, true);

        /* if (authorizeTask != null)
         {

             if (authorizeTask.IsCompleted)
             {
                 if (authorizeTask.Status == TaskStatus.RanToCompletion)
                     Debug.Log($"Authorized: {authorizeTask.Result.Token.IssuedUtc}", target);
                 else if (authorizeTask.Exception != null)
                     Debug.LogException(authorizeTask.Exception, target);
                 authorizeTask = null;
             }
         }
         else
         {
             if (GUILayout.Button(Styles.authorize))
             {
                 var provider = target as SheetsServiceProvider;
                 //authorizeTask = provider.Au;
             }
         }*/

        serializedObject.ApplyModifiedProperties();
    }
}
#endif