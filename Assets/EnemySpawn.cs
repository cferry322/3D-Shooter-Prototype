using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject[] spawnObjects;
    public GameObject spawnPosition;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            for (int i = 0; i < spawnObjects.Length; i ++)
            {
                spawnObjects[i].SetActive(true);
            }
        }
        else 
        {
            //Do nothing
        }
    }
}
