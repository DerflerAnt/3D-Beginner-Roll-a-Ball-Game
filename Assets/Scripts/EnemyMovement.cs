using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Drives an enemy towards the player using Unity's NavMesh system.
/// The enemy continuously re-targets the player's current position.
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    [Tooltip("Reference to the player's Transform")]
    public Transform player;

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (player == null) return;

        agent.SetDestination(player.position);
    }
}
