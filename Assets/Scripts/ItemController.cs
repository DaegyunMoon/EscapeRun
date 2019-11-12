using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    private ItemSpawner itemSpawner;
    private PlayerControl player;
    
    // Start is called before the first frame update
    void Start()
    {
        itemSpawner = FindObjectOfType<ItemSpawner>();
        player = FindObjectOfType<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            itemSpawner.RemoveItem(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            player.FullHP();
            itemSpawner.RemoveItem(this.gameObject);
        }
    }

}
