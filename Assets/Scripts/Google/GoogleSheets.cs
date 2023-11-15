using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Google.Apis.Requests;
using Google.Apis.Sheets.v4.Data;
using static Google.Apis.Sheets.v4.SpreadsheetsResource;
using Data = Google.Apis.Sheets.v4.Data;
using Google;
using Newtonsoft.Json;
using Request = Google.Apis.Sheets.v4.Data.Request;
using Google.Apis.Sheets.v4;
using Google.Apis.Auth.OAuth2;
using System.Collections;
using UnityEngine.Networking;


// always wrap with try/catch when calling methods for this class. Many conditions have to be satisfied for everything to be completed properly and therefore there are many potential errors.
public static class GoogleSheets 
{
    public static void OpenSheetInBrowser(string spreadSheetId)
    {
        Application.OpenURL($"https://docs.google.com/spreadsheets/d/{spreadSheetId}/");
    }

    public static void OpenSheetInBrowser(string spreadSheetId, int sheetId)
    {
        Application.OpenURL($"https://docs.google.com/spreadsheets/d/{spreadSheetId}/#gid={sheetId}");
    }

    // Creates a new sheet within the Spreadsheet
    // return any possible error message so we can show it as a helpbox in the data migration settings editor
    public static int AddSheet(SheetsService service, string spreadSheetId, string sheetName, NewSheetProperties newSheetProperties)
    {
        var createRequest = new Request()
        {
            AddSheet = new AddSheetRequest
            {
                Properties = new SheetProperties { Title = sheetName }
            }
        };
        var batchUpdateReqTask = SendBatchUpdateRequest(service, spreadSheetId, createRequest);
        var sheetId = batchUpdateReqTask.Replies[0].AddSheet.Properties.SheetId.Value;
        SetupSheet(service, spreadSheetId, sheetId, sheetName, newSheetProperties);
        return sheetId;
    }
    static void SetupSheet(SheetsService service, string spreadSheetId, int sheetId,string sheetName, NewSheetProperties newSheetProperties)
    {
        var requests = new List<Request>();

        requests.Add(SetTitleStyle(sheetId, newSheetProperties));

        if (newSheetProperties.HighlightDuplicateKeys)
            requests.Add(HighlightDuplicateKeys(sheetId, newSheetProperties));

        if (requests.Count > 0)
            SendBatchUpdateRequest(service,spreadSheetId, requests);
        // add titles 
        List<IList<object>> titiles = new List<IList<object>>();  titiles.Add(newSheetProperties.columnTitles);
        PushData(service, spreadSheetId,titiles, sheetName+"!A1");
    }
    public static IList<Dictionary<string, string>> PullData(SheetsService service, string spreadSheetId,int sheetId, bool skipFirstRow, int amountOfColumnsToRead)
    {

        string sheetName = GetSheetNameByID(service,spreadSheetId, sheetId);
        if (string.IsNullOrEmpty(sheetName))
        {
            Debug.LogError("Problem with sheet ID, could not get sheet name");
        }
        Debug.Log($"Pulling from Google sheet " + sheetName);
        string range = $"{sheetName}!{(skipFirstRow ? "A2" : "A1")}:{IndexToColumnName(amountOfColumnsToRead)}";
        var request = service.Spreadsheets.Values.Get(spreadSheetId, range);
        int rowCount = 0, columnCount = 0;
        ValueRange response = request.Execute();

        IList<IList<object>> values = response.Values;
        IList<string> columnTitles = GetColumnTitles(service,spreadSheetId,sheetId);
        if (columnTitles.Count < amountOfColumnsToRead)
        {
            Debug.LogError("Too little columns in sheet " + sheetName + ". Please check that you have set it up correctly");
            return null;
        }
        // keep all rows 
        // each row is a dictionary with the column name as key and cell content as value 
        IList<Dictionary<string, string>> table = new List<Dictionary<string, string>>();

        if (values != null && values.Count > 0)
        {
            foreach (IList<object> row in values)
            {
                //  create a dictionary for each row, with the column name as key and cell content as value 
                Dictionary<string, string> rowDic = new Dictionary<string, string>();
                columnCount = 0;
                foreach (object cell in row)
                {
                    if(!string.IsNullOrWhiteSpace(cell.ToString()))
                        rowDic[columnTitles[columnCount]] = cell.ToString().Trim();
                    columnCount++;
                }
                table.Add(rowDic);
                rowCount++;
            }

            Debug.Log($"Imported successfully.");
            return table;
        }
        else
        {
            Debug.LogError($"No sheet data available for {sheetId} in Spreadsheet {spreadSheetId}. Add some data in the Meta Data google sheet");
            return null;
        }

    }


