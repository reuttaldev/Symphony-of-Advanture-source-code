using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[ExecuteInEditMode]
public class GetGlobalObjectId : MonoBehaviour
{
    [SerializeField]
    string uniqueId; // for this gameobject, persistent throughout the project 
    public string UniqueId { get { return uniqueId; } }

    private void Start()
    {
        Debug.Log(uniqueId);
    }
#if UNITY_EDITOR
    // Due to ExecuteAllways this is called once the component is created
    private void Awake()
    {
        // Don't do anything when running the game
        if (Application.isPlaying)
            return;
        if(string.IsNullOrEmpty(uniqueId))    
        {
            uniqueId = GlobalObjectId.GetGlobalObjectIdSlow(this).ToString();
            EditorUtility.SetDirty(this);
        }
    }
#endif
}
