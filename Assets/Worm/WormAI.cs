using Cinemachine;
using System.Collections;
using UnityEngine;


public class WormAI : MonoBehaviour
{
    [Header("Pathing")]
    [SerializeField] CinemachineSmoothPath path = default;
    [SerializeField] CinemachineDollyCart cart = default;
    [SerializeField] LayerMask terrainLayer = default;
    Player player;

    [HideInInspector] public Vector3 startPosition, endPosition;

    RaycastHit hitInfo;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();

        AI();
    }
    private void AI()
    {
        UpdatePath();
        StartCoroutine(FollowPath());
        IEnumerator FollowPath()
        {
            while (true)
            {
                // wait a beat to come out of ground again
                yield return new WaitUntil(() => cart.m_Position >= 0.99f);
                yield return new WaitForSeconds(Random.Range(1, 2));

                //reset path
                UpdatePath();
                yield return new WaitUntil(() => cart.m_Position <= 0.05f);
            }
        }
    }

    private void UpdatePath()
    {
        Vector3 playerPosition = player.transform.position;
        playerPosition.y = Mathf.Max(10, playerPosition.y);
        Vector3 randomRange = Random.insideUnitSphere * 100;
        randomRange.y = 0;
        startPosition = playerPosition + randomRange;
        endPosition = playerPosition - randomRange;

        if (Physics.Raycast(startPosition, Vector3.down, out hitInfo, 1000, terrainLayer.value))
        {
            startPosition = hitInfo.point;

        }

        if (Physics.Raycast(endPosition, Vector3.down, out hitInfo, 1000, terrainLayer.value))
        {
            endPosition = hitInfo.point;
        }

        path.m_Waypoints[0].position = startPosition + (Vector3.down * 15);
        path.m_Waypoints[1].position = playerPosition + (Vector3.up * 10);
        path.m_Waypoints[2].position = endPosition + (Vector3.down * 45);

        path.InvalidateDistanceCache();
        cart.m_Position = 0;

        //speed
        cart.m_Speed = cart.m_Path.PathLength / 1500;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(startPosition, 1);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(endPosition, 1);

    }
}
