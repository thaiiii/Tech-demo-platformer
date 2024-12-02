using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    [Header("Objects to Activate/Deactivate")]
    public GameObject objectToActivate; // Vật thể sẽ xuất hiện
    public bool isObjectActivated = false;

    [Header("Object to Activate Loop")]
    public List<ObjectActivateLoop> objectActivateLoops;
    List<Coroutine> objectActivateLoopCoroutines = new List<Coroutine>();

    [System.Serializable]
    public class ObjectActivateLoop
    {
        public GameObject objectToActivateLoop;
        public bool isObjectActivatedLoop = false;
        public bool ObjectActivatedOrigin = false;
        public float delayLoop = 2f;
    }



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

    #region Activate Loop
    public void ActivateObjectLoop()
    {
        StopAllCoroutines();
        foreach (var o in objectActivateLoops) {
            objectActivateLoopCoroutines.Add(StartCoroutine(StartObjectLoop(o)));
        }
    }

    public void DeactivateObjectLoop()
    {
        foreach (var o in objectActivateLoops)
        {
            o.objectToActivateLoop.SetActive(o.ObjectActivatedOrigin);
            o.isObjectActivatedLoop = o.ObjectActivatedOrigin;
        }
        StopAllCoroutines();
    }

    
    private IEnumerator StartObjectLoop(ObjectActivateLoop o)
    {
        yield return new WaitForSeconds(o.delayLoop);

        o.isObjectActivatedLoop = !o.isObjectActivatedLoop;
        o.objectToActivateLoop.SetActive(o.isObjectActivatedLoop);
        
        StartCoroutine(StartObjectLoop(o));
    }
    #endregion
}
