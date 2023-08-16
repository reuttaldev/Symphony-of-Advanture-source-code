using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


// this script is in charge of sending user's responses to Google spreadsheets for data collection of the user's response 
public class ExportManager :MonoBehaviour, IRegistrableService
{
    PlayerData playerData;
    string spreadsheetURL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSf2Kf3ArIg6J9ln6p_IVUcXidNdS4Cx7nNHMVL0Yvap2xVCTw/formResponse";
    void Awake()
    {
        ManagerLocator.Instance.Register<ExportManager>(this);
    }

    public void ExportTrackData(string interactionID, TrackData track)
    {
        if (string.IsNullOrEmpty(playerData.playerID) || string.IsNullOrEmpty(playerData.playerName) || string.IsNullOrEmpty(track.GetTrackId()) || string.IsNullOrEmpty(track.GetUserResponse())) 
        {
            Debug.LogError("Trying to export track data with one or more empty attributes");
        }
        string eventID;
        StartCoroutine(Publish(eventID, interactionID, playerData.playerName, playerData.playerID,track.GetTrackId(), track.GetUserResponse()));
    }
    IEnumerator Publish(string eventID, string interactionID, string playerName,string playerID, string trackId, string response)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.1661633331", eventID);
        form.AddField("entry.575476485", interactionID);
        form.AddField("entry.466456098", playerID);
        form.AddField("entry.1064789443", playerName);
        form.AddField("entry.1415367845", trackId);
        form.AddField("entry.2040363894", response);

        //UnityWebRequest handles the flow of HTTP communication with web servers
        using (UnityWebRequest www = UnityWebRequest.Post(spreadsheetURL, form))
        {
            //This is a coroutine and not a method to allow game to continue running without waiting for the response.
            // it will run like a function until the yield return, then it will wait for a response or a result (pauses its execution and returns control to the main Unity update loop, so it doesn't block the CPU while waiting)
            // once response has arrived, code after the yield return will execute
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Successfully updated spreadsheet");
            }

        }
    }
}
