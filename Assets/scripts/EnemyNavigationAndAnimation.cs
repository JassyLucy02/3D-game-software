using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavigationAndAnimation : MonoBehaviour
{
    private NavMeshAgent enemy;
    private Animator animator;

    [SerializeField]
    [Tooltip("Parent of the navigation objects")]
    private Transform route;

    private Vector3[] targetPositions;
    private int targetPositionIndex = 0;



    //Instanzierung der Route
    private void Start()
    {
        enemy = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (route != null)
        {
            InitRoute();
            StartCoroutine(Patroul());
        }

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

    private void Update()
    {
        if (enemy.remainingDistance < 2)
        {
            targetPositionIndex++;
            if (targetPositionIndex == targetPositions.Length)
            {
                targetPositionIndex = 0;
            }
            enemy.destination = targetPositions[targetPositionIndex];
        }
    }

    private IEnumerator Patroul()
    {
        enemy.isStopped = false;
        animator.SetBool("IsEnemyWalk", true);
        enemy.destination = targetPositions[targetPositionIndex];
        yield return new WaitForSeconds(Random.Range(30, 90));
        StartCoroutine(PauseEnemy());
    }

    private IEnumerator PauseEnemy()
    {
        enemy.isStopped = true;
        animator.SetBool("IsEnemyWalk", false);
        yield return new WaitForSeconds(Random.Range(10, 30));
        StartCoroutine(Patroul());
    }
}

