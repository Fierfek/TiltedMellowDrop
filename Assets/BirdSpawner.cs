using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSpawner : MonoBehaviour {

    public float spawnRate = 4; // 5 = average one bird every 5s
    public float height = 2.5f; // height from bottom
    float minSpawnDelay = 3; // must have minimum of X seconds before spawning another bird
    public bool spawningBirds = true; // If bird are spawning right now

	// Use this for initialization
	void Start () {
        StartCoroutine("SpawnBirds");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator SpawnBirds()
    {
        while(spawningBirds)
        {
            height = Random.value * 2.5f + 1f;
            if (Random.value <= 1 / spawnRate) // Try to spawn
            {
                GameObject bird = Instantiate((GameObject)Resources.Load("Prefabs/Bird"), new Vector3(Camera.main.orthographicSize * Screen.width / Screen.height + 1, Camera.main.transform.position.y + height), Quaternion.identity);
                while (bird != null) // Wait until previous bird has passed (and killed itself) rip
                    yield return new WaitForFixedUpdate();
                yield return new WaitForSeconds(minSpawnDelay); 
                // Delays [minSpawnDelay] seconds after previous bird passes until can spawn another bird
            }
            yield return new WaitForSeconds(1f); // delay .5s so spawnRate actually controls spawn rate (without waiting, it would try to spawn every frame)
        }
    }
}
