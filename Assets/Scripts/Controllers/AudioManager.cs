using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour, IRegistrableService
{
    private TrackData currentlyPlayingTrack;
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
}
