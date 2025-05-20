using System.Timers;
using UnityEditor.Rendering;
using UnityEngine;

public class CharacterBehavior : MonoBehaviour
{
    [Header("Drone Settings")]
    public float droneDetectionRange = 50f;// Distance at which the character can detect the drone
    public Transform drone;

    [Header("First Aid Kit Settings")]
    public float kitPickupRange = 10f;// Distance at which the character can pick up the kit
    public float turnCompleteAngle = 30f;// Angle at which the character will stop turning

    public float kitDestroyVelocity = 8f;

    public float stuckRotationTime = 5f;// Time before the character is considered stuck while rotating
    public float stuckMovementDistance = 2f;// Distance to check if the character is stuck while moving
    public GameObject rightHand;

    private Transform targetKit;
    private float rotationStartTime;
    private bool isRotating;// Whether the character is currently rotating

    private Vector3 lastPosition;
    private float positionCheckInterval = 1f;
    private float lastPositionCheckTime;

    [Header("Gizmo Settings")]
    public float gizmoSize = 1f;

    private Animator animator;
    private bool saved = false;// true when the character has picked up a kit, must return false afterwards 
    private bool hasKit = false;// true when the character has a kit in hand
    private bool kitHand = false;


    float yWithGravity = 0f;// y position with gravity  
    CharacterController controller;
    void Start()
    {
        animator = GetComponent<Animator>();
        lastPosition = transform.position;// used to check if the character is stuck
        controller = GetComponent<CharacterController>();

    }

    void Update()
    {
        // if (saved && !hasKit) return; this keeps the character stuck after picking up 

        HandleDroneProximity();// handle the drone proximity detection

        if (targetKit != null)// checks if character has a kit in hand
        {
            HandleKitPickup();// handle the kit pickup process
            CheckForStuckRotation();// check if the character is stuck while rotating
            if (kitHand && targetKit != null)
            {
                targetKit.SetParent(rightHand.transform);
                targetKit.GetComponent<Rigidbody>().isKinematic = true;
                targetKit.localPosition = Vector3.zero;
                targetKit.localRotation = Quaternion.identity;

                // destory after 3 sec when is held 
                Destroy(targetKit.gameObject, 3f);
            }
        }
        else
        {
            //reset to idle sitting and re checks for kit 
            animator.SetBool("turn_right", false);
            animator.SetBool("walk", false);
            animator.SetBool("pickup", false);

            animator.SetBool("sitting", true);
            animator.SetBool("waving", false);

            animator.SetBool("saved", false);
            animator.SetBool("saved_kit", false);


            FindNearestKit();// find the nearest kit if the character doesn't have one
            HandleDroneProximity(); // check if the drone is nearby agian 

        }

        if(controller.isGrounded)
        {
            yWithGravity = 0.0f;
        }
        yWithGravity += Physics.gravity.y * Time.deltaTime;
        Vector3 moveDirection = new Vector3(0, yWithGravity, 0);

        transform.position += moveDirection * Time.deltaTime;

        CheckPositionChange();// to protect against stuck movement
    }


    private void HandleDroneProximity()
    {
        if (drone == null) return;// check if the drone is assigned

        // Check if the drone is within detection range
        float distanceToDrone = Vector3.Distance(transform.position, drone.position);
        bool isDroneNear = distanceToDrone <= droneDetectionRange;

        animator.SetBool("waving", isDroneNear && !hasKit);
    }

    private void HandleKitPickup()
    {
        if (targetKit == null) return;// if there is no kit, return

        //Move towards the kit
        Vector3 directionToKit = targetKit.position - transform.position;//direction
        directionToKit.y = 0;// ignore y-axis
        float angleToKit = Vector3.Angle(transform.forward, directionToKit);//angle
        float distanceToKit = directionToKit.magnitude;

        // If kit is very close, just pick it up
        if (distanceToKit < 0.3f)
        {
            PickupKit();
            return;
        }

        // Turn toward kit
        if (angleToKit > turnCompleteAngle)
        {
            if (!isRotating)// if the character is not already rotating
            {
                // Calculate rotation step
                float rotationStep = Mathf.Min(angleToKit, turnCompleteAngle);
                //Quaternion targetRotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(directionToKit), rotationStep);
                //transform.rotation = targetRotation;


                // Check if the character is stuck while rotating
                if (angleToKit > stuckMovementDistance)
                {
                    CheckForStuckRotation();

                }
                // Start rotation timer

                rotationStartTime = Time.time;// reset the rotation timer
                isRotating = true;// set the character to rotating
            }


            animator.SetBool("turn_right", true);// after rotating, set the animation to turn right
            animator.SetBool("walk", false);
            animator.SetBool("pickup", false);
        }
        // Walk toward kit
        else if (distanceToKit > 0.5f)
        {
            isRotating = false;
            animator.SetBool("turn_right", false);
            animator.SetBool("walk", true);
        }
        // Pick up the kit , close enough 
        else
        {
            PickupKit();
        }
    }

