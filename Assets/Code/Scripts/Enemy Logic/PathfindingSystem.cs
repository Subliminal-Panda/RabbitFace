// PathfindingSystem.cs
using UnityEngine;
using UnityEngine.AI;

public class PathfindingSystem : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    Transform player;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void SetDestination(Vector3 destination)
    {
        navMeshAgent.SetDestination(destination);
    }
}
