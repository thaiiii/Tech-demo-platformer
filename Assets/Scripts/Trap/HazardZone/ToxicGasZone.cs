using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicGasZone : HazardZone
{
    private void Start()
    {
        hazardType = HazardType.ToxicGas;
    }
}
