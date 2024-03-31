using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody rb;

    [SerializeField] private float upwardHelpIntensity = 4f;
    [SerializeField] private float pushIntensity = 1.0f;
    [SerializeField] private float cameraRotationSpeed = 2.0f;
    [SerializeField] private float maxCameraRotation = 80.0f;

    public Transform mainCameraTransform;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        // Add upward force to help the player stand up
        rb.AddForce(Vector3.up * upwardHelpIntensity * Time.deltaTime);

        float vertical = Input.GetAxis("Vertical") * pushIntensity * Time.deltaTime;
        float horizontal = Input.GetAxis("Horizontal") * pushIntensity * Time.deltaTime;

        // Move translation along the object's z-axis
        // transform.Translate(Vector3.forward * translation);
        rb.AddForce(Vector3.forward * vertical);

        // Push the player towards the right/left
        rb.AddForce(Vector3.right * horizontal);


        // Rotate the camera based on mouse input
        float mouseX = Input.GetAxis("Mouse X") * cameraRotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * cameraRotationSpeed * Time.deltaTime;

        // Rotate the camera horizontally around the player (yaw)
        mainCameraTransform.RotateAround(transform.position, Vector3.up, mouseX);

        // Rotate the camera vertically (pitch)
        Vector3 currentRotation = mainCameraTransform.localEulerAngles;
        currentRotation.x -= mouseY;
        currentRotation.x = Mathf.Clamp(currentRotation.x, -maxCameraRotation, maxCameraRotation);
        mainCameraTransform.localEulerAngles = currentRotation;
    }
}
