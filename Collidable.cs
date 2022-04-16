using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script handles anything that can be collided with
public class Collidable : MonoBehaviour
{
    protected Collider[] detectedHits = new Collider[10];
    protected Vector3 originalHitBoxSize;
    [SerializeField]
    protected BoxCollider hitBox;
    protected virtual void Start()
    {
        originalHitBoxSize = hitBox.size;
    }
    protected virtual void OnCollisionEnter(Collision coll)
    {
        Debug.Log("No OnCollisionEnter Detected in " + gameObject.name);
    }
    protected virtual void OnTriggerEnter(Collider other)
    {

    }
}