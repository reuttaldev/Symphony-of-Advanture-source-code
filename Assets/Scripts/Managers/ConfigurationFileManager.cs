using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurationFileManager : MonoBehaviour
{
    #region SYNTAX CHECKS

    // return an error message, if any
    public static string CheckSyntax(string text)
    {
        if(string.IsNullOrEmpty(text))
        {
            return "Text file is empty.";
        }
        return null;
    }
    #endregion
}
