using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private TrackData currentlyPlayingTrack;
    // a dictionary of all currently available songs for the player to hear. key is track id and value is it's data container 
    private Dictionary<string,TrackData> library = new Dictionary<string,TrackData>();
    // all possible tracks that can be used in the game
    private Dictionary<string,TrackData> allTracks = new Dictionary<string,TrackData>();
    TrackData GetCurrentlyPlaying()
    {
        return currentlyPlayingTrack;
    }
    void playSound(int soundID)
    {
        // Play sound using console audio api...
    }

    void stopSound(int soundID)
    {
        // Stop sound using console audio api...
    }

    void stopAllSounds()
    {
        // Stop all sounds using console audio api...
    }
    // check if track id is found in the currently available tracks library
    bool TrackAvilable(string trackID)
    {
        if(library.ContainsKey(trackID))
            return true;
        return false;
    }

    // check if a track id is found in all possible game tracks
    bool TrackExists(string trackID)
    {
        if (allTracks.ContainsKey(trackID))
            return true;
        return false;
    }

    // when the user has categorized/ labeled a track to a certain emotion, use this method to add this new information to the track's data
    public void SetTrackEmotion(string trackID,string emotion)
    {
        if(!TrackAvilable(trackID))
        {
            Debug.LogError("Track is not found in the current available tracks library");
            return;
        }
        library[trackID].SetUserResponse(emotion);
    }
    public void AddTrackToLibrary(string trackID)
    {
        if (TrackAvilable(trackID))
        {
            Debug.LogError("Track is already in library");
            return;
        }
        if(!TrackExists(trackID))
        {
            Debug.LogError("Track does not exist");
            return;
        }
        library.Add(trackID, allTracks[trackID]);
    }
    public void RemoveTrackFromLibrary(string trackID)
    {
        if (!TrackAvilable(trackID))
        {
            Debug.LogError("Track is not found in the current available tracks library");
            return;
        }
        library.Remove(trackID);
    }
}
