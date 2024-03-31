using UnityEngine;

public class IKFootSolver : MonoBehaviour
{
    [SerializeField] LayerMask terrainLayer = default;
    [SerializeField] Transform body = default;
    [SerializeField] IKFootSolver otherFoot = default;
    [SerializeField] float speed = 1;
    [SerializeField] float stepDistance = 4;
    [SerializeField] float stepLength = 4;
    [SerializeField] float stepHeight = 1;
    [SerializeField] Vector3 footOffset = default;

    float footSpacing;
    Vector3 oldPosition, currentPosition, newPosition;
    Vector3 oldNormal, currentNormal, newNormal;
    float lerp;

    void Start()
    {
        footSpacing = transform.localPosition.x;
        currentPosition = newPosition = oldPosition = transform.position;
        currentNormal = newNormal = oldNormal = transform.up;
        lerp = 1;
    }

    void Update()
    {
        UpdateFootPositionAndNormal();
        MoveFoot();
    }

    void UpdateFootPositionAndNormal()
    {
        Vector3 rayDirection = Quaternion.Euler(0, transform.eulerAngles.y, 0) * (Vector3.right * footSpacing);

        // Create a ray from the side of the body pointing downwards
        Ray ray = new Ray(body.position + rayDirection, Vector3.down);
        Debug.DrawRay(body.position + rayDirection, Vector3.down, Color.green);

        // Check if the ray hits the terrain layer
        if (Physics.Raycast(ray, out RaycastHit hit, 10, terrainLayer.value))
        {
            // Check if the foot needs to take a step
            if (Vector3.Distance(newPosition, hit.point) > stepDistance && !otherFoot.IsMoving() && lerp >= 1)
            {
                // Set the target position for the foot
                int direction = body.InverseTransformPoint(hit.point).z > body.InverseTransformPoint(newPosition).z ? 1 : -1;
                newPosition = hit.point + (body.forward * stepLength * direction) + footOffset;
                newNormal = hit.normal;
                lerp = 0; // Start interpolation
            }
        }
    }


    void MoveFoot()
    {
        if (lerp < 1)
        {
            // Interpolate foot position and orientation
            Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            currentPosition = tempPosition;
            currentNormal = Vector3.Lerp(oldNormal, newNormal, lerp);
            lerp += Time.deltaTime * speed;
        }
        else if (Vector3.Distance(transform.position, newPosition) > 0.01f)
        {
            // Ensure that the foot is exactly at the target position before stopping interpolation
            currentPosition = newPosition;
            currentNormal = newNormal;
        }
        else
        {
            // Foot reached the target position, update old position and normal
            oldPosition = newPosition;
            oldNormal = newNormal;
        }

        // Apply the position and orientation to the foot
        transform.position = currentPosition;
        transform.up = currentNormal;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, 0.1f);
    }

    public bool IsMoving()
    {
        return lerp < 1;
    }
}