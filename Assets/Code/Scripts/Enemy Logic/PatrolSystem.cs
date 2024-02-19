// PatrolSystem.cs
using UnityEngine;

public class PatrolSystem : MonoBehaviour
{
    public Transform[] waypoints;
    int currentWaypointIndex;

    void Start()
    {
        currentWaypointIndex = 0;
    }

    public Vector3 GetNextWaypoint()
    {
        // Implement logic to determine the next waypoint
        // For example:
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        return waypoints[currentWaypointIndex].position;
    }
}
