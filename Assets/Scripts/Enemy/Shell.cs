using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    [SerializeField]private GameObject shell;
    public Vector3 offsetPosition;

    void Update()
    {
        shell.transform.position = transform.position + offsetPosition;   
    }
}
