using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnEntry
{
    public Enemy prefab;
    public float weight = 1f;   // higher = spawns more often
    public int maxAlive = 10;   // most of this type allowed on screen at once

    [System.NonSerialized] List<Enemy> alive;

    public int AliveCount
    {
        get
        {
            if (alive == null) alive = new List<Enemy>();
            alive.RemoveAll(e => e == null);   // drop enemies that have been destroyed
            return alive.Count;
        }
    }

    public bool CanSpawn => prefab != null && weight > 0f && AliveCount < maxAlive;

    public void Track(Enemy enemy)
    {
        if (alive == null) alive = new List<Enemy>();
        alive.Add(enemy);
    }
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] SpawnEntry[] entries;
    [SerializeField] float spawnInterval = 2f;
    [SerializeField] float extraDistance = 2f;   // how far past the screen corner to spawn

    float timer;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            Spawn();
        }
    }

    void Spawn()
    {
        // Only types under their specified spawn cap can be picked
        float total = 0f;
        SpawnEntry last = null;
        foreach (var e in entries)
        {
            if (!e.CanSpawn) continue;
            total += e.weight;
            last = e;
        }
        if (last == null) return;  // every type is at it cap, skip this spawn

        float roll = Random.value * total;
        foreach (var e in entries)
        {
            if (!e.CanSpawn) continue;
            roll -= e.weight;
            if (roll <= 0f)
            {
                SpawnOne(e);
                return;
            }
        }

        SpawnOne(last);   // safety net for floating point rounding
    }

    void SpawnOne(SpawnEntry entry)
    {
        Enemy spawned = Instantiate(entry.prefab, RandomPointOutsideScreen(), Quaternion.identity);
        entry.Track(spawned);
    }

    Vector2 RandomPointOutsideScreen()
    {
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        float radius = Mathf.Sqrt(halfW * halfW + halfH * halfH) + extraDistance;

        float angle = Random.value * Mathf.PI * 2f;
        Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        return (Vector2)cam.transform.position + dir * radius;
    }
}