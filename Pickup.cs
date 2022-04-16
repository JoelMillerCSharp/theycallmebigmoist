using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : Collidable
{
    /* 0 = Heal Player
     * 1 = Add Gun Ammo
     * 2 = Add Shiruken Ammo */
    public int pickupType;
    public int amount;
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            Destroy(gameObject);
    }
}