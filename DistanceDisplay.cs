using UnityEngine;
using TMPro;

public class DistanceDisplay : MonoBehaviour
{
    public Transform droneTransform; // Your drone
    public Transform[] strandedPeople; // Characters with names
    public TextMeshProUGUI distanceTextUI; // UI text element

    private const float maxRange = 800f;

    void Update()
    {
        if (droneTransform == null || distanceTextUI == null || strandedPeople.Length == 0)
            return;

        float closestDistance = Mathf.Infinity;
        string closestName = "Unknown";

        foreach (Transform person in strandedPeople)
        {
            float distance = Vector3.Distance(droneTransform.position, person.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestName = person.name; // Uses the GameObject's name
            }
        }

        if (closestDistance <= maxRange)
        {
            distanceTextUI.text = $"🧍 Closest: {closestName}\n📏 Distance: {closestDistance:F1} units";
        }
        else
        {
            distanceTextUI.text = $"🧍 Closest: {closestName}\n📏 Distance: OUT OF RANGE";
        }
    }
}
