using System.Collections.Generic;
using UnityEngine;

public class BridgeSpawner : MonoBehaviour
{
    public GameObject[] bridgePrefabs;      // Bridge prefabs
    public Transform playerTransform;       // Player transform

    enum enType { Straight, L_Corner, R_Corner }
    enum enDirection { North, East, West }

    List<GameObject> activeSegments = new List<GameObject>();

    Vector3 spawnPos;
    enDirection currentDir = enDirection.North;

    float segLength = 6f;  // segment length
    float segWidth = 3f;   // bridge width for corners
    int segsOnScreen = 6;  // initial segments

    public float bridgeHeight = 2.7f; // height from ground

    void Start()
    {
        // ✅ Find player automatically
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
            else
                Debug.LogError("Player not found! Tag it 'Player'");
        }

        // ✅ Start position exactly where the first segment will be
        spawnPos = new Vector3(0, bridgeHeight, 0);

        // ✅ Spawn first segment directly under player
        SpawnSegment(enType.Straight);

        // ✅ Snap player exactly on top of the first segment
        if (playerTransform != null && activeSegments.Count > 0)
        {
            Vector3 firstSegmentPos = activeSegments[0].transform.position;
            playerTransform.position = new Vector3(firstSegmentPos.x, firstSegmentPos.y + 0.1f, firstSegmentPos.z);
        }

        // ✅ Spawn remaining segments ahead
        for (int i = 1; i < segsOnScreen; i++)
        {
            SpawnSegment(enType.Straight);
        }
    }

    void Update()
    {
        if (playerTransform == null || activeSegments.Count == 0) return;

        // ✅ Spawn new segments ahead of player
        if (Vector3.Distance(playerTransform.position, activeSegments[0].transform.position) > 12f)
        {
            SpawnRandom();
            RemoveOld();
        }
    }

    void SpawnRandom()
    {
        int rand = Random.Range(0, 3);
        SpawnSegment((enType)rand);
    }

    void SpawnSegment(enType type)
    {
        GameObject prefab;

        if (type == enType.Straight)
            prefab = bridgePrefabs[Random.Range(0, 11)];
        else if (type == enType.L_Corner)
            prefab = bridgePrefabs[12];
        else
            prefab = bridgePrefabs[11];

        Quaternion rot = Quaternion.identity;
        Vector3 forward = Vector3.forward;
        Vector3 right = Vector3.right;

        // ✅ Determine orientation
        switch (currentDir)
        {
            case enDirection.North:
                rot = Quaternion.Euler(0, 0, 0);
                forward = Vector3.forward;
                right = Vector3.right;
                break;

            case enDirection.East:
                rot = Quaternion.Euler(0, 90, 0);
                forward = Vector3.right;
                right = Vector3.back;
                break;

            case enDirection.West:
                rot = Quaternion.Euler(0, -90, 0);
                forward = Vector3.left;
                right = Vector3.forward;
                break;
        }

        // ✅ Move spawnPos forward for segments AFTER first
        if (activeSegments.Count > 0) // only move if not the first segment
            spawnPos += forward * segLength;

        // ✅ Corner offsets
        if (type == enType.L_Corner)
            spawnPos += right * (-segWidth / 2f);
        else if (type == enType.R_Corner)
            spawnPos += right * (segWidth / 2f);

        // ✅ Instantiate segment
        GameObject seg = Instantiate(prefab, spawnPos, rot);
        activeSegments.Add(seg);

        // ✅ Update direction after corner
        if (type == enType.L_Corner)
        {
            if (currentDir == enDirection.North) currentDir = enDirection.West;
            else if (currentDir == enDirection.East) currentDir = enDirection.North;
        }
        else if (type == enType.R_Corner)
        {
            if (currentDir == enDirection.North) currentDir = enDirection.East;
            else if (currentDir == enDirection.West) currentDir = enDirection.North;
        }
    }

    void RemoveOld()
    {
        if (activeSegments.Count == 0) return;
        Destroy(activeSegments[0]);
        activeSegments.RemoveAt(0);
    }

    public void CleanTheScene()
    {
        foreach (var seg in activeSegments)
            Destroy(seg);

        activeSegments.Clear();

        spawnPos = new Vector3(0, bridgeHeight, 0);
        currentDir = enDirection.North;

        // ✅ Spawn first segment under player
        SpawnSegment(enType.Straight);

        // ✅ Snap player on bridge
        if (playerTransform != null && activeSegments.Count > 0)
        {
            Vector3 firstSegmentPos = activeSegments[0].transform.position;
            playerTransform.position = new Vector3(firstSegmentPos.x, firstSegmentPos.y + 0.1f, firstSegmentPos.z);
        }

        // ✅ Spawn remaining segments ahead
        for (int i = 1; i < segsOnScreen; i++)
        {
            SpawnSegment(enType.Straight);
        }
    }
}