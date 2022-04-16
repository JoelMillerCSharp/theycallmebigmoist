using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabLedge : MonoBehaviour
{
    private Moves moveScript;
    private void Awake()
    {
        moveScript = GetComponentInParent<Moves>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ledge")
            moveScript.ClimbLedge();
    }
}