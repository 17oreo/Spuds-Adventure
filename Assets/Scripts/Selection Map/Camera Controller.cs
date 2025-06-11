using UnityEngine;

public class CameraController : MonoBehaviour
{

    public static CameraController Instance { get; private set; }

    //[SerializeField] private Transform player; // Your player object
    [SerializeField] private Vector3[] cameraPositions; // The positions to move to
    [SerializeField] private float transitionSpeed = 2f; // Optional smooth transition

    private int currentZoneIndex = 0;

    private void Awake()
    {
        // Singleton pattern setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("More than one CameraController found!");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Selection) return;
        MoveCameraToCurrentZone();
    }

    public void SetZone(int index)
    {
        if (index >= 0 && index < cameraPositions.Length)
        {
            currentZoneIndex = index;
        }
        else
        {
            Debug.LogWarning($"Invalid zone index: {index}");
        }
    }

    private void MoveCameraToCurrentZone()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            cameraPositions[currentZoneIndex],
            Time.deltaTime * transitionSpeed
        );
    }
}
