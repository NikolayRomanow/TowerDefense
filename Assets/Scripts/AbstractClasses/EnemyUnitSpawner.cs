using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AbstractClasses
{
    public class EnemyUnitSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject enemy;
        [SerializeField] private float timeToSpawn;
        [SerializeField] private Transform playersCastle;

        private void Start()
        {
            StartCoroutine(EnemySpawner());
        }

        IEnumerator EnemySpawner()
        {
            while (true)
            {
                yield return new WaitForSeconds(timeToSpawn);
                Instantiate(enemy, transform.position, Quaternion.identity).GetComponent<Unit>().targetCastle =
                    playersCastle;
            }
        }
    }
}
