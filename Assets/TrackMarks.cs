using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TrackMarks : MonoBehaviour
{
    public GameObject tireMarks;
    public int tireMarksLifeTime = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBeforeTransformParentChanged()
    {

        Debug.Log("OnBeforeTransformParentChanged");
    }
}
