using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class defines all needed data for a <c>Course</c>.
/// </summary>
public class CourseData
{
    #region Attributes
    private string courseName;
    private readonly string semester;
    private readonly string description;
    private readonly bool active;
    private readonly List<WorldData> worlds;
    #endregion

    #region Constructor 
    public CourseData( string courseName, string semester, string description, bool active, List<WorldData> worlds)
    {
        this.courseName = courseName;
        this.semester = semester;
        this.description = description;
        this.active = active;
        this.worlds = worlds;
    }
    #endregion

    /// <summary>
    ///     This function converts a CourseDTO to CourseData
    /// </summary>
    /// <param name="dto">The CourseDTO to convert</param>
    /// <returns>The converted CourseData</returns>
    public static CourseData ConvertDtoToData(CourseDTO dto)
    {
        string courseName = dto.courseName;
        string semester = dto.semester;
        string description = dto.description;
        bool active = dto.active;

        List<WorldData> worlds = new List<WorldData>();
        List<WorldDTO> worldDTOs = dto.worlds;
        foreach(WorldDTO worldDTO in worldDTOs)
        {
            WorldData worldData = WorldData.ConvertDtoToData(worldDTO);
            worlds.Add(worldData);
        }

        CourseData data = new CourseData(courseName, semester, description, active, worlds);
        return data;
    }

    /// <summary>
    ///     Thi function converts a list of CourseDTOs to a list of CourseDatas
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public static List<CourseData> ConvertDtoToData(List<CourseDTO> dto)
    {
        List<CourseData> dataList = new List<CourseData>();

        foreach (CourseDTO courseDTO in dto)
        {
            CourseData data = ConvertDtoToData(courseDTO);
            dataList.Add(data);
        }

        return dataList;
    }

    #region Getter 
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string GetCourseName()
    {
        return courseName;
    }
    #endregion
}