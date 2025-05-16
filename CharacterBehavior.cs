using UnityEngine;

public class CharacterBehavior : MonoBehaviour
{
    [Header("Drone Settings")]
    public float droneDetectionRange = 5f;
    public Transform drone;  //to assign drone in Inspector

    [Header("First Aid Kit Settings")]
    public float kitPickupRange = 3f;
    public float turnCompleteAngle = 30f; // angle at which turn stops
    private Transform targetKit; // current kit to pick up

    // Animator references
    private Animator animator;
    private bool saved = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (saved) return; // Skip if already saved

        // 1. Check for drone proximity
        HandleDroneProximity();

        // 2. Handle kit pickup if one is nearby
        if (targetKit != null)
        {
            HandleKitPickup();
        }
    }

    // DRONE PROXIMITY 
    private void HandleDroneProximity()
    {
        if (drone == null) return;

        float distanceToDrone = Vector3.Distance(transform.position, drone.position);
        bool isDroneNear = distanceToDrone <= droneDetectionRange;

        animator.SetBool("waving", isDroneNear);
    }

    // FIRST AID KIT LOGIC 
    private void HandleKitPickup()
    {
        Vector3 directionToKit = targetKit.position - transform.position;
        directionToKit.y = 0; // Ignore vertical difference
        float angleToKit = Vector3.Angle(transform.forward, directionToKit);
        float distanceToKit = directionToKit.magnitude;

        // 1. Turn toward kit (always right)
        if (angleToKit > turnCompleteAngle)
        {
            animator.SetBool("turn_right", true);
            animator.SetBool("walk", false);
        }
        // 2. Walk toward kit
        else if (distanceToKit > 0.5f) // Small buffer to prevent jitter
        {
            animator.SetBool("turn_right", false);
            animator.SetBool("walk", true);
        }
        // 3. Pick up the kit
        else
        {
            animator.SetBool("walk", false);
            animator.SetBool("pickup", true);
        }
    }

    // TRIGGER DETECTION (For Kits)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FirstAidKit") && !saved)
        {
            targetKit = other.transform;
            animator.SetBool("turn_right", true);
        }
    }

    // ANIMATION EVENT (Call at end of Pickup animation)
    public void OnPickupComplete()
    {
        animator.SetBool("pickup", false);
        animator.SetBool("sitting", true);
        saved = true;
        targetKit = null; // Reset target
    }

    // DEBUG HELPERS
    private void OnDrawGizmos()
    {
        // Drone detection range
        Gizmos.color = Color.blue;// turns blue when the drone is near
        Gizmos.DrawWireSphere(transform.position, droneDetectionRange);

        // Kit detection range (if using a trigger collider)
        Gizmos.color = Color.green; // turns green when the kit is near
        Gizmos.DrawWireSphere(transform.position, kitPickupRange);
    }
}