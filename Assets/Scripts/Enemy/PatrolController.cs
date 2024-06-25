using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolController : MonoBehaviour
{
    [SerializeField] private List<Transform> patrolPoints;
    private int currentPatrolIndex;
    private NavMeshAgent navMeshAgent;
    private EnemyController enemyController;
    [SerializeField] private bool isChasingPlayer;
    private Vector3 lastPlayerPosition;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyController = GetComponent<EnemyController>();

        if (patrolPoints != null && patrolPoints.Count > 0)
        {
            currentPatrolIndex = 0;
            MoveToNextPatrolPoint();
        }
    }

    void Update()
    {
        if (!isChasingPlayer && !navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            MoveToNextPatrolPoint();
        }

        if (enemyController != null)
        {
            if (enemyController.FindPlayer())
            {
                isChasingPlayer = true;
                lastPlayerPosition = enemyController.GetPlayerPosition();
                navMeshAgent.SetDestination(lastPlayerPosition);
            }
            else if (isChasingPlayer && !navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
            {
                StartCoroutine(WaitAtLastPlayerPosition());
            }
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
   

        // Rotate 90 degrees left
        Quaternion leftRotation = Quaternion.Euler(0, transform.eulerAngles.y - 90, 0);
        yield return RotateTo(leftRotation, 1f); // Rotate over 1 second
        yield return new WaitForSeconds(2f);
        // Rotate 180 degrees right
        Quaternion rightRotation = Quaternion.Euler(0, transform.eulerAngles.y + 180, 0);
        yield return RotateTo(rightRotation, 1f); // Rotate over 1 second

        isChasingPlayer = false;
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
}
