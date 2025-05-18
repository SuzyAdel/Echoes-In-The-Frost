using UnityEngine;

public class SnowFollowDrone : MonoBehaviour
{
    public Transform droneTransform; // Assign your drone object in the inspector
    public float heightOffset = 3f; // Adjust this to control how high the snow floats above the drone

    private ParticleSystem snowParticles;

    void Start()
    {
        // Get the particle system component
        snowParticles = GetComponent<ParticleSystem>();

        if (snowParticles == null)
        {
            Debug.LogWarning("SnowFollowDrone: No ParticleSystem found on this GameObject.");
        }

        if (droneTransform == null)
        {
            Debug.LogError("SnowFollowDrone: Drone Transform is not assigned!");
        }

        // Ensure the particle system is playing
        if (!snowParticles.isPlaying)
        {
            snowParticles.Play();
        }
    }

    void LateUpdate()
    {
        if (droneTransform != null)
        {
            // Follow the drone with the specified height offset
            transform.position = new Vector3(
                droneTransform.position.x,
                droneTransform.position.y + heightOffset,
                droneTransform.position.z
            );
        }
    }
}
