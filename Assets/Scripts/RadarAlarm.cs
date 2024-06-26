﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RadarAlarm : MonoBehaviour
{
    public GameObject enemy;

    private LineRenderer lineRenderer;
    private GameObject spawnPoint;

    [Range(0.1f, 100f)]
    public float radius = 30.0f;

    [Range(3, 256)]
    public int numSegments = 128;

    private void Awake()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        spawnPoint = GameObject.Find("SpawnPoint");
    }

    void Start()
    {
        DoRenderer();
    }

    public void DoRenderer(bool detectedTank = false)
    {         
        Color c1 = new Color(0.5f, 0.5f, 0.5f, 1);
        lineRenderer.material = detectedTank ? Resources.Load<Material>("Materials/Alarm") 
                                                : Resources.Load<Material>("Materials/Save");
        lineRenderer.startColor = c1;
        lineRenderer.endColor = c1;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = numSegments + 1;                
        lineRenderer.useWorldSpace = false;

        float deltaTheta = (float)(2.0 * Mathf.PI) / numSegments;
        float theta = 0f;

        for (int i = 0; i < numSegments + 1; i++)
        {
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);
            Vector3 pos = new Vector3(x, 0, z);
            lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        // Your code here.
        
        // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
        //InvokeRepeating("Spawn", 3f, 3f);

        if(other.gameObject.tag == "Player")
        {
            InvokeRepeating("Spawn", 1f, 3f);
            DoRenderer(detectedTank: true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        // Your code here.

        //CancelInvoke("Spawn");
        if (other.gameObject.tag == "Player")
        {
            CancelInvoke("Spawn");
            DoRenderer(detectedTank: false);
        }
    }


    void Spawn()
    {
        if (enemy != null)
        {
            // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
            GameObject enemytank = Instantiate(enemy, spawnPoint.transform.position, spawnPoint.transform.rotation);


            //Scale the enemy tanks so that the secondary weapon can actually hit them.
            enemytank.transform.localScale *= 1.5f;
        }        
    }
}
