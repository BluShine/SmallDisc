using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weight : MonoBehaviour {

    public float weight;

    DiscGame game;
    [HideInInspector]
    public AudioSource sound;

	// Use this for initialization
	void Start () {
		if(game == null)
        {
            game = FindObjectOfType<DiscGame>();
        }
        game.AddWeight(this);

        sound = GetComponent<AudioSource>();
        sound.pitch = Random.Range(.7f, 2.5f);
        if(game.gameTimer < .2f)
        {
            sound.Stop();
        }
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (sound != null && !sound.isPlaying)
        {
            sound.pitch = Random.Range(.7f, 2.5f);
            sound.Play();
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
