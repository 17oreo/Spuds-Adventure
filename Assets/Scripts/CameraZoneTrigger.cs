using UnityEngine;

public class CameraZoneTrigger : MonoBehaviour
{
    [SerializeField] private int zoneIndex;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && CameraController.Instance != null)
        {
            CameraController.Instance.SetZone(zoneIndex);
        }
    } 
}
