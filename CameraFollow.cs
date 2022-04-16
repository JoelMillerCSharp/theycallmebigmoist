using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player;
    private void Awake()
    {
        player = GameObject.Find("Herbert").GetComponent<Transform>();
    }
    private void FixedUpdate()
    {
        transform.position = player.position + new Vector3(0,2,-12);
    }
}