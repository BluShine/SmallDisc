using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDisc : MonoBehaviour {

    public float waterForce = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionStay(Collision collision)
    {
        collision.rigidbody.AddForce(-waterForce * (transform.position - collision.contacts[0].point).normalized,
            ForceMode.VelocityChange);
    }
}
