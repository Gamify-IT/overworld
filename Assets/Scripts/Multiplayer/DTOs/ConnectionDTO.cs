using System;
using UnityEngine;

/// <summary>
///     This class is used to retrieve data from Get Requests.
/// </summary>
[Serializable]
public class ConnectionDTO
{
    /// <summary>
    ///     This function converts a json string to a <c>ConnectionDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>ConnectionDTO</c> object containing the data</returns>
    public static ConnectionDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<ConnectionDTO>(jsonString);
    }

    #region Attributes

    public string playerId;
    public string clientId;
    public string courseId;


    #endregion

    #region Constructors

    public ConnectionDTO(string playerId, string clientId, string courseId)
    {
        this.playerId = playerId;
        this.clientId = clientId;
        this.courseId = courseId;
    }

    public ConnectionDTO()
    {
    }

    #endregion
}