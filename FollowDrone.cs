using UnityEngine;

public class FollowDrone : MonoBehaviour
{
    public Transform drone;      // Assign your drone GameObject here
    public Vector3 offset;       

    void LateUpdate()
    {
        if (drone != null)
        {
            transform.position = drone.position + offset;
        }
    }
}
