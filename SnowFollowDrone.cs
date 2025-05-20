using UnityEngine;

[RequireComponent(typeof(ParticleSystem), typeof(AudioSource))]
public class SnowFollowDrone : MonoBehaviour
{
    [Header("Drone Settings")]
    public Transform droneTransform;             // Assign the drone here in Inspector
    public float heightOffset = 3f;              // Snow hover height above drone

    private ParticleSystem snowParticles;
    private AudioSource blizzardAudio;

    void Start()
    {
        // Get components
        snowParticles = GetComponent<ParticleSystem>();
        blizzardAudio = GetComponent<AudioSource>();

        // Safety checks
        if (droneTransform == null)
        {
            Debug.LogError("❌ SnowFollowDrone: Drone Transform is not assigned!");
            enabled = false;
            return;
        }

        if (!snowParticles.isPlaying)
        {
            snowParticles.Play();
        }

        if (blizzardAudio != null && !blizzardAudio.isPlaying)
        {
            blizzardAudio.loop = true;
            blizzardAudio.Play();
        }
    }

    void LateUpdate()
    {
        // Keep snow and sound following the drone
        if (droneTransform != null)
        {
            Vector3 followPosition = droneTransform.position + Vector3.up * heightOffset;
            transform.position = followPosition;
        }
    }
}
