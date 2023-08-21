using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using UnityEngine;
using UnityEditor;
using UnityEditor.MPE;
using Google.Apis.Sheets.v4.Data;

#if UNITY_EDITOR
[CustomEditor(typeof(SheetsServiceProvider))]
class SheetsServiceProviderEditor : Editor
{
    class Styles
    {
        public static readonly GUIContent authorize = EditorGUIUtility.TrTextContent("Authorize...", "Authorize the user. This is not required however the first time a connection to a Google sheet is required then authorization will be required.");
        public static readonly GUIContent authentication = EditorGUIUtility.TrTextContent("Authentication");
        public static readonly GUIContent cancel = EditorGUIUtility.TrTextContent("Cancel Authentication");
        public static readonly GUIContent clientId = EditorGUIUtility.TrTextContent("Client Id");
        public static readonly GUIContent clientSecret = EditorGUIUtility.TrTextContent("Client Secret");
        public static readonly GUIContent noCredentials = EditorGUIUtility.TrTextContent("No Credentials Selected");
        public static readonly GUIContent loadCredentials = EditorGUIUtility.TrTextContent("Load Credentials...", "Load the credentials from a json file");
    }

    public SerializedProperty clientID;
    public SerializedProperty clientSecret;
    public SerializedProperty newSheetProperties;
    static Task<UserCredential> authorizeTask;
    static CancellationTokenSource cancellationToken;

    public void OnEnable()
    {
        newSheetProperties = serializedObject.FindProperty("newSheetProperties");
        clientID = serializedObject.FindProperty("clientID");
        clientSecret = serializedObject.FindProperty("clientSecret");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(clientID, new GUIContent("Client ID"));
        EditorGUILayout.PropertyField(clientSecret);

        if (GUILayout.Button(Styles.loadCredentials))
        {
            var file = EditorUtility.OpenFilePanel(Styles.loadCredentials.text, "", "json");
            if (!string.IsNullOrEmpty(file))
            {
                var json = File.ReadAllText(file);
                var secrets = SheetsServiceProvider.LoadSecrets(json);
                clientID.stringValue = secrets.ClientId;
                clientSecret.stringValue = secrets.ClientSecret;
            }
        }
        
        // don't show the authorize button until client id and secret are inputed 
        EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(clientID.stringValue) || string.IsNullOrEmpty(clientSecret.stringValue));

        if (authorizeTask != null)
        {

            if (authorizeTask.IsCompleted)
            {
                if (authorizeTask.Status == TaskStatus.RanToCompletion)
                    Debug.Log($"Authorized: {authorizeTask.Result.Token.IssuedUtc}", target);
                else if (authorizeTask.Exception != null)
                    Debug.LogException(authorizeTask.Exception, target);
                authorizeTask = null;
                cancellationToken = null;
            }
        }
        else
        {
            if (GUILayout.Button(Styles.authorize))
            {
                var provider = target as SheetsServiceProvider;
                cancellationToken = new CancellationTokenSource();
                authorizeTask = provider.AuthorizeOAuthAsync(cancellationToken.Token);
            }
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.PropertyField(newSheetProperties, true);


        serializedObject.ApplyModifiedProperties();
    }
}
#endif