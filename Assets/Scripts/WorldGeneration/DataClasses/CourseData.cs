using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class defines all needed data for a <c>Course</c>.
/// </summary>
public class CourseData
{
    #region Attributes
    private readonly int id;
    private readonly string courseName;
    private readonly string semester;
    private readonly string description;
    private readonly bool active;
    private readonly List<WorldData> worlds;
    #endregion

    #region Constructor 
    public CourseData(int id, string courseName, string semester, string description, bool active, List<WorldData> worlds)
    {
        this.id = id;
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
        int courseID = dto.id;
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

        CourseData data = new CourseData(courseID, courseName, semester, description, active, worlds);
        return data;
    }

    /// <summary>
    ///     This function converts a list of CourseDTOs to a list of CourseDatas
    /// </summary>
    /// <param name="dto">The CourseDTO to convert</param>
    /// <returns>The converted CourseData list</returns>
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

    /// <summary>
    ///     This function converts an array of CourseDTOs to a list of CourseDatas
    /// </summary>
    /// <param name="dto">The CourseDTO to convert</param>
    /// <returns>The converted CourseData array</returns>
    public static CourseData[] ConvertDtoToData(CourseDTO[] dto)
    {
        CourseData[] dataArray = new CourseData[dto.Length];

        for (int i = 0; i < dto.Length; i++)
        {
            CourseData data = ConvertDtoToData(dto[i]);
            dataArray[i] = data;
        }

        return dataArray;
    }

    #region Getter 
    /// <summary>
    ///     Gets the name of the course
    /// </summary>
    /// <returns>name of the course</returns>
    public string GetCourseName()
    {
        return courseName;
    }

    /// <summary>
    ///     Gets the course ID of the course
    /// </summary>
    /// <returns>ID of the course</returns>
    public int GetCourseID()
    {
        return id;
    }

    /// <summary>
    ///     Gets the semester of the course
    /// </summary>
    /// <returns>smester of the course</returns>
    public string GetSemester()
    {
        return semester;
    }

    /// <summary>
    ///     Gets the description of the course
    /// </summary>
    /// <returns>course description</returns>
    public string GetDescription()
    {
        return description;
    }

    /// <summary>
    ///     Gets the status of the course, i.e., if the course is active or not
    /// </summary>
    /// <returns>course status</returns>
    public bool GetActiveStatus()
    {
        return active;
    }

    /// <summary>
    ///     Gets all worlds of the course
    /// </summary>
    /// <returns>course worlds</returns>
    public List<WorldData> GetWorldData()
    {
        return worlds;
    }
    #endregion
}