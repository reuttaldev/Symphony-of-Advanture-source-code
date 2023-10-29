using UnityEngine;

public class ResearcherData : ScriptableObject
{
    public string projectType, projectId, clientEmail, clientId;
    public string privateKey, privateKeyId;

    public void LoadData(string type,string  projectId, string clientEmail, string clientId, string key, string keyID)
    {
        this.projectType = type;
        this.projectId = projectId;
        this.clientEmail = clientEmail;
        this.clientId = clientId;   
        this.privateKey = key;
        this.privateKeyId = keyID;
    }
}
