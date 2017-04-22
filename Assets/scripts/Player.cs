﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float moveForce;
    public float maxSpeed;

    Rigidbody body;

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        Vector3 input = new Vector3(-Input.GetAxis("Horizontal"), 0, -Input.GetAxis("Vertical"));
        if(input.magnitude > 1) { input = input.normalized; }
        if (body.velocity.magnitude < maxSpeed)
        {
            body.AddForce(input * moveForce);
        }

        if(transform.position.y < -5)
        {
            body.velocity = Vector3.zero;
            transform.position = Vector3.up * 2;
        }
    }
}
