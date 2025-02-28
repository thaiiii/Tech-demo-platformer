using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaZone : HazardZone
{
    private void Start()
    {
        hazardType = HazardType.Lava;
    }
}
