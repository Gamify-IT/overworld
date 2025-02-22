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

    public byte playerId;


    #endregion

    #region Constructors

    public ResponseDTO(byte playerId)
    {
        this.playerId = playerId;
    }

    public ResponseDTO()
    {
    }

    #endregion
}