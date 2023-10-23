using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMask : MonoBehaviour
{
    private AreaInformation area;

    public WorldMask(AreaInformation area)
    {
        this.area = area;
    }

    public void SetArea(AreaInformation area)
    {
        this.area = area;
    }

    public AreaInformation GetArea()
    {
        return area;
    }
}
