using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BridgeSpawner : MonoBehaviour
{
    public GameObject[] bridgePrefabs;

    enum enType
    {
        L_Corner,
        Straight,
        R_Corner,
    }

    enum enDirection
    {
        North,
        East,
        West,
    }
    class Segments
    {
        public GameObject segPrefab;
        public enType segType;

        public Segments(GameObject segPrefab, enType segType)
        {
            this.segPrefab = segPrefab;
            this.segType = segType;
        }
    };

    List<GameObject> activeSegments = new List<GameObject>();
    Segments segment;
    Vector3 spawnCoord = new Vector3(0, 0, -6.0f);
    enDirection segCurrentdirection = enDirection.North;
    enDirection segNextDirecton = enDirection.North;
    Transform playerTransform;

    float segOnScreen = 6.0f;
    float segWidth = 3.0f;
    int segsOnScreen = 5;
    private float segLength;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        segment = new Segments(bridgePrefabs[0], enType.Straight);
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        InitializeSegments();
       
    }

    // Update is called once per frame
    void Update()
    {
        PlayerTrigger();
    }

    void InitializeSegments()
    {
        for (int i = 0; i < segsOnScreen; i++)
        {
            
            SpawnSegments();
        }
    }

    void CreateSegments()
    {
        switch(segCurrentdirection)
        {
            case enDirection.North:
                segment.segType =(enType)Random.Range(0, 3);
                if(segment.segType == enType.Straight) { segment.segPrefab = bridgePrefabs[Random.Range(0, 6)]; }
                else if(segment.segType == enType.L_Corner) { segment.segPrefab = bridgePrefabs[7]; }
                else if(segment.segType == enType.R_Corner) { segment.segPrefab = bridgePrefabs[6]; }
                break;
            case enDirection.East:
                segment.segType = (enType)Random.Range(0, 2);
                if(segment.segType == enType.Straight) { segment.segPrefab = bridgePrefabs[Random.Range(0, 6)];  }
                else if(segment.segType == enType.L_Corner) { segment.segPrefab = bridgePrefabs[7]; }
                break;
            case enDirection.West:
                segment.segType = (enType)Random.Range(1, 3);
                if (segment.segType == enType.Straight) { segment.segPrefab = bridgePrefabs[Random.Range(0, 6)]; }
                else if (segment.segType == enType.R_Corner) { segment.segPrefab = bridgePrefabs[6]; }
                break;
        }
    }

    void SpawnSegments()
    {
        GameObject prefabToInstantiate = segment.segPrefab;
        Quaternion prefabRotation = Quaternion.identity;
        Vector3 offSet = new Vector3(0, 0, 0);

        switch(segCurrentdirection)
        {
            case enDirection.North:
                if (segment.segType == enType.Straight) { prefabRotation = Quaternion.Euler(000, 000, 000); segNextDirecton = enDirection.North; spawnCoord.z += segLength; }
                else if (segment.segType == enType.R_Corner) { prefabRotation = Quaternion.Euler(000, 000, 000); segNextDirecton = enDirection.East; spawnCoord.z += segLength; offSet.z += segLength + segWidth / 2; offSet.x += segWidth / 2; }
                else if (segment.segType == enType.L_Corner) { prefabRotation = Quaternion.Euler(000, 000, 000); segNextDirecton = enDirection.West; spawnCoord.z += segLength; offSet.z += segLength + segWidth / 2; offSet.x -= segWidth / 2; }
                break;


            case enDirection.East:
                if(segment.segType == enType.Straight) { prefabRotation = Quaternion.Euler(000, 090, 000); segNextDirecton = enDirection.East; spawnCoord.x += segLength; }
                else if(segment.segType == enType.L_Corner) { prefabRotation = Quaternion.Euler(000, 090, 000); segNextDirecton = enDirection.North; spawnCoord.x += segLength; offSet.z += segWidth / 2; offSet.x += segLength + segWidth / 2; }
                break;

            case enDirection.West:
                if (segment.segType == enType.Straight) { prefabRotation = Quaternion.Euler(000, -090, 000); segNextDirecton = enDirection.West; spawnCoord.x -= segLength; }
                else if (segment.segType == enType.R_Corner) { prefabRotation = Quaternion.Euler(000, -090, 000); segNextDirecton = enDirection.North; spawnCoord.x -= segLength; offSet.z += segWidth / 2; offSet.x += segLength + segWidth / 2; }
                break;
        }

        if(prefabToInstantiate != null)
        {
            GameObject spawnedPrefab = Instantiate(prefabToInstantiate, spawnCoord, prefabRotation) as GameObject;
            activeSegments.Add(spawnedPrefab);
            spawnedPrefab.transform.parent = this.transform;
        }

        segCurrentdirection = segNextDirecton;
        spawnCoord += offSet;
    }

    void RemoveSegments()
    {
        Destroy(activeSegments[0]);
        activeSegments.RemoveAt(0);
        
    }

    void PlayerTrigger()
    {
        GameObject go = activeSegments[0];

        if(Mathf.Abs(Vector3.Distance(playerTransform.position, go.transform.position)) > 16.0f)
        {
            CreateSegments();
            SpawnSegments();
            RemoveSegments();
        }
    }

}
