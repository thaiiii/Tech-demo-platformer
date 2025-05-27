using System.Collections;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] float time;
    bool start = false;
    IEnumerator Explosion()
    {
        if (!start)
            yield return null;

        yield return new WaitForSeconds(time);
        Destroy(gameObject);

    }
    private void Update()
    {
        if (gameObject.activeSelf)
            start = true;
        StartCoroutine(Explosion());
    }
}
