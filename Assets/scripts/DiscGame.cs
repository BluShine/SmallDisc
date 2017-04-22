using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscGame : MonoBehaviour {

    public Transform background;
    public Transform turtle;
    public WindZone wind;
    public float windPower = .1f;

    List<Weight> weights;

    Vector2 tilt;
    float turtleDir = 0;

    public float weightMultiplier = 1;

	// Use this for initialization
	void Start () {
        weights = new List<Weight>();
		foreach(Weight w in FindObjectsOfType<Weight>())
        {
            weights.Add(w);
        }
    }
	
	// Update is called once per frame
	void Update () {
        //rotate the disc
        background.transform.rotation = Quaternion.Slerp(background.transform.rotation, Quaternion.Euler(-tilt.y, 0, tilt.x), .1f);
        //spin the turtle
        Vector2 normT = tilt.normalized;
        float targetDir = Vector2.Angle(normT, Vector2.up);
        if(Vector3.Cross(normT, Vector2.up).z > 0)
        {
            targetDir = 360 - targetDir;
        }
        turtleDir = Mathf.LerpAngle(turtleDir, targetDir, .05f);
        turtle.transform.localRotation = Quaternion.Euler(-90, 0, 180 - turtleDir);
        //move the clouds
        wind.windMain = tilt.magnitude * windPower;
        wind.transform.rotation = Quaternion.Euler(0, -turtleDir, 0);
	}

    private void FixedUpdate()
    {
        tilt = Vector2.zero;
        foreach(Weight w in weights)
        {
            tilt += new Vector2(w.transform.position.x, w.transform.position.z) * w.weight;
        }
        tilt *= weightMultiplier;
    }

    public void AddWeight(Weight w)
    {
        weights.Add(w);
    }

    public void RemoveWeight(Weight w)
    {
        weights.Remove(w);
    }
}
