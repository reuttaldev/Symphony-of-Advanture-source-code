using System.Collections.Generic;
using UnityEngine;

public class RuntimeExportManager : MonoBehaviour, IRegistrableService
{
    [SerializeField]
    DataMigrationSettings settings;
    [SerializeField]
    PlayerData playerData;
    string range = "Collected Data!A2";

    void Awake()
    {
        ServiceLocator.Instance.Register<RuntimeExportManager>(this);
    }

    #region GOOGLE EXPORT
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
        GoogleSheets.PushData(settings.spreadsheetID, settings.exportSheetID, values,range);
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
