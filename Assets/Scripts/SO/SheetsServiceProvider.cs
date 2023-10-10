using System;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using UnityEngine;

// this is the scriptable object that will be set up with correct credentials in order to connect to google sheets using the API
[CreateAssetMenu(fileName = "Google Sheets Service", menuName = "Data Migration/Google Sheets Service")]
[HelpURL("https://developers.google.com/sheets/api/guides/authorizing#AboutAuthorization")]
public class SheetsServiceProvider : ScriptableObject
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

    private static string keyFileName=  "credentials.json";
    // call the combine method rather than + to ensure the path uses the correct system slash and the path will be readable on every operating system.
    private static string savedKeyPath = Path.Combine(Application.streamingAssetsPath, keyFileName);
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
                credential= GetCredentialFromJson();
            // check if getting credentials was successful 
            if (credential == null)
                Debug.LogError("Problem retrieving credential.");
            return credential;
        }
    }

    [SerializeField]
    private static string applicationName = "Reut's App";
    // The Google API access application we are requesting.
    private  readonly string[] scopes = { SheetsService.Scope.Spreadsheets };

    private static string instructionLocation = "";

    // load the json file with the service account key from the researcher's machine
    // service account key is a credential 
    public void LoadServiceAccountKey(string originalKeyPAth)
    {
        // encrypt it 
        // save it to streaming assets folder so we can access it in runtime in builds
        try
        {
            File.Copy(originalKeyPAth, savedKeyPath, true);// true for override if file already exists 
            Debug.Log("Saving of service account key was successful.");
        }
        catch (IOException e)
        {
            Debug.LogError($"Service account credential file is not found. Please follow instructions on {instructionLocation} and try again.");
            Debug.LogError(e.Message);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        // extract credentials 
        GetCredentialFromJson();

    }
    private static GoogleCredential GetCredentialFromJson()
    {
        try
        {
            GoogleCredential c = null;
            //The streamingAssets path is read-only. Don’t modify or write new files to the streamingAssets directory at runtime.
            //On Android and WebGL platforms, it’s not possible to access the streaming asset files directly via file system APIs and streamingAssets path because these platforms return a URL. Use the UnityWebRequest class to access the content instead.
            // dycript 
            using (var stream = new FileStream(savedKeyPath, FileMode.Open, FileAccess.Read))
            {
                // getting service account credentials 
                //The CreateScoped method returns a copy of the credentials. 
                c = GoogleCredential.FromStream(stream).CreateScoped(new string[] { SheetsService.Scope.Spreadsheets });
            }
            Debug.Log("Extraction of credential from json file was successful.");
            return c;

        }
        catch (IOException e)
        {
            Debug.LogError("Could not read file.");
            Debug.LogError(e.Message);

        }
        catch (Exception e)
        {
            Debug.LogError($"The file that was selected is not a service account key file. Please follow instructions on {instructionLocation} and try again.");
            Debug.LogError(e.Message);
        }
            return null;
    }

    public static SheetsService ConnectWithServiceAccountKey()
    {
        var service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = Credential,
            ApplicationName = applicationName
        });
        return service;
    }
   
}





    
