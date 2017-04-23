using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public float interval = 10f;
    public int spawns = 3;

    public GameObject prefab;
    public Vector3 offset;

    float timer = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if(timer > interval)
        {
            timer -= interval;
            if(spawns > 0)
            {
                spawns--;
                GameObject.Instantiate(prefab, transform.position + offset + 
                    Vector3.right * Random.Range(-5f, 5f) + Vector3.forward * Random.Range(-5f, 5f), 
                    Quaternion.Euler(0, Random.value * 360, 0));
            }
        }
    }
}
