using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ConsumableItem : ItemBase
{
    public float speedRate;
    public float speedEffectDuration;
    public float sizeUpValue; 

    public override void OnPickUp()
    {
        //Thay đổi thông số 
        switch (itemName)
        {
            case "SizeBoost":
                FindFirstObjectByType<PlayerEffects>().sizeUpValue = sizeUpValue;
                break;
            case "SpeedBoost":
                FindFirstObjectByType<PlayerEffects>().speedRate = speedRate;
                FindFirstObjectByType<PlayerEffects>().speedEffectDuration = speedEffectDuration;
                break;
        }
        
        FindFirstObjectByType<PlayerEffects>().ApplyEffect(itemName);
        gameObject.SetActive(false);
    }


}
