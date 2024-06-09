using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Google.Apis.Sheets.v4;
using Google;
using System.Net.Http;

// I am making it a singleton because otherwise each time we load a scene and we reach start, the csv file will be reset.
// I need the CSV file to be created and reset only ONCE at the very start of the game
public class ExportManager : SimpleSingleton<ExportManager>, IRegistrableService
{
    [SerializeField]
    GameSettings gameSettings;
    [SerializeField]
    DataMigrationSettings dataMigrationSettings;
    [Header("Google")]
    string rangeEnd = "!A2";
    // I want to save my latest export data so I can execute it again in the case it failed the first time
    List<object> pendingPushData = null;
    [Header("CSV")]
    string CSVDirectory = Path.Combine(Application.dataPath,"CVSData");
    const char CSVseparator = ',';
    const string CSVExtension = ".csv";

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        ServiceLocator.Instance.Register<ExportManager>(this);
    }
    void Start()
    {

        gameSettings = ServiceLocator.Instance.Get<GameManager>().settings;
        dataMigrationSettings = ServiceLocator.Instance.Get<GameManager>().dataMigrationSettings;
        // do these at start to avoid delay on the first export call
        if(dataMigrationSettings.sentResultByEmail)
            VerifyCVSFile();
        // also, do a check at start that we have an Internet connection
    }

    //this function will collect the data we need to export and do a syntax check to make sure the data we are exporting matches the title columns on the spreadsheets
    List<object> CollectAndCheckData(TrackData trackData, string interactionId,string interactionName,string response, string lastDialogueNode)
    {
        List<object> data = new List<object>();
        DateTime exportTime = GetCETTime();
        foreach (var item in dataMigrationSettings.newSheetProperties.columnTitles)
        {
            switch (item)
            {
                case ("CET Time stamp"):
                    data.Add(exportTime.ToString());
                    break;
                case ("Export Event ID"):
                    data.Add(GetExportEventID(exportTime));
                    break;
                case ("Track ID"):
                    data.Add(trackData.trackID);
                    break;
                case ("Annotation"):
                    data.Add(response);
                    break;
                case ("Interaction ID"):
                    data.Add(interactionId);
                    break;
                case ("Interaction Name"):
                    data.Add(interactionName);
                    break;
                case ("Dialogue Node"):
                    data.Add(lastDialogueNode);
                    break;
                case ("User Name"):
                    data.Add(gameSettings.playerName);
                    break;
                case ("User ID"):
                    data.Add(gameSettings.playerId);
                    break;
                case ("Game Session Index"):
                    data.Add(gameSettings.gameSessionIndex.ToString());
                    break;
                case ("Build ID"):
                    data.Add(gameSettings.buildID);
                    break;
                case ("Configuration ID"):
                    data.Add(gameSettings.configurationID);
                    break;
                case ("Time of Day"):
                    data.Add(GetTimeOfDay(exportTime));
                    break;
                default: // if it doesn't match any case
                    Debug.LogError($"a column title {item} in the export sheet does not match any data. Please ensure that the list of column titles under Data Migration Settings -> New Spreadsheet properties are correct.");
                    break;
            }
        }
        return data;
    }

    public void ExportData(TrackData trackData, MusicDialogueData dialogueData,string lastDialogueNode)
    {
        List<object> dataToExport = CollectAndCheckData(trackData, dialogueData.GlobalID,dialogueData.interactionName, dialogueData.emotionToInvoke.ToString(), lastDialogueNode);
        if (dataToExport.Count != dataMigrationSettings.newSheetProperties.columnTitles.Length) // the data we have collected does not match what we require for the spreadsheet
            return;
        pendingPushData = dataToExport;
        if (dataMigrationSettings.sentResultByEmail)
            WriteToCSV(dataToExport);
        ExportToGoogleSheets(dataToExport);

    }

    // this will be called if a push to google sheets was failed bc of no internet connection and we want to resume the last call.
    public void ExportPendingData()
    {
        if(pendingPushData != null)
            ExportToGoogleSheets(pendingPushData);
    }

    #region GOOGLE EXPORT
    void ExportToGoogleSheets(IList<object> dataToExport)
    {

        var values = new List<IList<object>> { dataToExport };
        SheetsService service = SheetsServiceProvider.ConnectWithServiceAccountKey(dataMigrationSettings);
        try
        {
            GoogleSheets.PushData(service,dataMigrationSettings.spreadsheetID, values,  GoogleSheets.GetSheetNameByID(service, dataMigrationSettings.spreadsheetID, dataMigrationSettings.exportSheetID) + rangeEnd);
            Debug.Log("Recorded the following data to Google Sheets: " + string.Join(" ,", dataToExport));
            // mark that nothing is pending
            pendingPushData = null;
        }
        // first, catch any HTTP exception i.e. check that we are connected to the Internet 
        catch(HttpRequestException e)
        {
            ServiceLocator.Instance.Get<InternetConnectionManager>().NoConnectionDetected();
        }
        catch(GoogleApiException e)
        {
            GoogleSheets.HandleGoogleSheetExceptions(e);
        }
    }

    #endregion
    #region CSV EXPORT

    string GetCSVPath()
    {
        return Path.Combine(CSVDirectory, GetExperimentID() + CSVExtension);
    }
    void WriteToCSV(List<object> dataToExport)
    {
        using (var writer = new StreamWriter(GetCSVPath(), true))
        {
            writer.WriteLine(string.Join(CSVseparator, dataToExport));
        }
        Debug.Log("Recorded the following data to CSV: " + string.Join(" ,", dataToExport));
    }
    void VerifyCVSFile() // check that the CSV file exists, if it doesn't then create one
    {
        if (!File.Exists(GetCSVPath()))
        {
            CreateCSVWithTitles(GetCSVPath());
        }
    }
    void CreateCSVWithTitles(string path)
    {
        try
        {
            //create the folder
            if (!Directory.Exists(CSVDirectory))
            {
                Directory.CreateDirectory(CSVDirectory);
            }
            else // I am deleting everything where our asset will be placed to ensure no old  data remains 
            {
                DirectoryInfo directory = new DirectoryInfo(path);
            }
            // add the titles first, false to remove everything that's already there is something is there 
            using (var writer = new StreamWriter(path, false))
            {
                writer.WriteLine(string.Join(CSVseparator, dataMigrationSettings.newSheetProperties.columnTitles));
            }
            Debug.Log("Created CSV in path: " + path);
        }
        catch (Exception e)
        {
            Debug.LogError("Could not write and save CSV. Error is: " + e.Message);
        }
    }

    // returns true for success and false for failure 
    public bool SendCSVByEmail()
    {
        if(!dataMigrationSettings.sentResultByEmail)
        {
            Debug.LogWarning("Send CSV by email is set to false in the data migration settings.");
            return false;
        }
        // add configuration name to the email 
        try
        {
            if (string.IsNullOrEmpty(dataMigrationSettings.researchersEmail))
            {
                Debug.LogError("Email to send to is unknown. Please set it up in the Data Migration Settings window.");
            }
            EmailSender.SendEmail("Collected Data for experiment " + GetExperimentID(), BuildEmailBody(), dataMigrationSettings.researchersEmail, GetCSVPath()); ;
            Debug.Log("Successfully send collected data CSV to email");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Could not send CSV by email. Error is: " + e.Message);
            return false;

        }
    }
    string BuildEmailBody()
    {
        string body = "Enclosed within this email, you will find the data gathered during experiment with the following parameters:"+ Environment.NewLine;
        foreach (var item in GetExperimentData())
        {
            body+= $"- {item.Key}: {item.Value}";
            body += Environment.NewLine;
        }
        return body;
    }
    #endregion
    #region VALUES
    string GetExportEventID(DateTime exportTime)
    {
        // in UUID format 
        // YYYY:MM:DD:HH:MM:SS_PlayerID_GameSessionIndex_cc867280-68a7-4737-8676-0f14d2ae1b1f for data point id
        Guid randomGuid = Guid.NewGuid();
        string randomGuidString = randomGuid.ToString();
        string formattedDateTime = exportTime.ToString("yyyy:MM:dd:HH:mm:ss");
        return formattedDateTime + "_" + gameSettings.playerId + "_" + gameSettings.gameSessionIndex + "_" + randomGuidString;
    }
    string GetExperimentID()
    {
        return gameSettings.configurationID + "_" + gameSettings.playerId + "_" + gameSettings.gameSessionIndex + "_" + Application.version;
    }

    Dictionary<string, string> GetExperimentData()
    {
        Dictionary<string, string> data = new Dictionary<string, string> { { "Configuration ID" , gameSettings.configurationID }, {"Application Version" , Application.version },{ "User Name", gameSettings.playerName} ,{ "User ID", gameSettings.playerId }, { "Game Session Index" , gameSettings.gameSessionIndex.ToString() } };
        return data;
    }
    DateTime GetCETTime()
    {
        try
        {
            TimeZoneInfo cetTimeZone;
            // Define the CET time zone
            try
            {
                cetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("CET");

            }
            catch
            {
                cetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

            }
            // Get the current UTC time
            DateTime utcNow = DateTime.UtcNow;
            // Convert UTC time to CET time
            return TimeZoneInfo.ConvertTimeFromUtc(utcNow, cetTimeZone);
        }
        catch
        {
            Debug.LogError("CET time could not be found. returned local time zone");
            return DateTime.Now;
        }
    }

    string GetTimeOfDay(DateTime time)
    {
        int hour = time.Hour;

        if (hour >= 5 && hour < 12)
        {
            return "Morning";
        }
        else if (hour >= 12 && hour < 17)
        {
            return "Afternoon";
        }
        else if (hour >= 17 && hour < 21)
        {
            return "Evening";
        }
        else
        {
            return "Night";
        }
    }
    #endregion
}
