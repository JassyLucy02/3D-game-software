using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavigationAndAnimation : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    [SerializeField] [Tooltip ("Parent of the navigation objects")]
    private Transform route;

    private Vector3[] targetPositions;
    private int destinationIndex = 0;

    //Instanzierung der Route
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if(route != null)
        {
            InitRoute();
            agent.destination = targetPositions[destinationIndex];
        }

        agent.destination = targetPositions[destinationIndex];
    }


    private void InitRoute()
    {
        //Das Array wird mitgeteilt wie groﬂ es ist also wie viele Standpunkte es enth‰lt 
        targetPositions = new Vector3[route.childCount];

        //Es wird so lange gez‰hlt bis, alle Standpunkte von der Route eine Position bekommen haben
        for (int i = 0; i < targetPositions.Length; i++)
        {
            targetPositions[i] = route.GetChild(i).position;
        }
    }

}
