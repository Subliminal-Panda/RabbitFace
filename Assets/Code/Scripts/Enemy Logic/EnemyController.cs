// EnemyController.cs
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack,
        Retreat
    }

    public EnemyState currentState;

    NavMeshAgent navMeshAgent;
    Transform player;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = EnemyState.Patrol;
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                // Implement patrolling logic
                break;
            case EnemyState.Chase:
                // Implement chasing logic
                break;
            case EnemyState.Attack:
                // Implement attacking logic
                break;
            case EnemyState.Retreat:
                // Implement retreating logic
                break;
        }
    }

    // Other methods for handling state transitions, combat actions, etc.
}
