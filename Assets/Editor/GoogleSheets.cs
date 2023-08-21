using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Requests;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;
using UnityEngine;
using static Google.Apis.Sheets.v4.SpreadsheetsResource;
using Data = Google.Apis.Sheets.v4.Data;
using Object = UnityEngine.Object;
using Google.Apis.Sheets.v4;
using UnityEditor;

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
            throw new ArgumentNullException(nameof(provider));

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
    public void PullIntoStringTableCollection(int sheetId, IList<SheetColumn> columnMapping, ITaskReporter reporter = null, bool createUndo = false)
    {
        VerifyPushPullArguments(sheetId, collection, columnMapping, typeof(IPullKeyColumn));

        try
        {
            var modifiedAssets = collection.StringTables.Select(t => t as Object).ToList();
            modifiedAssets.Add(collection.SharedData);

            if (createUndo)
            {
                Undo.RegisterCompleteObjectUndo(modifiedAssets.ToArray(), $"Pull `{collection.TableCollectionName}` from Google sheets");
            }

            if (reporter != null && reporter.Started != true)
                reporter.Start($"Pull `{collection.TableCollectionName}` from Google sheets", "Preparing columns");

            // The response columns will be in the same order we request them, we need the key
            // before we can process any values so ensure the first column is the key column.
            var sortedColumns = columnMapping.OrderByDescending(c => c is IPullKeyColumn).ToList();

            // We can only use public API. No data filters.
            // We use a data filter when possible as it allows us to remove a lot of unnecessary information,
            // such as unneeded sheets and columns, which reduces the size of the response. A Data filter can only be used with OAuth authentication.
            reporter?.ReportProgress("Generating request", 0.1f);
            ClientServiceRequest<Spreadsheet> pullReq = UsingApiKey ? GeneratePullRequest() : GenerateFilteredPullRequest(sheetId, columnMapping);

            reporter?.ReportProgress("Sending request", 0.2f);
            var response = ExecuteRequest<Spreadsheet, ClientServiceRequest<Spreadsheet>>(pullReq);

            reporter?.ReportProgress("Validating response", 0.5f);

            // When using an API key we get all the sheets so we need to extract the one we are pulling from.
            var sheet = UsingApiKey ? response.Sheets?.FirstOrDefault(s => s?.Properties?.SheetId == sheetId) : response.Sheets[0];
            if (sheet == null)
                throw new Exception($"No sheet data available for {sheetId} in Spreadsheet {SpreadSheetId}.");

            // The data will be structured differently if we used a filter or not so we need to extract the parts we need.
            var pulledColumns = new List<(IList<RowData> rowData, int valueIndex)>();

            if (UsingApiKey)
            {
                // When getting the whole sheet all the columns are stored in a single Data. We need to extract the correct value index for each column.
                foreach (var sortedCol in sortedColumns)
                {
                    pulledColumns.Add((sheet.Data[0].RowData, sortedCol.ColumnIndex));
                }
            }
            else
            {
                if (sheet.Data.Count != columnMapping.Count)
                    throw new Exception($"Column mismatch. Expected a response with {columnMapping.Count} columns but only got {sheet.Data.Count}");

                // When using a filter each Data represents a single column.
                foreach (var d in sheet.Data)
                {
                    pulledColumns.Add((d.RowData, 0));
                }
            }

            MergePull(pulledColumns, collection, columnMapping, UsingApiKey, removeMissingEntries, reporter);

            // There is a bug that causes Undo to not set assets dirty (case 1240528) so we always set the asset dirty.
            modifiedAssets.ForEach(EditorUtility.SetDirty);

            LocalizationEditorSettings.EditorEvents.RaiseCollectionModified(this, collection);

            // Flush changes to disk.
            collection.SaveChangesToDisk();
        }
        catch (Exception e)
        {
            reporter?.Fail(e.Message);
            throw;
        }
    }

    void VerifyPushPullArguments(int sheetId, IList<SheetColumn> columnMapping)
    {
        if (string.IsNullOrEmpty(SpreadSheetId))
            throw new Exception($"{nameof(SpreadSheetId)} is required.");

        if (columnMapping == null)
            throw new ArgumentNullException(nameof(columnMapping));

        if (columnMapping.Count == 0)
            throw new ArgumentException("Must include at least 1 column.", nameof(columnMapping));

        if (columnMapping.Count(c => requiredKeyType.IsAssignableFrom(c.GetType())) != 1)
            throw new ArgumentException($"Must include 1 {requiredKeyType.Name}.", nameof(columnMapping));

        ThrowIfDuplicateColumnIds(columnMapping);
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
    ClientServiceRequest<Spreadsheet> GeneratePullRequest()
    {
        var request = SheetsService.Service.Spreadsheets.Get(SpreadSheetId);
        request.IncludeGridData = true;
        request.Fields = "sheets.properties.sheetId,sheets.properties.gridProperties.rowCount,sheets.data.rowData.values.formattedValue,sheets.data.rowData.values.note";
        return request;
    }

    ClientServiceRequest<Spreadsheet> GenerateFilteredPullRequest(int sheetId, IList<SheetColumn> columnMapping)
    {
        var getRequest = new GetSpreadsheetByDataFilterRequest { DataFilters = new List<DataFilter>() };

        foreach (var col in columnMapping)
        {
            getRequest.DataFilters.Add(new DataFilter
            {
                GridRange = new GridRange
                {
                    SheetId = sheetId,
                    StartRowIndex = 1, // Ignore header
                    StartColumnIndex = col.ColumnIndex,
                    EndColumnIndex = col.ColumnIndex + 1
                }
            });
        }

        var request = SheetsService.Service.Spreadsheets.GetByDataFilter(getRequest, SpreadSheetId);
        request.Fields = "sheets.properties.gridProperties.rowCount,sheets.data.rowData.values.formattedValue,sheets.data.rowData.values.note";
        return request;
    }
    void ThrowIfDuplicateColumnIds(IList<SheetColumn> columnMapping)
    {
        var ids = new HashSet<string>();
        foreach (var col in columnMapping)
        {
            if (ids.Contains(col.Column))
                throw new Exception($"Duplicate column found. The Column {col.Column} is already in use");
            ids.Add(col.Column);
        }
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
}
