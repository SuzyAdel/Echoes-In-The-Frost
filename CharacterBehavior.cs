using UnityEngine;

public class CharacterBehavior : MonoBehaviour
{
    [Header("Drone Settings")]
    public float droneDetectionRange = 5f;
    public Transform drone;  // Assign drone in Inspector

    [Header("First Aid Kit Settings")]
    public float kitDetectionRange = 5f;
    public float kitPickupRange = 0.5f;
    public float turnCompleteAngle = 30f;
    private Transform targetKit;
    private float lastKitCheckTime;
    private float kitCheckInterval = 0.5f;

    // Animator references
    private Animator animator;
    private bool saved = false;
    private bool isTurning = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (saved) return;

        HandleDroneProximity(); // Fixed method name (was HandleDroneProximity)

        if (Time.time - lastKitCheckTime > kitCheckInterval && targetKit == null)
        {
            FindNearestKit();
            lastKitCheckTime = Time.time;
        }

        if (targetKit != null)
        {
            HandleKitPickup();
        }
    }

    // Fixed method name (was HandleDroneProximity)
    private void HandleDroneProximity()
    {
        if (drone == null) return;

        float distanceToDrone = Vector3.Distance(transform.position, drone.position);
        bool isDroneNear = distanceToDrone <= droneDetectionRange;

        // Only wave if not currently handling a kit
        if (targetKit == null)
        {
            animator.SetBool("waving", isDroneNear);
        }
        else
        {
            animator.SetBool("waving", false);
        }
    }

    private void FindNearestKit()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, kitDetectionRange);
        float closestDistance = Mathf.Infinity;
        Transform closestKit = null;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("FirstAidKit") && hitCollider.enabled)
            {
                // Additional check to make sure kit isn't falling through surfaces
                if (Physics.Raycast(hitCollider.transform.position, Vector3.down, 0.1f))
                {
                    float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestKit = hitCollider.transform;
                    }
                }
            }
        }

        if (closestKit != null)
        {
            targetKit = closestKit;
            isTurning = true;
            animator.SetBool("turn_right", true);
            animator.SetBool("waving", false);
        }
    }

    private void HandleKitPickup()
    {
        if (targetKit == null)
        {
            animator.SetBool("turn_right", false);
            animator.SetBool("walk", false);
            return;
        }

        Vector3 directionToKit = targetKit.position - transform.position;
        directionToKit.y = 0;
        float angleToKit = Vector3.Angle(transform.forward, directionToKit);
        float distanceToKit = directionToKit.magnitude;

        // 1. Turn toward kit
        if (angleToKit > turnCompleteAngle && isTurning)
        {
            // Calculate turn direction (still using right turn animation)
            float dot = Vector3.Dot(transform.right, directionToKit.normalized);
            if (dot > 0) // If kit is to our right
            {
                animator.SetBool("turn_right", true);
            }
            else // If kit is to our left, we'll still use right turn animation
            {
                animator.SetBool("turn_right", true);
                // Optional: Add slight rotation to help face the kit
                transform.Rotate(0, Time.deltaTime * 100f, 0);
            }
            animator.SetBool("walk", false);
        }
        // 2. Walk toward kit
        else if (distanceToKit > kitPickupRange)
        {
            isTurning = false;
            animator.SetBool("turn_right", false);
            animator.SetBool("walk", true);

            // Smooth rotation toward kit
            Quaternion targetRotation = Quaternion.LookRotation(directionToKit);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
        // 3. Pick up the kit
        else
        {
            animator.SetBool("walk", false);
            animator.SetBool("pickup", true);

            // Make sure kit exists before destroying
            if (targetKit != null)
            {
                Destroy(targetKit.gameObject);
            }
            targetKit = null;
        }
    }

    public void OnPickupComplete()
    {
        animator.SetBool("pickup", false);
        animator.SetBool("sitting", true);
        saved = true;
        targetKit = null;
        isTurning = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, droneDetectionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, kitDetectionRange);
    }
}
