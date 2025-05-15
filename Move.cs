using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class Move : MonoBehaviour
{

    public float liftForce = 15f; //Public, to change later if required.
    float moveForce = 15f;
    float rotationTorque = 0.05f;
    float pitchTorque = 1f;
    float stabilizationTorque = 0.2f;
    //new functionalities 
    public GameObject firstAidKitPrefab; // to assign in inspector
    public float dropForce = 5f; // intial force applied when dropping
    private Rigidbody rb;

    public Transform[] propellers; // Assign all propeller transforms in inspector
    [Header("Propeller Settings")]
    public float maxPropellerSpeed = 3000f; // Start with this high value
    public float speedCurveExponent = 3f; // Adjust in Inspector
    public float speedMultiplier = 2f; // Adjust in Inspector
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Enter to drop first aid kit
    void HandleFirstAidDrop()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (firstAidKitPrefab != null)
            {
                GameObject kit = Instantiate(firstAidKitPrefab,
                                            transform.position - transform.up * 0.5f,
                                            Quaternion.identity); // to drop the kit below the drone
                Rigidbody kitRb = kit.GetComponent<Rigidbody>();
                if (kitRb != null)// check for Rigidbody
                {

                    kitRb.linearVelocity = rb.linearVelocity; // match drone's velocity
                    // linear velovity because it is a vector3 so i need x,y,z
                    kitRb.AddForce(-transform.up * dropForce, ForceMode.Impulse); // apply force downwards
                }
            }
        }
    }
    // to rotate propellers
    void UpdatePropellerSpeed()
    {
        if (propellers == null || propellers.Length == 0) return;

        // More aggressive speed curve with higher exponent and multiplier
        float normalizedLift = Mathf.Clamp01((liftForce - 5f) / 20f);
        float speed = maxPropellerSpeed * Mathf.Pow(normalizedLift, 3f) * 2f; // Cubed and doubled for dramatic effect

        // Stronger minimum speed threshold
        if (liftForce <= 5.1f)
        {
            speed = 0f;
            // Force reset all propellers to original rotation
            foreach (Transform propeller in propellers)
            {
                propeller.localRotation = Quaternion.identity;
            }
            return;
        }

        // Apply rotation with increased sensitivity
        foreach (Transform propeller in propellers)
        {
            propeller.Rotate(Vector3.forward, speed * Time.deltaTime);
        }
    }


    public void HandleThrust()
    {
        if (Input.GetKey(KeyCode.N))
        {
            if (liftForce - Time.fixedDeltaTime > 5)
                liftForce -= Time.fixedDeltaTime * 2.0f;
            else
                liftForce = 5.0f;

        }
        if (Input.GetKey(KeyCode.M))
        {
            if (liftForce + Time.fixedDeltaTime < 25)
                liftForce += Time.fixedDeltaTime * 2.0f;
            else
                liftForce = 25.0f;
        }

    }

    void FixedUpdate()
    {
        HandleThrust();
        HandleLift();
        HandleMovement();
        HandleRotation();
        StabilizeDrone();
        HandleFirstAidDrop();
        UpdatePropellerSpeed();
    }

    void HandleLift()
    {
        if (Input.GetKey(KeyCode.Space))
            rb.AddForce(transform.up * liftForce, ForceMode.Force);
    }

    void HandleMovement()
    {
        if (!Input.GetKey(KeyCode.Space))
            return;

        Vector3 moveDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) 
            moveDirection += transform.forward;
            Debug.Log("Pressed W");
        if (Input.GetKey(KeyCode.S)) 
            moveDirection -= transform.forward;
        if (Input.GetKey(KeyCode.A)) 
            moveDirection -= transform.right;
        if (Input.GetKey(KeyCode.D)) 
            moveDirection += transform.right;
        rb.AddForce(moveDirection.normalized * moveForce, ForceMode.Force);
    }

    void HandleRotation()
    {
        float yaw = 0f;
        float pitch = 0f;
        if (Input.GetKey(KeyCode.Q)) 
            yaw = -1f;
        if (Input.GetKey(KeyCode.E)) 
            yaw = 1f;

        rb.AddTorque(transform.up * yaw * rotationTorque, ForceMode.Force);

        if (Input.GetKey(KeyCode.Z))
            pitch = -1f;
        if (Input.GetKey(KeyCode.C))
            pitch = 1f;

        rb.AddTorque(transform.right * pitch * pitchTorque, ForceMode.Force); 
    }

    void StabilizeDrone()
    {
        Vector3 localTilt = transform.localRotation.eulerAngles;
        if (localTilt.x > 180) 
            localTilt.x -= 360;
        if (localTilt.z > 180) 
            localTilt.z -= 360;

        Vector3 stabilizationTorqueVec = new Vector3(-localTilt.x, 0f, -localTilt.z) * stabilizationTorque;
        rb.AddTorque(transform.TransformVector(stabilizationTorqueVec), ForceMode.Force);
    }
}
