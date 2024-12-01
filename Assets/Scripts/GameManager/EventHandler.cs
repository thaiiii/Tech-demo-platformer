using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    [Header("Objects to Activate/Deactivate")]
    public GameObject objectToActivate; // Vật thể sẽ xuất hiện
    public bool isObjectActivated = false;

    #region Activate/Deactivate object
    public void ActivateObject()
    {
        if (objectToActivate != null && !isObjectActivated)
        {
            isObjectActivated = true;
            objectToActivate.SetActive(isObjectActivated);
        }
    }
    public void DeactivateObject()
    {
        if (objectToActivate != null && isObjectActivated)
        {
            isObjectActivated = false;
            objectToActivate.SetActive(isObjectActivated);
        }
    }
    #endregion
}
