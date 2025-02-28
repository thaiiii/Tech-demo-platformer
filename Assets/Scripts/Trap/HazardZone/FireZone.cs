using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HazardZone;

public class FireZone : HazardZone
{
    private void Start()
    {
        hazardType = HazardType.Fire;
    }
}
