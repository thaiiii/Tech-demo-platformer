using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowGravityZone : HazardZone
{
    private void Start()
    {
        hazardType = HazardType.LowGravity;
    }
}
