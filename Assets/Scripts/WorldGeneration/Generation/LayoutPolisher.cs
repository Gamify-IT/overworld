using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutPolisher
{
    private WorldStyle areaStyle;

    public LayoutPolisher(WorldStyle areaStyle)
    {
        this.areaStyle = areaStyle;
    }

    /// <summary>
    ///     This function polished the given floor / wall layout based on the selected style
    /// </summary>
    /// <param name="baseLayout">The layout to polish</param>
    /// <returns>The polished layout</returns>
    public bool[,] Polish(bool[,] baseLayout)
    {
        //polishing

        return baseLayout;
    }
}
