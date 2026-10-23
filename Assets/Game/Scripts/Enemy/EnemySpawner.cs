using UnityEngine;

[System.Serializable]
public class SpawnEntry
{
    public Enemy prefab;
    public float weight = 1f;   // higher = spawns more often
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
        float total = 0f;
        foreach (var e in entries) total += e.weight;

        float roll = Random.value * total;
        foreach (var e in entries)
        {
            roll -= e.weight;
            if (roll <= 0f)
            {
                Instantiate(e.prefab, RandomPointOutsideScreen(), Quaternion.identity);
                return;
            }
        }
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