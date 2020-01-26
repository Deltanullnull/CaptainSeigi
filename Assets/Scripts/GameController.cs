using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject spawnA, spawnB;

    public PlayerMovement player;
    public Transform enemySocket;

    int downedEnemies = 0;

    int finishedWaves = 0;

    bool newWave = true;

    List<GameObject> enemiesInRange = new List<GameObject>();

    // Use this for initialization
    void Start () {
        player.OnEnemyEntered += ((e) =>
        {
            enemiesInRange.Add(e);
        });
        player.OnEnemyExited += ((e) =>
        {
            enemiesInRange.Remove(e);
        });

        StartCoroutine(SpawnRandomly());
    }

    // Update is called once per frame
    void Update () {
		
	}

    IEnumerator SpawnRandomly()
    {


        while (true)
        {
            if (enemySocket.childCount < 2 + finishedWaves && newWave)
            {
                SpawnEnemy();
                
                
            }

            if (enemySocket.childCount == 2+ finishedWaves)
            {
                newWave = false;
            }

            yield return null;

        }

        
    }

    private void SpawnEnemy()
    {
        GameObject spawnArea;

        int randSpawn = Random.Range(0, 2);
        spawnArea = randSpawn == 0 ? spawnA : spawnB;

        float posX = Random.Range(-1f, 1f) + spawnArea.transform.position.x;
        float posY = Random.Range(-1f, 1f) + spawnArea.transform.position.y;
        float posZ = Random.Range(-1f, 1f) + spawnArea.transform.position.z;

        GameObject newEnemy = Instantiate(Resources.Load("Prefabs/Enemy") as GameObject, enemySocket, true);

        newEnemy.transform.position = new Vector3(posX, posY, posZ);

        newEnemy.GetComponent<EnemyMovement>().OnKilled += ((e) =>
        {
            // TODO increment score
            downedEnemies++;

            if (enemySocket.childCount == 1)
            {
                finishedWaves++;
                newWave = true;
            }
        });



    }

}
