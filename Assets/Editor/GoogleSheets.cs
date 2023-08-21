using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Requests;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;
using static Google.Apis.Sheets.v4.SpreadsheetsResource;
using Data = Google.Apis.Sheets.v4.Data;
using Object = UnityEngine.Object;
using UnityEditor;
using UnityEngine.Profiling;
using System.Data;
using Unity.Collections.LowLevel.Unsafe;

public class GoogleSheets : MonoBehaviour
{
    /// <summary>
    /// The sheets provider is responsible for providing the SheetsService and configuring the type of access.
    /// <seealso cref="SheetsServiceProvider"/>.
    /// </summary>
    public IGoogleSheetsService SheetsService { get; private set; }

    /// <summary>
    /// The Id of the Google Sheet. This can be found by examining the url:
    /// https://docs.google.com/spreadsheets/d/<b>SpreadsheetId</b>/edit#gid=<b>SheetId</b>
    /// Further information can be found <see href="https://developers.google.com/sheets/api/guides/concepts#spreadsheet_id">here.</see>
    /// </summary>
    public string SpreadSheetId { get; set; }

    /// <summary>
    /// Creates a new instance of a GoogleSheets connection.
    /// </summary>
    /// <param name="provider">The Google Sheets service provider. See <see cref="SheetsServiceProvider"/> for a default implementation.</param>
    public GoogleSheets(IGoogleSheetsService provider)
    {
        if (provider == null)
            Debug.LogError("No provider");
        SheetsService = provider;
    }

    /// <summary>
    /// Opens the spreadsheet in a browser.
    /// </summary>
    /// <param name="spreadSheetId"></param>
    public static void OpenSheetInBrowser(string spreadSheetId) => Application.OpenURL($"https://docs.google.com/spreadsheets/d/{spreadSheetId}/");

    /// <summary>
    /// Opens the spreadsheet with the sheet selected in a browser.
    /// </summary>
    /// <param name="spreadSheetId"></param>
    /// <param name="sheetId"></param>
    public static void OpenSheetInBrowser(string spreadSheetId, int sheetId) => Application.OpenURL($"https://docs.google.com/spreadsheets/d/{spreadSheetId}/#gid={sheetId}");

