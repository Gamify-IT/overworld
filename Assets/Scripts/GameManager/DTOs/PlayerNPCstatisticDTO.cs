using System;

/// <summary>
///     This class is used to retrieve data from Get Requests.
/// </summary>
[Serializable]
public class PlayerNPCStatisticDTO
{
    #region Attributes

    public string id;
    public bool completed;
    public NPCDTO npc;

    #endregion
}