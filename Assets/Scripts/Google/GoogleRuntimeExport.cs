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
    public SheetsServiceProvider sheetServiceProvider;
    static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };


    IEnumerator Update(IList<IList<object>> values, string range)
    {
        // Call the UpdateValues Coroutine
        ValueRange body = new ValueRange

        {
            Values = values

        };

        var google = GetGoogleSheets();
        var update = google.SheetsService.Service.Spreadsheets.Values.Update(body, google.SpreadSheetId, range);
        update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
        yield return update.Execute();
        //Debug.Log($"{update.UpdatedCells} cells updated.");

    }
    void Start()
    {
        var values = new List<IList<object>> { new List<object> { "Hello, Google Sheets!" , "I am trying"} };
        StartCoroutine(Update(values, "Collected Data!A1"));
    }
    void Awake()
    {
    }
    GoogleSheets GetGoogleSheets()
    {
        var google = new GoogleSheets(sheetServiceProvider);
        google.SpreadSheetId = dataMigrationSettings.spreadsheetID;
        return google;
    }
}
