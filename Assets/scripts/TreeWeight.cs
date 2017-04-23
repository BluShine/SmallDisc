using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeWeight : Weight {

    public float growTimer = 0;

    public bool fallen = false;

    private void FixedUpdate()
    {
        if(!fallen) growTimer += Time.fixedDeltaTime * Random.value;
    }

    public void fall()
    {
        gameObject.AddComponent<Rigidbody>();
        gameObject.GetComponent<Collider>().enabled = true;
        fallen = true;
    }
}
