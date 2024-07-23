using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
///     This class is used to transfer an <c>CourseData</c> from and to the overworld backend
/// </summary>
[Serializable]
public class CourseDTO
{
    #region Attributes
    public string courseName;
    public string semester;
    public string description;
    public bool active;
    public List<WorldDTO> worlds;
    #endregion

    #region Constructors 
    public CourseDTO(string courseName, string semester, string description, bool active, List<WorldDTO> worlds)
    {
        this.courseName = courseName;
        this.semester = semester;
        this.description = description;
        this.active = active;
        this.worlds = worlds;
    }

    public CourseDTO()
    {
        worlds = new List<WorldDTO>();
    }
    #endregion

    /// <summary>
    ///     This function converts a json string to a <c>CourseDTO</c> object.
    /// </summary>
    /// <param name="jsonString">The json string to convert</param>
    /// <returns>A <c>CourseDTO</c> object containing the data</returns>
    public static CourseDTO CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<CourseDTO>(jsonString);
    }
}
