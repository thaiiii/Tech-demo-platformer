using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCannonUI : MonoBehaviour
{
    [HideInInspector] public Canvas cannonUICanvas;
    [HideInInspector] public Slider powerSlider;
    private PlayerCannon cannon;

    void Start()
    {
        cannon = gameObject.GetComponent<PlayerCannon>();
        cannonUICanvas.enabled = false;
    }

    // Update is called once per frame


    public void ShowUI()
    {
        if (cannon.isPlayerInside == true)
        {
            cannonUICanvas.enabled = true;

            //Cap nhat gia tri
            powerSlider.value = cannon.force;
            Debug.Log(cannon.force);
        }
    }
    public void HideUI()
    {
        cannonUICanvas.enabled = false;
    }
}
