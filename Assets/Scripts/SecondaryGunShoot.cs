using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SecondaryGunShoot : MonoBehaviour
{
    public int damagePerShot = 20;                  // The damage inflicted by each bullet.
    public float timeBetweenBullets = 0.15f;        // The time between each shot.
    public float range = 100f;                      // The distance the gun can fire.
    public GameObject laserPrefab;                  // The laser prefab to be instantiated when the gun is fired.

    float timer;                                    // A timer to determine when to fire.
    Ray shootRay = new Ray();                       // A ray from the gun end forwards.
    RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
    ParticleSystem gunParticles;                    // Reference to the particle system.
    float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.
    int hitMask;                                   // A layer mask so the raycast only hits rocks.


    void Awake()
    {
        // Set up the references.
        gunParticles = GetComponent<ParticleSystem>();
        hitMask = LayerMask.GetMask("Rock", "Enemy");
    }


    void Update()
    {
        // Add the time since Update was last called to the timer.
        timer += Time.deltaTime;

        // If the Fire1 button is being press and it's time to fire...
        if (Input.GetMouseButton(1) && timer >= timeBetweenBullets && Time.timeScale != 0)
        {
            // ... shoot the gun.
            Shoot();
        }
    }

    void Shoot()
    {
        // Reset the timer.
        timer = 0f;

        // Stop the particles from playing if they were, then start the particles.
        gunParticles.Stop();
        gunParticles.Play();

        // Enable the line renderer and set it's first position to be the end of the gun.

        // Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        // Perform the raycast against gameobjects on the shootable layer and if it hits something...
        // If it hits something, set the second position of the line renderer to the point the raycast hit, otherwise, 
        // set the second position of the line renderer to the maximal raycast range.

        // Your code here.
        var laserEndPos = transform.position + shootRay.direction * range;
        if (Physics.Raycast(shootRay, out shootHit, range, hitMask))
        {
            laserEndPos = shootHit.point;

            if (shootHit.collider.tag == "Enemy")
            {
                Destroy(shootHit.collider.gameObject);
                Quaternion rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        //Find the middle point of the laser 
        var laserMidPoint = (transform.position + laserEndPos) / 2;
        var laserObject = Instantiate(laserPrefab, laserMidPoint, laserPrefab.transform.rotation);
        laserObject.transform.localScale = new Vector3(laserObject.transform.localScale.x,
                                                        Vector3.Distance(transform.position, laserEndPos)/2,
                                                        laserObject.transform.localScale.z);
        laserObject.transform.LookAt(laserEndPos);
        laserObject.transform.Rotate(90, 0, 0, Space.Self);
        Destroy(laserObject, effectsDisplayTime);
    }
}
