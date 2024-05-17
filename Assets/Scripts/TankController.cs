using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody))]
public class TankController : MonoBehaviour
{
    public float m_Speed = 12f;
    public float m_TurnSpeed = 180f;
    public float m_WheelRotateSpeed = 90f;
    public TrailRenderer[] tank_trackMarks;     //First attempt at track marks
    public GameObject skidMarksPrefab;
    public float despawnTime = 3f;
    public float xOffset = 0.1f;
    public float zOffset = 0.1f;
    
    private Rigidbody m_Rigidbody;              // Reference used to move the tank.
    private string m_MovementAxisName;          // The name of the input axis for moving forward and back.
    private string m_TurnAxisName;              // The name of the input axis for turning.
    private float m_MovementInputValue;         // The current value of the movement input.
    private float m_TurnInputValue;             // The current value of the turn input.
    private Vector3 m_MouseInputValue;          // The current value of the mouse input
    private int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
    private List<GameObject> m_wheels = new List<GameObject>();
    private GameObject m_turret;
    private float camRayLength = 100f;          // The length of the ray from the camera into the scene.
    
    private List<GameObject> m_orugas = new List<GameObject>();
    private List<GameObject> m_trackMarkpawnPoints = new List<GameObject>();
    private List<float> trackMarksTimeStamp = new List<float>();
    private Queue<GameObject> trackMarksQueue = new Queue<GameObject>();

    private float pollingTime = 2;
    private float timeLastPoll;



    private void Awake()
    {
        // Create a layer mask for the floor layer.
        floorMask = LayerMask.GetMask("Ground");

        m_Rigidbody = GetComponent<Rigidbody>();

        Transform[] children = GetComponentsInChildren<Transform>();
        for (var i = 0; i < children.Length; i++)
        {
            // Get all wheels in the children
            if (children[i].name.Contains("wheel"))
            {
                m_wheels.Add(children[i].gameObject);
            }

            // Get turret
            if (children[i].name.Contains("Turret"))
            {
                m_turret = children[i].gameObject;
            }

            // Get ORUGAS
            if (children[i].name.Contains("ORUGA"))
            {
                m_orugas.Add(children[i].gameObject);
            }

            // Get spawnPoints
            if (children[i].name.Contains("TrackSpawnPoint"))
            {
                m_trackMarkpawnPoints.Add(children[i].gameObject);
            }

        }
        timeLastPoll = Time.time;
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_MovementAxisName = "Vertical";
        m_TurnAxisName = "Horizontal";

    }

    private void Update()
    {
        // Store the value of both input axes.
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);
        m_MouseInputValue = Input.mousePosition;
        IsMoving();
    }

    private void FixedUpdate()
    {
        // Adjust the rigidbodies position and orientation in FixedUpdate.
        Move();
        Turn();
        RotateWheels();
        RotateTurret();
        IsMoving();
    }


    private void OnEnable()
    {
        // When the tank is turned on, make sure it's not kinematic.
        m_Rigidbody.isKinematic = false;

        // Also reset the input values.
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }

    private void OnDisable()
    {
        // When the tank is turned off, set it to kinematic so it stops moving.
        m_Rigidbody.isKinematic = true;
    }

    private void Move()
    {
        // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

        // Apply this movement to the rigidbody's position.
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }


    private void Turn()
    {
        // Determine the number of degrees to be turned based on the input, speed and time between frames.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        // Make this into a rotation in the y axis.
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        // Apply this rotation to the rigidbody's rotation.
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }

    private void RotateWheels()
    {
        // Rotate tank wheels. When tank moves forward, the wheels should rotate forward; When tank moves backwards, the wheels should rotate backwards.
        // Your code here
        //Debug.Log($"m_MovementInputValue: {m_MovementInputValue}");
        if (m_MovementInputValue != 0)
        {
            float turn = -m_MovementInputValue * m_WheelRotateSpeed * Time.deltaTime;
            Quaternion turnRotation = Quaternion.Euler(turn, 0f, 0f);
            foreach (var wheel in m_wheels)
            {
                wheel.transform.localRotation *= turnRotation;
                //Debug.Log($"turnRotation: {turnRotation}");
            }
        }
    }

    private void RotateTurret()
    {
        // Create a ray from the mouse cursor on screen in the direction of the camera.
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Create a RaycastHit variable to store information about what was hit by the ray.
        RaycastHit floorHit;

        // Perform the raycast and if it hits something on the floor layer...
        if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
        {
            // Your code here.
            Vector3 pointToLookAt = floorHit.point;
            pointToLookAt.y = m_turret.transform.position.y;
            m_turret.transform.rotation = Quaternion.LookRotation(pointToLookAt - m_turret.transform.position);
        }
    }

    private void IsMoving()
    {
        if (m_MovementInputValue != 0)
        {
            SpawnSkidMarks();
        }
        else
        {
        }
    }

    private void SpawnSkidMarks()
    {
        foreach (var spawnPoint in m_trackMarkpawnPoints)
        {
            Vector3 position = spawnPoint.transform.position;
            position.y = -0.1f;
            Vector3 tankRotation = transform.rotation.eulerAngles;
            Quaternion rotation = Quaternion.Euler(90, 0, 0);
            rotation.SetLookRotation(-Vector3.up, spawnPoint.transform.up);
            GameObject trackMark = Instantiate(skidMarksPrefab, position, rotation);
            Destroy(trackMark, despawnTime);
        } 
    }
}
