using System.Collections;
using UnityEditor;
using UnityEngine;

// this is a container for a music dialogue data that will be open, so when can know some information about it 
[CreateAssetMenu(fileName = "Music Dialogue Data", menuName = "Scriptable Objects/ Music Dialogue Data")]
public class MusicDialogueData : MyScriptableObject
{
    public string emotionToInvoke;
    public string onCompletionNode = null;
    public string interactionName;

}
