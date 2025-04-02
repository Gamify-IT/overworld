using System;
using UnityEngine;

/// <summary>
///     This class is used to retrieve data from Get Requests.
/// </summary>
[Serializable]
public class ResponseDTO
{
    /// <summary>
    ///     This function converts a json string to a <c>ResponseDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>ResponseDTO</c> object containing the data</returns>
    public static ResponseDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<ResponseDTO>(jsonString);
    }

    #region Attributes

    public ushort clientId;

    #endregion

    #region Constructors

    public ResponseDTO(ushort clientId)
    {
        this.clientId = clientId;
    }

    public ResponseDTO()
    {
    }

    #endregion
}