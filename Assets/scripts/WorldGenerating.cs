using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerating : MonoBehaviour
{
    private GameObject environment;
    
    [SerializeField] private string nameObject = "";
    [SerializeField] private GameObject[] prefabList;

    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 direction;
    [SerializeField] private int prefabAmount;
    [SerializeField] private float distancePrefab;
    [SerializeField] private float minPrefabSize;
    [SerializeField] private float maxPrefabSize;
    [SerializeField] private int rotation;

    [SerializeField] private int widthField;

    [SerializeField] private bool randomRotation;
    [SerializeField] private bool createField;


    void Start()
    {
        //Findet das GameObject in der Hierarchie
        environment = GameObject.Find("Environment");

        //Aufruf der Methoden: Wenn createField wahr ist wird die BuildField Methode ausgeführt, sonst wird BuildLinie ausgeführt
        if (createField == true)
        {
            BuildField(startPosition, direction, prefabAmount, distancePrefab, minPrefabSize, maxPrefabSize, widthField);
        }
        else
        {
            BuildLine(startPosition, direction, prefabAmount, distancePrefab, minPrefabSize, maxPrefabSize);
        }
    }

    
    private void BuildLine(Vector3 startPosition, Vector3 direction, int prefabAmount, float distancePrefab, float minPrefabSize, float maxPrefabSize)
    {        
        for (int i = 0;i < prefabAmount; i++) 
        {
            //Bei jedem Durchlauf wird die Position neu berechnet 
            Vector3 position = startPosition + direction * i * distancePrefab;

            GameObject randomPrefab = prefabList[Random.Range(0, prefabList.Length)];

            //Erzeugung der Klone
            GameObject prefab = Instantiate(randomPrefab, position, Quaternion.Euler(rotation,0,0), environment.transform);

            if (randomRotation == true)
            {
                prefab.transform.rotation = Quaternion.Euler(rotation, Random.Range(0,360), 0);
            }
            
        }        
    }

    //Anhand der widthField Variabel wird die Zählschleife definiert
    //Bei jedem Durchgang wird die startPosition um einen vektor (z achse) verschoben
    private void BuildField(Vector3 startPosition, Vector3 direction, int prefabAmount, float distancePrefab, float minPrefabSize, float maxPrefabSize, int widthField)
    {
        for(float i = 0f; i < widthField; i += 0.5f)
        {    
            BuildLine(startPosition + new Vector3(0,0,i), direction, prefabAmount, distancePrefab, minPrefabSize, maxPrefabSize);
        }
    }


}


