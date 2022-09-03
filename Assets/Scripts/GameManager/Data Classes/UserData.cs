using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    #region Attributes
    public string userId;
    public string username;
    #endregion

    #region Constructor
    public UserData(string id)
    {
        userId = id;
        username = id;
    }
    #endregion
}
