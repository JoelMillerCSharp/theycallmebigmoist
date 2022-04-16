using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    //Baseline properties for any damage-based projectile.
    public float projectileSpeed;
    public float projectileLifetime;
    public int baseDamage;
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
    private void Update()
    {
        //Pushes the projectile forward based on speed and lifetime.
        transform.position += transform.right * Time.deltaTime * projectileSpeed;
        if (projectileLifetime < 0)
            Destroy(gameObject);
        projectileLifetime -= Time.deltaTime;
    }
}