    /// <summary>
    /// Creates a new sheet within the Spreadsheet with the id <see cref="SpreadSheetId"/>.
    /// </summary>
    /// <param name="title">The title for the new sheet</param>
    /// <param name="newSheetProperties">The settings to apply to the new sheet.</param>
    /// <returns>The new sheet id.</returns>
    public int AddSheet(string title, NewSheetProperties newSheetProperties)
    {
        if (string.IsNullOrEmpty(SpreadSheetId))
            throw new Exception($"{nameof(SpreadSheetId)} is required. Please assign a valid Spreadsheet Id to the property.");

        if (newSheetProperties == null)
            throw new ArgumentNullException(nameof(newSheetProperties));

        var createRequest = new Request()
        {
            AddSheet = new AddSheetRequest
            {
                Properties = new SheetProperties { Title = title }
            }
        };

        var batchUpdateReqTask = SendBatchUpdateRequest(SpreadSheetId, createRequest);
        var sheetId = batchUpdateReqTask.Replies[0].AddSheet.Properties.SheetId.Value;
        SetupSheet(SpreadSheetId, sheetId, newSheetProperties);
        return sheetId;
    }
    public IList<Dictionary<string, string>> PullData(int sheetId, bool skipFirstRow, int amountOfColumnsToRead)
    {

        string sheetName = GetSheetNameByID(sheetId);
        if (string.IsNullOrEmpty(sheetName))
        {
            Debug.LogError("Problem with sheet ID, could not get sheet name");
        }
        Debug.Log($"Pulling from Google sheet " + sheetName);
        string range = $"{sheetName}!{(skipFirstRow ? "A2" : "A1")}:{IndexToColumnName(amountOfColumnsToRead)}";
        var request = SheetsService.Service.Spreadsheets.Values.Get(SpreadSheetId, range);
        int rowCount = 0, columnCount = 0;
        ValueRange response = request.Execute();

        IList<IList<object>> values = response.Values;
        IList<string> columnTitles = GetColumnTitles(sheetId);
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
            Debug.LogError($"No sheet data available for {sheetId} in Spreadsheet {SpreadSheetId}. Add some data in the Meta Data google sheet");
            return null;
        }

    }
    /// <summary>
    /// Returns a list of all the sheets in the Spreadsheet with the id <see cref="SpreadSheetId"/>.
    /// </summary>
    /// <returns>The sheets names and id's.</returns>
    public List<(string name, int id)> GetSheets()
    {
        if (string.IsNullOrEmpty(SpreadSheetId))
            throw new Exception($"The {nameof(SpreadSheetId)} is required. Please assign a valid Spreadsheet Id to the property.");

        var sheets = new List<(string name, int id)>();
        var spreadsheetInfoRequest = SheetsService.Service.Spreadsheets.Get(SpreadSheetId);
        var sheetInfoReq = ExecuteRequest<Spreadsheet, GetRequest>(spreadsheetInfoRequest);

        foreach (var sheet in sheetInfoReq.Sheets)
        {
            sheets.Add((sheet.Properties.Title, sheet.Properties.SheetId.Value));
        }

        return sheets;
    }
    public string GetSheetNameByID(int sheetId)
    {
        if (string.IsNullOrEmpty(SpreadSheetId) || string.IsNullOrEmpty(sheetId.ToString()))
            throw new Exception($"The {nameof(SpreadSheetId)} is required. Please assign a valid Spreadsheet Id to the property.");
        var spreadsheetInfoRequest = SheetsService.Service.Spreadsheets.Get(SpreadSheetId);
        var sheetInfoReq = ExecuteRequest<Spreadsheet, GetRequest>(spreadsheetInfoRequest);

        foreach (var sheet in sheetInfoReq.Sheets)
        {
            if (sheet.Properties.SheetId.Value == sheetId)
                return sheet.Properties.Title;
        }
        return null;
    }
    /// <summary>
    /// Returns all the column titles(values from the first row) for the selected sheet inside of the Spreadsheet with id <see cref="SpreadSheetId"/>.
    /// This method requires the <see cref="SheetsService"/> to use OAuth authorization as it uses a data filter which reuires elevated authorization.
    /// </summary>
    /// <param name="sheetId">The sheet id.</param>
    /// <returns>All the </returns>
    public IList<string> GetColumnTitles(int sheetId)
    {
        if (string.IsNullOrEmpty(SpreadSheetId))
            throw new Exception($"{nameof(SpreadSheetId)} is required.");

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

        var request = SheetsService.Service.Spreadsheets.Values.BatchGetByDataFilter(batchGetValuesByDataFilterRequest, SpreadSheetId);
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
    /// <summary>
    /// Asynchronous version of <see cref="GetRowCount"/>
    /// <inheritdoc cref="GetRowCount"/>
    /// </summary>
    /// <param name="sheetId">The sheet to get the row count from</param>
    /// <returns>The row count for the sheet.</returns>
    public async Task<int> GetRowCountAsync(int sheetId)
    {
        var rowCountRequest = GenerateGetRowCountRequest(sheetId);
        var task = ExecuteRequestAsync<Spreadsheet, GetByDataFilterRequest>(rowCountRequest);
        await task.ConfigureAwait(true);

        if (task.Result.Sheets == null || task.Result.Sheets.Count == 0)
            throw new Exception($"No sheet data available for {sheetId} in Spreadsheet {SpreadSheetId}.");
        return task.Result.Sheets[0].Properties.GridProperties.RowCount.Value;
    }

    /// <summary>
    /// Returns the total number of rows in the sheet inside of the Spreadsheet with id <see cref="SpreadSheetId"/>.
    /// This method requires the <see cref="SheetsService"/> to use OAuth authorization as it uses a data filter which reuires elevated authorization.
    /// </summary>
    /// <param name="sheetId">The sheet to get the row count from.</param>
    /// <returns>The row count for the sheet.</returns>
    public int GetRowCount(int sheetId)
    {
        var rowCountRequest = GenerateGetRowCountRequest(sheetId);
        var response = ExecuteRequest<Spreadsheet, GetByDataFilterRequest>(rowCountRequest);

        if (response.Sheets == null || response.Sheets.Count == 0)
            throw new Exception($"No sheet data available for {sheetId} in Spreadsheet {SpreadSheetId}.");
        return response.Sheets[0].Properties.GridProperties.RowCount.Value;
    }

    GetByDataFilterRequest GenerateGetRowCountRequest(int sheetId)
    {
        if (string.IsNullOrEmpty(SpreadSheetId))
            throw new Exception($"{nameof(SpreadSheetId)} is required.");

        return SheetsService.Service.Spreadsheets.GetByDataFilter(new GetSpreadsheetByDataFilterRequest
        {
            DataFilters = new DataFilter[]
            {
                    new DataFilter
                    {
                        GridRange = new GridRange
                        {
                            SheetId = sheetId,
                        },
                    },
            },
        }, SpreadSheetId);
    }
  

    void SetupSheet(string spreadSheetId, int sheetId, NewSheetProperties newSheetProperties)
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

    Request FreezeTitleRowAndKeyColumn(int sheetId)
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

    Request HighlightDuplicateKeys(int sheetId, NewSheetProperties newSheetProperties)
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

    Request SetTitleStyle(int sheetId, NewSheetProperties newSheetProperties)
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

    Request ResizeRow(int sheetId, int newSize)
    {
        return new Request
        {
            UpdateSheetProperties = new UpdateSheetPropertiesRequest
            {
                Properties = new SheetProperties
                {
                    SheetId = sheetId,
                    GridProperties = new GridProperties
                    {
                        RowCount = newSize
                    },
                },
                Fields = "gridProperties.rowCount"
            }
        };
    }
    static Data.Color UnityColorToDataColor(UnityEngine.Color color) => new Data.Color() { Red = color.r, Green = color.g, Blue = color.b, Alpha = color.a };

    internal protected virtual Task<BatchUpdateSpreadsheetResponse> SendBatchUpdateRequestAsync(string spreadsheetId, IList<Request> requests)
    {
        var service = SheetsService.Service;
        var requestBody = new BatchUpdateSpreadsheetRequest { Requests = requests };
        var batchUpdateReq = service.Spreadsheets.BatchUpdate(requestBody, spreadsheetId);
        return batchUpdateReq.ExecuteAsync();
    }

    internal protected virtual BatchUpdateSpreadsheetResponse SendBatchUpdateRequest(string spreadsheetId, IList<Request> requests)
    {
        var service = SheetsService.Service;
        var requestBody = new BatchUpdateSpreadsheetRequest { Requests = requests };
        var batchUpdateReq = service.Spreadsheets.BatchUpdate(requestBody, spreadsheetId);
        return batchUpdateReq.Execute();
    }

    internal protected virtual BatchUpdateSpreadsheetResponse SendBatchUpdateRequest(string spreadsheetId, params Request[] requests)
    {
        var service = SheetsService.Service;
        var requestBody = new BatchUpdateSpreadsheetRequest { Requests = requests };
        var batchUpdateReq = service.Spreadsheets.BatchUpdate(requestBody, spreadsheetId);
        return batchUpdateReq.Execute();
    }

    internal protected virtual Task<TResponse> ExecuteRequestAsync<TResponse, TClientServiceRequest>(TClientServiceRequest req) where TClientServiceRequest : ClientServiceRequest<TResponse> => req.ExecuteAsync();
    internal protected virtual TResponse ExecuteRequest<TResponse, TClientServiceRequest>(TClientServiceRequest req) where TClientServiceRequest : ClientServiceRequest<TResponse> => req.Execute();
    /// <summary>
    /// Converts a column id value into its name. Column ids start at 0.
    /// E.G 0 = 'A', 1 = 'B', 26 = 'AA', 27 = 'AB'
    /// </summary>
    /// <param name="index">Id of the column starting at 0('A').</param>
    /// <returns>The column name or null.</returns>
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

    /// <summary>
    /// Convert a column name to its id value.
    /// E.G 'A' = 0, 'B' = 1, 'AA' = 26, 'AB' = 27
    /// </summary>
    /// <param name="name">The name of the column, case insensitive.</param>
    /// <returns>The column index or 0.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static int ColumnNameToIndex(string name)
    {
        int power = 1;
        int index = 0;
        for (int i = name.Length - 1; i >= 0; --i)
        {
            char c = name[i];
            char a = char.IsUpper(c) ? 'A' : 'a';
            int charId = c - a + 1;

            if (charId < 1 || charId > 26)
                throw new ArgumentException($"Invalid Column Name '{name}'. Must only contain values 'A-Z'. Item at Index {i} was invalid '{c}'", nameof(name));

            index += charId * power;
            power *= 26;
        }
        return index - 1;
    }
}
