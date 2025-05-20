using UnityEngine;

public class KitPickup : MonoBehaviour
{
    public int strandedIndex; // 0, 1, or 2 based on GameManager

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Stranded"))
        {
            GameManager.Instance.KitDeliveredTo(strandedIndex);
            Destroy(gameObject); // Simulate pickup
        }
    }
}
