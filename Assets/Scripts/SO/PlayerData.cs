using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Player Data", menuName = "Scriptable Objects/ Player Data")]

public class PlayerData : ScriptableObject
{
    public string playerID, playerName;
}
