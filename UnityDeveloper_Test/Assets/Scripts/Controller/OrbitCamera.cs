using UnityEngine;


public class OrbitCamera : MonoBehaviour
{
    public Transform character; // Assign your character's Transform here
    public Transform cameraBoom; // Assign the GameObject holding the actual camera
    public float rotationSpeed = 5f;
    public float distance = 5f;
    public float height = 2f;

    private float currentX = 0.0f;
    private float currentY = 0.0f;

    private void Awake()
    {

    }

    private void Update()
    {

    }

    void LateUpdate()
    {
        if (!character || !cameraBoom) return;

        if (!Input.GetMouseButton(0)) return;

        // Get mouse input for rotation
        currentX += Input.GetAxis("Mouse X") * rotationSpeed;
        currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;

        // Optional: Clamp vertical angle to prevent camera flipping upside down
        currentY = Mathf.Clamp(currentY, -60f, 60f); // Adjust min/max angles as needed

        // Calculate rotation based on character's up vector
        // This creates a rotation quaternion that respects the character's dynamic 'up'
        Quaternion characterRotation = Quaternion.LookRotation(character.forward, character.up);
        Quaternion rotation = characterRotation * Quaternion.Euler(currentY, currentX, 0);

        // Set the position and orientation
        Vector3 direction = rotation * Vector3.back; // Vector3.back is used to position the camera behind the character based on current rotation
        Vector3 desiredPosition = character.position + direction * distance + character.up * height;

        transform.position = character.position; // The 'OrbitCamera' object stays with the character
        cameraBoom.position = desiredPosition;
        cameraBoom.LookAt(character.position, character.up); // Ensure camera looks at character and its 'up' is aligned
    }
}
