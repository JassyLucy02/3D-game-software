using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerating : MonoBehaviour
{
    private GameObject environment;
    [SerializeField] private GameObject[] mediumAssetsList;

    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 direction;
    [SerializeField] private int prefabAmount;
    [SerializeField] private float distancePrefab;
    [SerializeField] private float minPrefabSize;
    [SerializeField] private float maxPrefabSize;

    void Start()
    {
        //Findet das GameObject in der Hierarchie
        environment = GameObject.Find("Environment");

        BuildLine(startPosition, direction, prefabAmount, distancePrefab, minPrefabSize, maxPrefabSize);

    }

    private void BuildLine(Vector3 startPosition, Vector3 direction, int prefabAmount, float distancePrefab, float minPrefabSize, float maxPrefabSize)
    {        
        for (int i = 0;i < prefabAmount; i++) 
        {
            Vector3 position = startPosition + direction * i * distancePrefab;

            //
            GameObject randomPrefab = mediumAssetsList[Random.Range(0, mediumAssetsList.Length)];


            GameObject plant = Instantiate(randomPrefab, position, randomPrefab.transform.rotation, environment.transform);
        }        
    }
}


