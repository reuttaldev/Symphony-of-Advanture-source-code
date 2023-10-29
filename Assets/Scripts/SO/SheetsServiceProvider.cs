using System;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using UnityEngine;

// this class is in charge of setting up the correct credentials in order to connect to google sheets using the API
[HelpURL("https://developers.google.com/sheets/api/guides/authorizing#AboutAuthorization")]
public static class SheetsServiceProvider  
{
    // this part is contains data of the credentials needed in order to authenticate and authorize a call to the Google API using a service account


    private static SheetsService sheetService;
    

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
    // The Google API access application we are requesting.
    static  readonly string[] scopes = { SheetsService.Scope.Spreadsheets };
    public static string instructionLocation = "";
    public static string savePath = "Assets/Scriptable Objects/Researcher Data";

    private static GoogleCredential GetCredential(JsonCredentialParameters parms)
    {
        if (credential == null)
        {
            try
            {
                credential = GoogleCredential.FromJsonParameters(parms).CreateScoped(new string[] { SheetsService.Scope.Spreadsheets });
                Debug.Log("Extraction of credential from json file was successful.");
            }
            catch (Exception e)
            {
                Debug.LogError("Could not get a Google Credential. " + e.Message);
            }
        }
        return credential;

    }
    public static SheetsService ConnectWithServiceAccountKey(JsonCredentialParameters parms)
    {
        if (sheetService == null)
        {
            try
            {
                var service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = GetCredential(parms),
                    ApplicationName = "App"
                });
                sheetService = service;
            }
            catch (Exception e) 
            {
                Debug.LogError("Could not get a SheetsService. "+ e.Message);
            }
        }
        return sheetService;
    }
   
}





    
