// DetectionSystem.cs
using UnityEngine;

public class DetectionSystem : MonoBehaviour
{
    public float detectionRange = 10f;
    Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < detectionRange)
        {
            // Player detected, trigger action
        }
    }
}
