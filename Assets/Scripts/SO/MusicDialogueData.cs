using System.Collections;
using UnityEditor;
using UnityEngine;

// this is a container for a music dialogue data that will be open, so when can know some information about it 
[CreateAssetMenu(fileName = "Music Dialogue Data", menuName = "Scriptable Objects/ Music Dialogue Data")]
public class MusicDialogueData : ScriptableObject
{
    public Emotions emotionToEnvoke;
    [SerializeField]
    string onCompletionNode = null;
    [SerializeField]
    string uniqueId; // for this gameobject, persistent throughout the project 
    public string InteractionID { get { return uniqueId; } }
    public string interactionName;
#if UNITY_EDITOR

    private void OnEnable()
    {
        uniqueId = GlobalObjectId.GetGlobalObjectIdSlow(this).ToString();
    }
#endif
    public Emotions GetEmotion()
    {
        return emotionToEnvoke;
    }
}
