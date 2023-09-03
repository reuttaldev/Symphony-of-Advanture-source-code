using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;
using System;
using System.Collections;

using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class RuntimeExportManager : MonoBehaviour, IRegistrableService
{
    [SerializeField]
    DataMigrationSettings dataMigrationSettings;
    [SerializeField]
    PlayerData playerData;
    string range = "Collected Data!A2";
    private SheetsService sheetsService;
    private string appName = "Reut's thesis project";
    private string _spreadsheetId = "1qHmvEGfXoCBZQWwDBrjASd-an27MBgBMTmAK-BSrNbI";
    private string serviceAccountKeyPath = @"\Assets\StreamingAssets\credentials.json";

    void Awake()
    {
        ServiceLocator.Instance.Register<RuntimeExportManager>(this);
    }
    void Start()
    {
        ConnectToGoogle();
    }
    #region GOOGLE EXPORT

    void SetGoogleSheets()
    {
        //GoogleSheets.SetSettings(sheetServiceProvider, dataMigrationSettings.spreadsheetID);
    }

    //service account will be used to authenticate ourselves to google, and to obtain an OAuth 2.0 access token
    private void ConnectToGoogle()
    {
        GoogleCredential credential;

        // Put your credentials json file in the root of the solution and make sure copy to output dir property is set to always copy 
        try
        {
            using (var stream = new FileStream(serviceAccountKeyPath, FileMode.Open, FileAccess.Read))
            {
                // getting service account credentials 
                //The CreateScoped method returns a copy of the credentials. 
                credential = GoogleCredential.FromStream(stream).CreateScoped(new string[] { SheetsService.Scope.Spreadsheets });
                /// <summary>
                /// Credential:
                ///A form of identification used in software security.In terms of authentication, 
                ///a credential is often a username and password combination. In terms of authorization for Google Workspace APIs,
                ///a credential is usually some form of identification, such as a unique secret string, 
                ///known only between the app developer and the authentication server. 
                ///Google supports these authentication credentials: API key, OAuth 2.0 Client ID, and service accounts.
                ///https://cloud.google.com/iam/docs/service-account-creds
                /// </summary>
            }
        // Create Google Sheets API service.
        // Generate a key from the credentials (json file) so you can run API requests
        sheetsService = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = appName
        });
        }
        catch(IOException e)
        {
            Debug.LogError("Service account credential file is not found. Please follow instructions on ");
        }
        catch(Exception e)
        {
            Debug.LogError(e.Message);
        }

    }


    // Pass in your data as a list of a list (2-D lists are equivalent to the 2-D spreadsheet structure)
    public string UpdateData(List<IList<object>> data)
    {
        string valueInputOption = "USER_ENTERED";

        // The new values to apply to the spreadsheet.
        List<ValueRange> updateData = new List<ValueRange>();
        var dataValueRange = new ValueRange();
        dataValueRange.Range = range;
        dataValueRange.Values = data;
        updateData.Add(dataValueRange);

        BatchUpdateValuesRequest requestBody = new BatchUpdateValuesRequest();
        requestBody.ValueInputOption = valueInputOption;
        requestBody.Data = updateData;

        var request = sheetsService.Spreadsheets.Values.BatchUpdate(requestBody, _spreadsheetId);

        BatchUpdateValuesResponse response = request.Execute();
        // Data.BatchUpdateValuesResponse response = await request.ExecuteAsync(); // For async 

        return JsonConvert.SerializeObject(response);
    }

    public void ExportToCollectedData(TrackData trackData,string interactionId, Emotions response)
    {
        string exportEventId = GetExportEventID();
        string timestamp = GetTime();
         List<object> dataToImpot= new List<object>();
        dataToImpot.Add(trackData.trackID);
        dataToImpot.Add(response.ToString());
        dataToImpot.Add(exportEventId);
        dataToImpot.Add(interactionId);
        dataToImpot.Add(playerData.playerName);
        dataToImpot.Add(playerData.playerID);
        dataToImpot.Add(timestamp);
        var values = new List<IList<object>> { dataToImpot };
        ConnectToGoogle();
        UpdateData(values);
        //StartCoroutine(UpdateSheet(values));

    }
    string GetExportEventID()
    {
        return "";
    }
    string GetTime()
    {
        return "";
    }


    #endregion
    #region CSV EXPORT
    #endregion
}
