using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using CsvHelper;


// I am making it a singleton because otherwise each time we load a scene and we reach start, the csv file will be reset.
// I need the CSV file to be created and reset only ONCE at the very start of the game
public class ExportManager : SimpleSingleton<ExportManager>, IRegistrableService
{    
    [Header("Google")]
    [SerializeField]
    DataMigrationSettings settings;
    [SerializeField]
    PlayerData playerData;
    string range = "Collected Data!A2";

    [Header("CSV")]
    string CSVDirectory = Application.dataPath;
    string CSVPath;
    string CSVName = "CVSExport.csv";
    char CSVseparator = ',';

    void Awake()
    {
        ServiceLocator.Instance.Register<ExportManager>(this);
    }


    public void ExportData(TrackData trackData, string interactionId, Emotions response)
    {
        DateTime time = GetCETTime();

        // store all data we want to export in an array so we can send it later to where ever we need to
        string[] dataToExport = {
            time.ToString(),
            GenerateDataPointID(),
            trackData.trackID,
            response.ToString(),
            interactionId,
            playerData.playerName,
            playerData.playerID,
            GetSessionIndex().ToString(),
            GetConfigurationID(),
            GetTimeOfDay(time)
        };

        ExportToGoogleSheets(dataToExport);
        ExportToCSV(dataToExport);
    }



    #region GOOGLE EXPORT
    void ExportToGoogleSheets(IList<object> dataToExport)
    {
        var values = new List<IList<object>> { dataToExport };
        bool success = GoogleSheets.PushData(settings.spreadsheetID, settings.exportSheetID, values,range);
        if(success) 
            Debug.Log("Recorded the following data to Google Sheets: " + string.Join(" ,", dataToExport));
    }

    void AddTitlesToGoogleSheets()
    {

    }

    #endregion
    #region CSV EXPORT

    void Start()
    {
        CreateCSVWithTitles();
        DontDestroyOnLoad(this);
    }
    void ExportToCSV(string[] dataToExport)
    {
        VerifyFile();
        using (var writer = new StreamWriter(CSVPath, true))
        {
            writer.WriteLine(string.Join(CSVseparator, dataToExport));
        }
        Debug.Log("Recorded the following data to CSV: " + string.Join(" ,", dataToExport));
    }
    void VerifyFile() // check that the CSV file exists, if it doesn't then create one
    {
        if(!File.Exists(CSVPath))
        {
            CreateCSVWithTitles();
        }
    }
    void CreateCSVWithTitles()
    {
        try
        {
            // add the titles first, remove everything that's already there is something is there 
            CSVName = ChooseNameForCSV();
            CSVPath = Path.Combine(CSVDirectory, CSVName);
            using (var writer = new StreamWriter(CSVPath, false))
            {
                writer.WriteLine(string.Join(CSVseparator, settings.columnTitles));
            }
            Debug.Log("Created CSV in path: " + CSVPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Could not write and save CSV. Error is: " + e.Message);
        }
    }

    string  ChooseNameForCSV()
    {
        return  "CVSExport"+".csv";
    }
    public void SendCSVByEmail()
    {
        VerifyFile();
        try 
        {
            Debug.Log("Successfully send collected data CSV to email");
        }
        catch (Exception e) 
        {
            Debug.LogError("Could not send CSV by email. Error is: " + e.Message);

        }
    }
    #endregion
    #region VALUES

    
    // Generate some method that will return a unique ID for every time you export data
    string GenerateDataPointID()
    {
        return "";
    }

    // return the amount of times player has replayed the game 
    int GetSessionIndex()
    {
        return 1;
    }
    string GetConfigurationID()
    {
        return "00001";
    }

    DateTime GetCETTime()
    {
        // Define the CET time zone
        TimeZoneInfo cetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        // Get the current UTC time
        DateTime utcNow = DateTime.UtcNow;
        // Convert UTC time to CET time
        return TimeZoneInfo.ConvertTimeFromUtc(utcNow, cetTimeZone);
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
