using System;
using System.IO;
using System.Text.Json;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using UnityEngine;

// this is the scriptable object that will be set up with correct credentials in order to connect to google sheets using the API
[CreateAssetMenu(fileName = "Google Sheets Service", menuName = "Data Migration/Google Sheets Service")]
[HelpURL("https://developers.google.com/sheets/api/guides/authorizing#AboutAuthorization")]
public class SheetsServiceProvider : ScriptableObject // change it to be a static clsss later and move the opening of file explorer to the data migration editor
{
    // this part is contains data of the credentials needed in order to authenticate and authorize a call to the Google API using a service account


    private static SheetsService sheetService;
    public static SheetsService Service
    {
        get
        {
            if (sheetService == null)
                sheetService = ConnectWithServiceAccountKey();
            return sheetService;
        }
    }

       // call the combine method rather than + to ensure the path uses the correct system slash and the path will be readable on every operating system.
    /// <summary>
    /// Credential:
    ///A form of identification used in software security.In terms of authentication, 
    ///a credential is often a username and password combination. In terms of authorization for Google Workspace APIs,
    ///a credential is usually some form of identification, such as a unique secret string, 
    ///known only between the app developer and the authentication server. 
    ///Google supports these authentication credentials: API key, OAuth 2.0 Client ID, and service accounts.
    ///https://cloud.google.com/iam/docs/service-account-creds
    /// </summary>
    private static GoogleCredential credential;
    public static GoogleCredential Credential
    {
        get
        {
            if (credential == null)
                credential= GetCredential();
            // check if getting credentials was successful 
            if (credential == null)
                Debug.LogError("Problem retrieving credential.");
            return credential;
        }
    }
    // The Google API access application we are requesting.
    private  readonly string[] scopes = { SheetsService.Scope.Spreadsheets };
    public  static string instructionLocation = "";
    public static string savePath = "Assets/Scriptable Objects/Researcher Data";

    private static GoogleCredential GetCredential()
    {
        GoogleCredential c = null;
        try
        {

            c = GoogleCredential.FromJsonParameters(LoadJsonCredentials()).CreateScoped(new string[] { SheetsService.Scope.Spreadsheets });
            Debug.Log("Extraction of credential from json file was successful.");
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        return c;

    }
    private static JsonCredentialParameters LoadJsonCredentials()
    {
        JsonCredentialParameters parms = new JsonCredentialParameters();
        return parms;
    }
    public static SheetsService ConnectWithServiceAccountKey()
    {
        var service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = Credential,
            ApplicationName = "App"
        });
        return service;
    }
   
}





    
