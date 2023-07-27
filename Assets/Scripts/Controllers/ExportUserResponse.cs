using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


// this script is in charge of sending user's responses to Google spreadsheets for data collection
public class ExportUserResponse : SimpleSingleton<ExportUserResponse>

{
    string playerName;
    int playerIndex;
    string spreadsheetURL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSf2Kf3ArIg6J9ln6p_IVUcXidNdS4Cx7nNHMVL0Yvap2xVCTw/formResponse";
    public void SetPlayerInfo(int playerIndex, string playerName)
    {
        this.playerIndex = playerIndex;
        this.playerName = playerName;
    }
    public void Export( int trackIndex, string trackName, string response)
    {
        StartCoroutine(Publish(trackIndex,trackName, response));
    }
    IEnumerator Publish( int trackIndex, string trackName,string response)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.1661633331", trackIndex);
        form.AddField("entry.837397244", trackName);
        form.AddField("entry.1064789443", playerIndex);
        form.AddField("entry.575476485", playerName);
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
