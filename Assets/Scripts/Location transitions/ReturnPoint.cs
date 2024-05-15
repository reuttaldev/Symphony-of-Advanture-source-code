using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnPoint : MonoBehaviour
{
    public string whenComingFrom;
    public Direction companionAtSideOfPlayer;
        
}

public enum Direction
{
    left,right,up,down,none
}
