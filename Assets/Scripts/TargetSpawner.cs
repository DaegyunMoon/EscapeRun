using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public GameObject gameObject = null;
    public List<GameObject> targetList = new List<GameObject>();

    public int SpawnMaxCount = 5;
    private float thresholdTime = 0;

    // Start is called before the first frame update
    void Update()
    {
        if(Time.time > thresholdTime)
        {
            thresholdTime += 1.0f;
            Spawn();
        }
    }

    void Spawn()
    {
        if (targetList.Count >= SpawnMaxCount)
        {
            return;
        }

        Vector3 spawnPos = new Vector3(Random.Range(-10.0f, 40.0f), 6.0f, Random.Range(-20.0f, 30.0f));
        GameObject newItem = Instantiate(gameObject, spawnPos, Quaternion.identity);
        targetList.Add(newItem);
    }

    public void RemoveTarget(GameObject gameObject)
    {
        targetList.Remove(gameObject);
        Destroy(gameObject);
    }
}
