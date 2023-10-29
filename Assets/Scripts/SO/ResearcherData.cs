using Google.Apis.Auth.OAuth2;
using UnityEngine;

public class ResearcherData : ScriptableObject
{
    public string projectType, projectId, clientEmail, clientId;
    public string privateKey, privateKeyId;

#if UNITY_EDITOR
    public void LoadData(JsonCredentialParameters parms)
    {
       /* this.projectType = type;
        this.projectId = projectId;
        this.clientEmail = clientEmail;
        this.clientId = clientId;   
        this.privateKey = key;
        this.privateKeyId = keyID;*/
    }
#endif
}
