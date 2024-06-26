using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    [SerializeField] private Animator animator; // Add the Animator field
    public EnemyData enemyData;
    private RaycastHit raycastHitInfo;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask ignoreLayers; // Layers to ignore during raycasting
    [SerializeField] private LineRenderer frontFOVLineRenderer; // Line renderer for the front FOV zone
    [SerializeField] private MeshFilter fovMeshFilter; // List of MeshFilters for each zone
    [SerializeField] private MeshRenderer fovMeshRenderer; // List of MeshRenderers for each zone

    [SerializeField] private List<Transform> patrolPoints;
    private int currentPatrolIndex;
    [SerializeField] private bool isChasingPlayer;
    private bool isRotating;
    private bool wasAttack;
    private Vector3 lastPlayerPosition;
    private Coroutine agroCoroutine; // Reference to the running Agro coroutine
    [SerializeField] private float minDistanceToAttack = 2.0f; // Minimum distance to attack

    private ShootController shootController; // Reference to the ShootController

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        shootController = GetComponent<ShootController>();

        if (patrolPoints != null && patrolPoints.Count > 0)
        {
            currentPatrolIndex = 0;
            MoveToNextPatrolPoint();
        }
    }

    void Update()
    {
        if (wasAttack) return;
        DrawFrontFOV();
        HandlePatrolAndChase();
        UpdateAnimator();
        CheckAttack();
    }

    void HandlePatrolAndChase()
    {
        if (isRotating)
            return;

        if (!isChasingPlayer && !navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            MoveToNextPatrolPoint();
        }

        if (FindPlayer())
        {
            lastPlayerPosition = GetPlayerPosition();
            navMeshAgent.SetDestination(lastPlayerPosition);
            shootController.StartShooting(); // Start shooting when chasing the player
        }
        else if (isChasingPlayer && !navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            StartCoroutine(WaitAtLastPlayerPosition());
        }
    }

    void MoveToNextPatrolPoint()
    {
        if (patrolPoints.Count == 0)
            return;

        navMeshAgent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
    }

    IEnumerator WaitAtLastPlayerPosition()
    {
        isRotating = true;

        // Rotate 90 degrees left
        Quaternion leftRotation = Quaternion.Euler(0, transform.eulerAngles.y - 90, 0);
        yield return RotateTo(leftRotation, 1f); // Rotate over 1 second
        yield return new WaitForSeconds(2f);

        // Rotate 180 degrees right
        Quaternion rightRotation = Quaternion.Euler(0, transform.eulerAngles.y + 180, 0);
        yield return RotateTo(rightRotation, 1f); // Rotate over 1 second
        yield return new WaitForSeconds(2f);

        isRotating = false;
        MoveToNextPatrolPoint();
    }

    IEnumerator RotateTo(Quaternion targetRotation, float duration)
    {
        Quaternion startRotation = transform.rotation;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
    }

    public void Agro(Vector3 point)
    {
        if (agroCoroutine != null)
        {
            StopCoroutine(agroCoroutine);
        }

        navMeshAgent.SetDestination(point);
        agroCoroutine = StartCoroutine(CheckArrivalAndLookAround());
    }

    IEnumerator CheckArrivalAndLookAround()
    {
        while (navMeshAgent.pathPending)
        {
            yield return null;
        }

        while (navMeshAgent.remainingDistance > 0.5f)
        {
            yield return null;
        }

        agroCoroutine = StartCoroutine(LookAroundAfterAgro());
    }

    IEnumerator LookAroundAfterAgro()
    {
        isRotating = true;

        // Rotate 90 degrees left
        Quaternion leftRotation = Quaternion.Euler(0, transform.eulerAngles.y - 90, 0);
        yield return RotateTo(leftRotation, 1f); // Rotate over 1 second
        yield return new WaitForSeconds(2f);

        // Rotate 180 degrees right
        Quaternion rightRotation = Quaternion.Euler(0, transform.eulerAngles.y + 180, 0);
        yield return RotateTo(rightRotation, 1f); // Rotate over 1 second
        yield return new WaitForSeconds(2f);

        isRotating = false;
        MoveToNextPatrolPoint();
    }

    public bool FindPlayer()
    {
        if (PlayerController.Instance.isHide)
        {
            PlayerController.Instance.RemoveChase(this);
            isChasingPlayer = false;
            shootController.StopShooting(); // Stop shooting when not chasing the player
            return false;
        }

        if (IsPlayerVisibleInFrontFOV(player.position))
        {
            navMeshAgent.SetDestination(player.position);
            PlayerController.Instance.AddChase(this);
            isChasingPlayer = true;
            return true;
        }
        else
        {
            PlayerController.Instance.RemoveChase(this);
            isChasingPlayer = false;
            shootController.StopShooting(); // Stop shooting when not chasing the player
        }
        isChasingPlayer = false;
        shootController.StopShooting(); // Stop shooting when not chasing the player
        return false;
    }

    public Vector3 GetPlayerPosition()
    {
        return player.position;
    }

    public bool IsPlayerVisibleInFrontFOV(Vector3 position)
    {
        Vector3 directionToPosition = (position - transform.position).normalized;
        float angleBetweenEnemyAndPosition = Vector3.Angle(transform.forward, directionToPosition);

        Color rayColor = Color.green; // Default to green if not visible
        Vector3 rayOrigin = transform.position + new Vector3(0, 0.65f, 0); // Adjust ray origin to enemy's eye level

        if (angleBetweenEnemyAndPosition <= enemyData.frontFOVAngle / 2)
        {
            // Cast a ray to check for direct line of sight, ignoring specified layers
            if (Physics.Raycast(rayOrigin, directionToPosition, out raycastHitInfo, enemyData.frontFOVDistance, ~ignoreLayers))
            {
                //Debug.Log(raycastHitInfo.collider.gameObject.name);
                // Check if the hit object is the player and no other object is in between
                if (raycastHitInfo.transform == player)
                {
                    // Check if there's an obstacle in between using the wall layer mask
                    if (!Physics.Raycast(rayOrigin, directionToPosition, raycastHitInfo.distance, enemyData.wallLayer))
                    {
                        rayColor = Color.red; // Change to red if visible
                        Debug.DrawRay(rayOrigin, directionToPosition * raycastHitInfo.distance, rayColor);
                        return true;
                    }
                    else
                    {
                        rayColor = Color.yellow; // Change to yellow if an obstacle is detected
                        Debug.DrawRay(rayOrigin, directionToPosition * raycastHitInfo.distance, rayColor);
                        return false;
                    }
                }
            }
        }

        // Draw the ray in the Scene view
        Debug.DrawRay(rayOrigin, directionToPosition * enemyData.frontFOVDistance, rayColor);
        return false;
    }

    void DrawFrontFOV()
    {
        if (frontFOVLineRenderer == null)
        {
            GameObject lineObj = new GameObject("FrontFOVLineRenderer");
            lineObj.transform.SetParent(transform);
            frontFOVLineRenderer = lineObj.AddComponent<LineRenderer>();
            frontFOVLineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Add a default material

            GameObject meshObj = new GameObject("FrontFOVMeshFilter");
            meshObj.transform.SetParent(transform);
            MeshFilter mf = meshObj.AddComponent<MeshFilter>();
            MeshRenderer mr = meshObj.AddComponent<MeshRenderer>();
            mr.material = new Material(Shader.Find("Standard"));
            fovMeshFilter = mf;
            fovMeshRenderer = mr;
        }

        frontFOVLineRenderer.startColor = enemyData.frontFOVColor;
        frontFOVLineRenderer.endColor = enemyData.frontFOVColor;
        frontFOVLineRenderer.widthMultiplier = 0.1f;
        DrawDetectionRadius(frontFOVLineRenderer, enemyData.frontFOVDistance, enemyData.frontFOVAngle);

        frontFOVLineRenderer.material.color = enemyData.frontFOVColor; // Set the color of the mesh renderer
        CreateMeshFromLineRenderer(frontFOVLineRenderer, fovMeshFilter);
    }

    void DrawDetectionRadius(LineRenderer lineRenderer, float radius, float fov)
    {
        int stepCount = Mathf.RoundToInt(fov * 10);
        float stepAngleSize = fov / stepCount;

        List<Vector3> fovPoints = new List<Vector3> { transform.position };

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = -fov / 2 + stepAngleSize * i;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            Vector3 endPoint = transform.position + direction * radius;

            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, radius, enemyData.wallLayer))
            {
                endPoint = hit.point;
            }

            fovPoints.Add(endPoint);
        }
        fovPoints.Add(transform.position);

        lineRenderer.positionCount = fovPoints.Count;
        lineRenderer.SetPositions(fovPoints.ToArray());
    }

    void CreateMeshFromLineRenderer(LineRenderer lineRenderer, MeshFilter meshFilter)
    {
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positions);

        List<Vector3> vertices = new List<Vector3> { Vector3.zero };

        for (int i = 0; i < positions.Length; i++)
        {
            Vector3 localPos = transform.InverseTransformPoint(positions[i]);
            vertices.Add(localPos);
        }

        List<int> triangles = new List<int>();

        for (int i = 1; i < positions.Length; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1 < positions.Length ? i + 1 : 1);
        }

        Mesh mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray()
        };
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    void OnDrawGizmos()
    {
        if (patrolPoints == null || patrolPoints.Count == 0)
            return;

        Gizmos.color = Color.blue;

        for (int i = 0; i < patrolPoints.Count; i++)
        {
            Vector3 currentPoint = patrolPoints[i].position;
            Vector3 nextPoint = patrolPoints[(i + 1) % patrolPoints.Count].position;

            Gizmos.DrawSphere(currentPoint, 0.3f);
            Gizmos.DrawLine(currentPoint, nextPoint);
        }
    }

    // Method to update the Animator based on the NavMeshAgent's velocity
    void UpdateAnimator()
    {
        float velocity = navMeshAgent.velocity.magnitude;
        animator.SetFloat("Velocity", velocity > 0.1f ? 1f : 0f); // Set "Velocity" to 1 for walking, 0 for idle
    }

    // Method to check if the enemy is close enough to attack
    void CheckAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= minDistanceToAttack && FindPlayer() && !wasAttack)
        {
            wasAttack = true;
            GetComponent<NavMeshAgent>().enabled = false;
       //     GetComponent<CharacterController>().enabled = false;
           
            PlayerController.Instance.GameOver(transform.position);
            animator.SetTrigger("Attack");
        }
    }
}