    public static void PushData(SheetsService service, string spreadSheetId, List<IList<object>> data, string range)
    {
        // The new values to apply to the spreadsheet.
        var dataValueRange = new ValueRange();
        dataValueRange.Range = range;
        dataValueRange.Values = data;
        var request = service.Spreadsheets.Values.Append(dataValueRange, spreadSheetId, range);
        request.ValueInputOption = ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
        request.Execute();
    }
    public static string GetSheetNameByID(SheetsService sheetService,string spreadSheetId,int sheetId)
    {
        if (string.IsNullOrEmpty(spreadSheetId) || string.IsNullOrEmpty(sheetId.ToString()))
            throw new Exception($"The {nameof(spreadSheetId)} is required. Please assign a valid Spreadsheet Id to the property.");
        var spreadsheetInfoRequest = sheetService.Spreadsheets.Get(spreadSheetId);
        var sheetInfoReq = ExecuteRequest<Spreadsheet, GetRequest>(spreadsheetInfoRequest);

        foreach (var sheet in sheetInfoReq.Sheets)
        {
            if (sheet.Properties.SheetId.Value == sheetId)
                return sheet.Properties.Title;
        }
        return null;
    }
    public static string HandleGoogleSheetExceptions(GoogleApiException ex)
    {
        if (ex == null)
            return "";
        Debug.Log(ex.GetType());
        string errorMessage = ex.Error.Message;
        switch (errorMessage)
        {
            case "Requested entity was not found.":
                errorMessage = "Spreadsheet Id is not correct. Please enter the correct spreadsheet ID and try again.";
                break;
        }
        Debug.LogError(errorMessage);
        return errorMessage;
    }

    /// <summary>
    /// Returns all the column titles(values from the first row) for the selected sheet inside of the Spreadsheet with id <see cref="spreadSheetId"/>.
    /// This method requires the <see cref="sheetsService"/> to use OAuth authorization as it uses a data filter which reuires elevated authorization.
    /// </summary>
    /// <param name="sheetId">The sheet id.</param>
    /// <returns>All the </returns>
    public static IList<string> GetColumnTitles(SheetsService service,string spreadSheetId, int sheetId)
    {
        if (string.IsNullOrEmpty(spreadSheetId))
            throw new Exception($"{nameof(spreadSheetId)} is required.");

        var batchGetValuesByDataFilterRequest = new BatchGetValuesByDataFilterRequest
        {
            DataFilters = new DataFilter[1]
            {
                    new DataFilter
                    {
                        GridRange = new GridRange
                        {
                            SheetId = sheetId,
                            StartRowIndex = 0,
                            EndRowIndex = 1
                        }
                    }
            }
        };

        var request = service.Spreadsheets.Values.BatchGetByDataFilter(batchGetValuesByDataFilterRequest, spreadSheetId);
        var result = ExecuteRequest<BatchGetValuesByDataFilterResponse, ValuesResource.BatchGetByDataFilterRequest>(request);

        var titles = new List<string>();
        if (result?.ValueRanges?.Count > 0 && result.ValueRanges[0].ValueRange.Values != null)
        {
            foreach (var row in result.ValueRanges[0].ValueRange.Values)
            {
                foreach (var col in row)
                {
                    titles.Add(col.ToString().Trim()); // trim to remove leading or ending white spaces 
                }
            }
        }
        return titles;
    }