    private void PickupKit()
    {
        animator.SetBool("walk", false);
        animator.SetBool("turn_right", false);
        animator.SetBool("pickup", true);

        // Immediately "pick up" the kit (in reality you might want to wait for animation)
        hasKit = true;
        if (targetKit != null)
        {
            //Destroy(targetKit.gameObject);// destroy the kit thats picked up
        }
        //targetKit = null;
    }

    private void CheckForStuckRotation()
    {
        if (isRotating && Time.time - rotationStartTime > stuckRotationTime)
        {
            // We've been rotating too long - try moving forward
            animator.SetBool("turn_right", false);
            animator.SetBool("walk", true);//walk not sit sa it might be slightly in front 

            // Reset rotation timer
            isRotating = false;

            // Try to find a new path to avoid beinf stuck 
            FindNearestKit();
        }
    }

    private void CheckPositionChange()
    {
        if (Time.time - lastPositionCheckTime > positionCheckInterval)
        {
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);
            // Check if the character has moved significantly
            if (distanceMoved < 0.1f)
            {
                // Character hasn't moved much fa might be stuck
                FindNearestKit();
            }

            lastPosition = transform.position;// update the last position
            lastPositionCheckTime = Time.time;// reset the last position check time
        }
    }

    private void FindNearestKit()
    {
        GameObject[] kits = GameObject.FindGameObjectsWithTag("FirstAidKit");//array 
        Transform closestKit = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject kit in kits)
        {
            // Skip destroyed or invalid kits , distroyed or null
            if (kit == null) continue;

            float distance = Vector3.Distance(transform.position, kit.transform.position);
            if (distance < closestDistance && distance <= kitPickupRange) // swap 
            {
                closestKit = kit.transform;
                closestDistance = distance;
            }
        }

        targetKit = closestKit;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FirstAidKit") && !hasKit)
        {
            // Check if this kit is valid (not destroyed)
            if (other.gameObject != null)
            {
                targetKit = other.transform;
                animator.SetBool("turn_right", true);
            }
        }
    }

    // Animation Event - Called at the end of pickup animation
    public void OnPickupComplete() // also done in update 
    {
        animator.SetBool("turn_right", false);
        animator.SetBool("walk", false);
        animator.SetBool("pickup", false);

        animator.SetBool("sitting", true);
        animator.SetBool("waving", false);

        animator.SetBool("saved", false);
        animator.SetBool("saved_kit", false);

        saved = true;
        hasKit = false; // Reset for next time
    }

    // First Aid Kit collision handler (to be called from the FirstAidKit script)
    public void OnKitDroppedNearby(GameObject kit)
    {
        if (!hasKit && Vector3.Distance(transform.position, kit.transform.position) <= kitPickupRange)
        {
            targetKit = kit.transform;
        }
    }

    public void PickUpKit(int val)
    {
        kitHand = val == 1;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, droneDetectionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, kitPickupRange);

        // Draw direction to target kit if one exists
        if (targetKit != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, targetKit.position);
        }
    }


    [RequireComponent(typeof(Rigidbody))]
    public class FirstAidKit : MonoBehaviour
    {
        public float validDropRange = 10f;
        private Rigidbody rb;
        private bool isDestroyed = false;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Rigidbody component missing!");
                enabled = false;
            }
        }

        void Update()
        {
            if (isDestroyed) return;

            if (rb.linearVelocity.magnitude > 8f)
            {
                DestroyKit();
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (isDestroyed) return;

            if (collision.gameObject.CompareTag("Terrain"))
            {
                if (rb.linearVelocity.magnitude > 8f)
                {
                    DestroyKit();
                }
                else
                {
                    NotifyCharacter();
                }
            }
        }

        private void DestroyKit()
        {
            isDestroyed = true;
            Destroy(gameObject);
        }

        private void NotifyCharacter()
        {
            CharacterBehavior character = FindObjectOfType<CharacterBehavior>();
            if (character != null)
            {
                character.OnKitDroppedNearby(gameObject);
            }
        }
    }
}
