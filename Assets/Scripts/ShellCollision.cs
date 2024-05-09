using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellCollision : MonoBehaviour
{
    public GameObject explosionParticlesPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        // Destroy the shell if it hits something (e.g. rock, ground, enemytank, etc.). 
        // If the shell hits enemy tank, the enemy tank will also be destroyed.

        // Your code here.
        if (collision.collider.tag != "Player")
        {
            //Debug.Log($"{collision.collider.name}");
            Destroy(gameObject);
            if (explosionParticlesPrefab != null)
            {
                GameObject explosion = (GameObject)Instantiate(explosionParticlesPrefab, transform.position, explosionParticlesPrefab.transform.rotation);
                Destroy(explosion, explosion.GetComponent<ParticleSystem>().main.startLifetimeMultiplier);
            }

            if(collision.collider.tag == "Enemy")
            {
                Destroy(collision.collider.gameObject);
            }
        }
    }

}
