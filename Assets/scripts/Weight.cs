using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weight : MonoBehaviour {

    public float weight;

    DiscGame game;

	// Use this for initialization
	void Start () {
        Rigidbody b = GetComponent<Rigidbody>();
        if (b != null)
        {
            b.mass = weight;
        }
		if(game == null)
        {
            game = FindObjectOfType<DiscGame>();
        }
	}

    private void FixedUpdate()
    {
        if(transform.position.magnitude > 27 || transform.position.y < 0)
        {
            game.RemoveWeight(this);
        }
        if(transform.position.y < -10)
        {
            Destroy(this);
        }
    }
}
