using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] item = new GameObject[10];
    public List<GameObject> itemList = new List<GameObject>();
    public Transform terrainTransform;

    public int SpawnMaxCount = 30;
    private float thresholdTime = 0;

    // Start is called before the first frame update
    void Update()
    {
        if (GameManager.instance.time > thresholdTime)
        {
            thresholdTime += 1.0f;
            Spawn();
        }
    }

    void Spawn()
    {
        if (itemList.Count >= SpawnMaxCount)
        {
            return;
        }

        int randomNum = Random.Range(0, 10);
        Vector3 spawnPos = new Vector3(Random.Range(-55.0f, 55.0f), 0.0f, Random.Range(-55.0f, 55.0f)) + terrainTransform.position;
        GameObject newItem = Instantiate(item[randomNum], spawnPos, Quaternion.identity);
        itemList.Add(newItem);
    }

    public void RemoveItem(GameObject gameObject)
    {
        itemList.Remove(gameObject);
        Destroy(gameObject);
    }
}
