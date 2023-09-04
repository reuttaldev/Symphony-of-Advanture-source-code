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
    SheetsServiceProvider sheetsServiceProvider;
    [SerializeField]
    PlayerData playerData;
    string range = "Collected Data!A2";

    void Awake()
    {
        ServiceLocator.Instance.Register<RuntimeExportManager>(this);
    }

    #region GOOGLE EXPORT




    // Pass in your data as a list of a list (2-D lists are equivalent to the 2-D spreadsheet structure)
    /*public string UpdateData(List<IList<object>> data)
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

        //var request = SheetsServiceProvider.Service.Spreadsheets.Values.BatchUpdate(requestBody, spreadsheetId);

        BatchUpdateValuesResponse response = request.Execute();
        // Data.BatchUpdateValuesResponse response = await request.ExecuteAsync(); // For async 

        return JsonConvert.SerializeObject(response);
    }*/

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
        //ConnectToGoogle();
        //UpdateData(values);
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
