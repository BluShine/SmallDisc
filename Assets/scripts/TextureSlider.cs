using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureSlider : MonoBehaviour {

    Material mat;
    public float speed = 10;
    float time = 0;
    public Vector2 scale;

	// Use this for initialization
	void Start () {
        mat = GetComponent<MeshRenderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime;
        time = time % speed;
        mat.mainTextureOffset = Vector2.Scale(scale, new Vector2(Mathf.Sin(Mathf.PI * 2 * time / speed), 
            Mathf.Cos(Mathf.PI * 6 * time / speed)));
	}
}
