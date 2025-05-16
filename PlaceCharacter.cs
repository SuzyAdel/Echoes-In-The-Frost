using UnityEngine;

public class PlaceCharacter : MonoBehaviour
{
    [Header("Animations")]
    public Animator characterAnimator;
    public string sittingAnimation = "Sitting";

    [Header("Placement Settings")]
    public float maxSlopeAngle = 10f;
    public Terrain terrain; // Assign in inspector or we'll find it automatically
    public LayerMask groundLayer;

    private void Start()
    {
        // Try to find terrain if not assigned
        if (terrain == null)
        {
            terrain = Terrain.activeTerrain;
        }

        // Play the sitting animation initially
        if (characterAnimator != null)
        {
            characterAnimator.Play(sittingAnimation);
        }

        // Find a suitable random position on the terrain
        PlaceCharacterOnValidGround();
    }

    private void PlaceCharacterOnValidGround()
    {
        if (terrain == null)
        {
            Debug.LogError("No terrain found for character placement!");
            return;
        }

        bool positionFound = false;
        int attempts = 0;
        const int maxAttempts = 100;

        // Get terrain bounds
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainSize = terrainData.size;
        Vector3 terrainPos = terrain.transform.position;

        while (!positionFound && attempts < maxAttempts)
        {
            attempts++;

            // Generate random position within terrain bounds
            float randomX = Random.Range(terrainPos.x, terrainPos.x + terrainSize.x);
            float randomZ = Random.Range(terrainPos.z, terrainPos.z + terrainSize.z);
            Vector3 randomPosition = new Vector3(randomX, terrainPos.y + terrainSize.y, randomZ);

            // Raycast down to find the ground
            if (Physics.Raycast(randomPosition, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                // Check the slope angle
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

                if (slopeAngle <= maxSlopeAngle)
                {
                    // Valid position found
                    transform.position = hit.point;

                    // Rotate character to face a random direction
                    transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

                    // Adjust position slightly to ensure feet are on ground
                    if (TryGetComponent(out CapsuleCollider collider))
                    {
                        transform.position += Vector3.up * collider.height * 0.5f;
                    }

                    positionFound = true;
                    Debug.Log($"Found valid position after {attempts} attempts");
                }
            }
        }

        if (!positionFound)
        {
            Debug.LogWarning($"Could not find valid position for {gameObject.name} after {maxAttempts} attempts. Keeping default position.");
        }
    }
}