    static Request FreezeTitleRowAndColumn(int sheetId,int columnToFreeze)
    {
        return new Request()
        {
            UpdateSheetProperties = new UpdateSheetPropertiesRequest
            {
                Fields = "GridProperties.FrozenRowCount,GridProperties.FrozenColumnCount,",
                Properties = new SheetProperties
                {
                    SheetId = sheetId,
                    GridProperties = new GridProperties
                    {
                        FrozenRowCount = 1,
                        FrozenColumnCount = 1
                    }
                }
            }
        };
    }

    static Request HighlightDuplicateKeys(int sheetId, NewSheetProperties newSheetProperties)
    {
        return new Request
        {
            // Highlight duplicates in the A(Key) field
            AddConditionalFormatRule = new AddConditionalFormatRuleRequest
            {
                Rule = new ConditionalFormatRule
                {
                    BooleanRule = new BooleanRule
                    {
                        Condition = new BooleanCondition
                        {
                            Type = "CUSTOM_FORMULA",
                            Values = new[] { new ConditionValue { UserEnteredValue = "=countif(A:A;A1)>1" } }
                        },
                        Format = new CellFormat { BackgroundColor = UnityColorToDataColor(newSheetProperties.DuplicateKeyColor) }
                    },
                    Ranges = new[]
                    {
                            new GridRange
                            {
                                SheetId = sheetId,
                                EndColumnIndex = 1
                            }
                        }
                }
            },
        };
    }

    static Request SetTitleStyle(int sheetId, NewSheetProperties newSheetProperties)
    {
        return new Request
        {
            // Header style
            RepeatCell = new RepeatCellRequest
            {
                Fields = "*",
                Range = new GridRange
                {
                    SheetId = sheetId,
                    StartRowIndex = 0,
                    EndRowIndex = 1,
                },
                Cell = new CellData
                {
                    UserEnteredFormat = new CellFormat
                    {
                        BackgroundColor = UnityColorToDataColor(newSheetProperties.HeaderBackgroundColor),
                        TextFormat = new TextFormat
                        {
                            Bold = true,
                            ForegroundColor = UnityColorToDataColor(newSheetProperties.HeaderForegroundColor)
                        }
                    }
                }
            }
        };
    }
    static Data.Color UnityColorToDataColor(UnityEngine.Color color) => new Data.Color() { Red = color.r, Green = color.g, Blue = color.b, Alpha = color.a };


    internal static   BatchUpdateSpreadsheetResponse SendBatchUpdateRequest(SheetsService sheetsService, string spreadsheetId, IList<Request> requests)
    {
        var requestBody = new BatchUpdateSpreadsheetRequest { Requests = requests };
        var batchUpdateReq = sheetsService.Spreadsheets.BatchUpdate(requestBody, spreadsheetId);
        return batchUpdateReq.Execute();
    }

    internal static   BatchUpdateSpreadsheetResponse SendBatchUpdateRequest(SheetsService sheetsService,string spreadsheetId, params Request[] requests)
    {
        var requestBody = new BatchUpdateSpreadsheetRequest { Requests = requests };
        var batchUpdateReq = sheetsService.Spreadsheets.BatchUpdate(requestBody, spreadsheetId);
        return batchUpdateReq.Execute();
    }

    internal static   Task<TResponse> ExecuteRequestAsync<TResponse, TClientServiceRequest>(TClientServiceRequest req) where TClientServiceRequest : ClientServiceRequest<TResponse> => req.ExecuteAsync();
    internal  static   TResponse ExecuteRequest<TResponse, TClientServiceRequest>(TClientServiceRequest req) where TClientServiceRequest : ClientServiceRequest<TResponse> => req.Execute();
    /// Converts a column id value into its name. Column ids start at 0.
    /// E.G 0 = 'A', 1 = 'B', 26 = 'AA', 27 = 'AB'
    public static string IndexToColumnName(int index)
    {
        index++;
        string result = null;
        while (--index >= 0)
        {
            result = (char)('A' + index % 26) + result;
            index /= 26;
        }
        return result;
    }
}
