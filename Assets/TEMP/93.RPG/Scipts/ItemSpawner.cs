using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    //생성할 아이템 소스
    public GameObject SpawnItem = null;
    public List<GameObject> ItemList = new List<GameObject>();

    public int SpawnMaxCount = Score.itemSpawnLimit;
    private double thresholdTime = 2;

    // Start is called before the first frame update
    void Update()
    {
        if (GameControl.instance.MyGameState == GameControl.GameState.Playing && TimeManager.instance.MyPlayTime >= thresholdTime)
        {
            Spawn();
            thresholdTime += 2;
        }
    }

    void Spawn()
    {
        if (ItemList.Count >= SpawnMaxCount)
        {
            Debug.Log("Not Spawned");
            return;
        }

        Vector3 spawnPos = new Vector3(Random.Range(-40.0f, 40.0f), 0.5f, Random.Range(-40.0f, 40.0f));
        
        /*
        Ray ray = new Ray(spawnPos, Vector3.down);
        RaycastHit raycastHit = new RaycastHit();
        if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity) == true)
        {
            spawnPos.y = raycastHit.point.y;
        }
        */

        GameObject newItem = Instantiate(SpawnItem, spawnPos, Quaternion.identity);
        ItemList.Add(newItem);
    }

    public void RemoveItem(GameObject gameObject)
    {
        ItemList.Remove(gameObject);
    }
}
