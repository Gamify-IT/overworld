using UnityEngine;
using System;
using System.Runtime.InteropServices;

public static class BrowserVariable
{
    [DllImport("__Internal")]
    public  static extern string GetToken(string tokenName);

    /// <summary>
    ///     This function tries to read the given browser variable
    /// </summary>
    /// <param name="browserVariable">The browser variable to be read</param>
    /// <returns>An optional containing the read value, if the variable exists, an empty optional otherwise</returns>
    public static Optional<string> TryToReadVariable(string browserVariable)
    {
        Optional<string> result = new Optional<string>();
        try
        {
            string content = GetToken(browserVariable);
            result.SetValue(content);
        }
        catch (EntryPointNotFoundException e)
        {
            Debug.LogError("Browser variable not found: " + browserVariable);
        }
        return result;
    }
}
