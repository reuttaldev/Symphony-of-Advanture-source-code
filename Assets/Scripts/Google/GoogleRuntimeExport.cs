using Google.Apis.Auth.OAuth2;

using Google.Apis.Sheets.v4;

using Google.Apis.Sheets.v4.Data;

using Google.Apis.Services;

using System.Collections;

using System.Collections.Generic;

using System.IO;

using UnityEngine;

using UnityEngine.Networking;
using UnityEngine;
using Newtonsoft.Json.Linq;
using UnityEditor.VersionControl;
using System;

public class GoogleRuntimeExport : MonoBehaviour, IRegistrableService
{
    [SerializeField]
    DataMigrationSettings dataMigrationSettings;
    [SerializeField]
    PlayerData playerData;
    [SerializeField]
    public SheetsServiceProvider sheetServiceProvider;
    void Awake()
    {
        ServiceLocator.Instance.Register<GoogleRuntimeExport>(this);
    }
    IEnumerator UpdateSheet(IList<IList<object>> values, string range)
    {
        // Call the UpdateValues Coroutine
        ValueRange body = new ValueRange

        {
            Values = values

        };

        var google = GetGoogleSheets();
        var update = google.SheetsService.Service.Spreadsheets.Values.Append(body, google.SpreadSheetId, range);
        update.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
        yield return update.Execute();
        //Debug.Log($"{update.UpdatedCells} cells updated.");
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
        StartCoroutine(UpdateSheet(values, "Collected Data!A1"));

    }
    string GetExportEventID()
    {
        return "";
    }
    string GetTime()
    {
        return "";
    }

    GoogleSheets GetGoogleSheets()
    {
        var google = new GoogleSheets(sheetServiceProvider);
        google.SpreadSheetId = dataMigrationSettings.spreadsheetID;
        return google;
    }
}
