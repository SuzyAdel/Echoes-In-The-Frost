using UnityEngine;

public class CharacterBehavior : MonoBehaviour
{
    [Header("Drone Settings")]
    public float droneDetectionRange = 5f;
    public Transform drone;  //to assign drone in Inspector

    [Header("First Aid Kit Settings")]
    public float kitDetectionRange = 5f; // Range to detect kits
    public float kitPickupRange = 0.5f; // Distance to actually pick up
    public float turnCompleteAngle = 30f; // angle at which turn stops
    private Transform targetKit; // current kit to pick up
    private float lastKitCheckTime;
    private float kitCheckInterval = 0.5f; // How often to check for kits

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

        // 2. Check for kits periodically (better performance than every frame)
        if (Time.time - lastKitCheckTime > kitCheckInterval)
        {
            FindNearestKit();
            lastKitCheckTime = Time.time;
        }

        // 3. Handle kit pickup if one is nearby
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

    // FIND NEAREST FIRST AID KIT
    private void FindNearestKit()
    {
        if (targetKit != null) return; // Already have a target

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, kitDetectionRange);
        float closestDistance = Mathf.Infinity;
        Transform closestKit = null;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("FirstAidKit"))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestKit = hitCollider.transform;
                }
            }
        }

        if (closestKit != null)
        {
            targetKit = closestKit;
            animator.SetBool("turn_right", true);
        }
    }

    // FIRST AID KIT LOGIC 
    private void HandleKitPickup()
    {
        // Check if kit was destroyed or picked up by someone else
        if (targetKit == null)
        {
            animator.SetBool("turn_right", false);
            animator.SetBool("walk", false);
            return;
        }

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
        else if (distanceToKit > kitPickupRange) // Use the defined pickup range
        {
            animator.SetBool("turn_right", false);
            animator.SetBool("walk", true);
            
            // Face the kit while walking (root motion will handle movement)
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(directionToKit),
                Time.deltaTime * 5f);
        }
        // 3. Pick up the kit
        else
        {
            animator.SetBool("walk", false);
            animator.SetBool("pickup", true);
            Destroy(targetKit.gameObject); // Remove the kit
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
        Gizmos.DrawWireSphere(transform.position, kitDetectionRange);
    }
}
