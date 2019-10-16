using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemControl : MonoBehaviour
{
    public ItemSpawner itemSpawner;

    private void Start()
    {
        itemSpawner = GameObject.FindObjectOfType<ItemSpawner>();
        transform.Rotate(new Vector3(15, 0, 0));
    }
    private void Update()
    {
        transform.Rotate(new Vector3(0, 60, 0) * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") == true)
        {
            Destroy(gameObject);
            itemSpawner.RemoveItem(gameObject);
        }
    }
}