using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject targetObject = null;
    public List<GameObject> targetList = new List<GameObject>();
    public Transform terrainTransform;
    public int SpawnMaxCount = 5;
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
        if (targetList.Count >= SpawnMaxCount)
        {
            return;
        }
        Vector3 spawnPos = GetSpawnPos() + terrainTransform.position;
        GameObject newItem = Instantiate(targetObject, spawnPos, Quaternion.identity);
        targetList.Add(newItem);
    }

    Vector3 GetSpawnPos()
    {
        float posX = Random.Range(-55.0f, 55.0f);
        float posZ = Random.Range(-55.0f, 55.0f);
        float posY;
        if (posX > 0.0f && posZ > 0.0f)
        {
            posY = 5.0f;
        }
        else if (posX < 0.0f && posZ < 0.0f)
        {
            posY = -5.0f;
        }
        else
        {
            posY = 0.0f;
        }
        return new Vector3(posX, posY, posZ);
    }

    public void RemoveTarget(GameObject gameObject)
    {
        targetList.Remove(gameObject);
        Destroy(gameObject);
    }
}
