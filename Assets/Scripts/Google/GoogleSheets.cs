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
    public static int AddSheet(string spreadSheetId, string title, NewSheetProperties newSheetProperties)
    {
        if (string.IsNullOrEmpty(spreadSheetId))
            throw new Exception($"{nameof(spreadSheetId)} is required. Please assign a valid Spreadsheet Id to the property.");

        if (newSheetProperties == null)
            throw new ArgumentNullException(nameof(newSheetProperties));

        var createRequest = new Request()
        {
            AddSheet = new AddSheetRequest
            {
                Properties = new SheetProperties { Title = title }
            }
        };
        try
        {
            var batchUpdateReqTask = SendBatchUpdateRequest(spreadSheetId, createRequest);
            var sheetId = batchUpdateReqTask.Replies[0].AddSheet.Properties.SheetId.Value;
            SetupSheet(spreadSheetId, sheetId, newSheetProperties);
            return sheetId;
        }
        catch (GoogleApiException ex)
        {
            Debug.LogError(ex.Message);
        }
        return 0;

    }

    static void SetupSheet(string spreadSheetId, int sheetId, NewSheetProperties newSheetProperties)
    {
        var requests = new List<Request>();

        requests.Add(SetTitleStyle(sheetId, newSheetProperties));

        if (newSheetProperties.FreezeTitleRowAndKeyColumn)
            requests.Add(FreezeTitleRowAndKeyColumn(sheetId));

        if (newSheetProperties.HighlightDuplicateKeys)
            requests.Add(HighlightDuplicateKeys(sheetId, newSheetProperties));

        if (requests.Count > 0)
            SendBatchUpdateRequest(spreadSheetId, requests);
    }
    public static IList<Dictionary<string, string>> PullData(string spreadSheetId,int sheetId, bool skipFirstRow, int amountOfColumnsToRead)
    {

        string sheetName = GetSheetNameByID(spreadSheetId, sheetId);
        if (string.IsNullOrEmpty(sheetName))
        {
            Debug.LogError("Problem with sheet ID, could not get sheet name");
        }
        Debug.Log($"Pulling from Google sheet " + sheetName);
        string range = $"{sheetName}!{(skipFirstRow ? "A2" : "A1")}:{IndexToColumnName(amountOfColumnsToRead)}";
        var request = SheetsServiceProvider.Service.Spreadsheets.Values.Get(spreadSheetId, range);
        int rowCount = 0, columnCount = 0;
        ValueRange response = request.Execute();

        IList<IList<object>> values = response.Values;
        IList<string> columnTitles = GetColumnTitles(spreadSheetId,sheetId);
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
                        rowDic[columnTitles[columnCount]] = cell.ToString();
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
    public static void PushData(string spreadSheetId, int sheetId, List<IList<object>> data,string range)
    {
        // The new values to apply to the spreadsheet.

        UpdateValuesResponse result = null;
        var dataValueRange = new ValueRange();
        dataValueRange.Range = range;
        dataValueRange.Values = data;
        var request = SheetsServiceProvider.Service.Spreadsheets.Values.Append(dataValueRange, spreadSheetId, range);
        request.ValueInputOption =ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
        try
        {
            // Data.BatchUpdateValuesResponse response = await request.ExecuteAsync(); // For async 
            AppendValuesResponse response = request.Execute();
            Debug.Log(JsonConvert.SerializeObject(response));
        }
        catch (Exception ex) 
        {
            Debug.LogError(ex.Message);
        }
    }
    public static string GetSheetNameByID(string spreadSheetId,int sheetId)
    {
        if (string.IsNullOrEmpty(spreadSheetId) || string.IsNullOrEmpty(sheetId.ToString()))
            throw new Exception($"The {nameof(spreadSheetId)} is required. Please assign a valid Spreadsheet Id to the property.");
        var spreadsheetInfoRequest = SheetsServiceProvider.Service.Spreadsheets.Get(spreadSheetId);
        var sheetInfoReq = ExecuteRequest<Spreadsheet, GetRequest>(spreadsheetInfoRequest);

        foreach (var sheet in sheetInfoReq.Sheets)
        {
            if (sheet.Properties.SheetId.Value == sheetId)
                return sheet.Properties.Title;
        }
        return null;
    }
    /// <summary>
    /// Returns all the column titles(values from the first row) for the selected sheet inside of the Spreadsheet with id <see cref="spreadSheetId"/>.
    /// This method requires the <see cref="sheetsService"/> to use OAuth authorization as it uses a data filter which reuires elevated authorization.
    /// </summary>
    /// <param name="sheetId">The sheet id.</param>
    /// <returns>All the </returns>
    public static IList<string> GetColumnTitles(string spreadSheetId, int sheetId)
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

        var request = SheetsServiceProvider.Service.Spreadsheets.Values.BatchGetByDataFilter(batchGetValuesByDataFilterRequest, spreadSheetId);
        var result = ExecuteRequest<BatchGetValuesByDataFilterResponse, ValuesResource.BatchGetByDataFilterRequest>(request);

        var titles = new List<string>();
        if (result?.ValueRanges?.Count > 0 && result.ValueRanges[0].ValueRange.Values != null)
        {
            foreach (var row in result.ValueRanges[0].ValueRange.Values)
            {
                foreach (var col in row)
                {
                    titles.Add(col.ToString());
                }
            }
        }
        return titles;
    }



    static Request FreezeTitleRowAndKeyColumn(int sheetId)
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

    internal static   Task<BatchUpdateSpreadsheetResponse> SendBatchUpdateRequestAsync(string spreadsheetId, IList<Request> requests)
    {
        var service = SheetsServiceProvider.Service;
        var requestBody = new BatchUpdateSpreadsheetRequest { Requests = requests };
        var batchUpdateReq = service.Spreadsheets.BatchUpdate(requestBody, spreadsheetId);
        return batchUpdateReq.ExecuteAsync();
    }

    internal static   BatchUpdateSpreadsheetResponse SendBatchUpdateRequest(string spreadsheetId, IList<Request> requests)
    {
        var service = SheetsServiceProvider.Service;
        var requestBody = new BatchUpdateSpreadsheetRequest { Requests = requests };
        var batchUpdateReq = service.Spreadsheets.BatchUpdate(requestBody, spreadsheetId);
        return batchUpdateReq.Execute();
    }

    internal static   BatchUpdateSpreadsheetResponse SendBatchUpdateRequest(string spreadsheetId, params Request[] requests)
    {
        var service = SheetsServiceProvider.Service;
        var requestBody = new BatchUpdateSpreadsheetRequest { Requests = requests };
        var batchUpdateReq = service.Spreadsheets.BatchUpdate(requestBody, spreadsheetId);
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
