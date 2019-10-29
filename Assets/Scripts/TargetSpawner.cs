using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public GameObject SpawnItem = null;
    public List<GameObject> ItemList = new List<GameObject>();

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
        if (ItemList.Count >= SpawnMaxCount)
        {
            return;
        }

        Vector3 spawnPos = new Vector3(Random.Range(-10.0f, 40.0f), 6.0f, Random.Range(-20.0f, 30.0f));
        GameObject newItem = Instantiate(SpawnItem, spawnPos, Quaternion.identity);
        ItemList.Add(newItem);
    }

    public void RemoveItem(GameObject gameObject)
    {
        ItemList.Remove(gameObject);
    }
}
