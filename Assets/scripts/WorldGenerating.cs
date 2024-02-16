using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerating : MonoBehaviour
{
    private GameObject environment;
    [SerializeField] private GameObject[] mediumAssetsList;

    Vector3 startPosition;
    Vector3 directionX;
    Vector3 directionY;
    int prefabAmount;
    float distancePrefab;
    
    void Start()
    {
        startPosition = new Vector3(0,0,0);
        directionX = new Vector3(0, 0, 1);
        directionY = new Vector3(1, 0, 0);

        //Findet das GameObject in der Hierarchie
        /*environment = GameObject.Find("Environment");

        for (int x = 0; x < 10; x++)
        {
             GameObject plant = Instantiate(mediumAssetsList[0], new Vector3(x,0,0), Quaternion.identity);
             plant.transform.parent = environment.transform;
        }*/

    }

    private void BuildLine(Vector3 startPosition, int prefabAmount, float distancePrefab, Vector3 direction)
    {        
        for (int x = 0;x < prefabAmount; x++) 
        {
            Vector3 position = startPosition + Random.Range(directionX,directionY) * x * distancePrefab;
        }
    }
}


