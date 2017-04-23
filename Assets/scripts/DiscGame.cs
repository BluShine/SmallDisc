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

    public float gridSize = 2;
    public int gridSquares = 10;
    public float terrainHeight = 2;
    public Texture2D terrainData;

    public GameObject gridPrefab;
    GameObject grid;

    public Vector3 backgroundOffset;
    public float introSpeed = 5;
    float offsetLerp;

    public Gradient landColor;
    public Gradient cliffColor;
    public float cliffAngle = 30;

	// Use this for initialization
	void Start () {
        offsetLerp = introSpeed;

        weights = new List<Weight>();
		foreach(Weight w in FindObjectsOfType<Weight>())
        {
            weights.Add(w);
        }

        grid = GameObject.Instantiate(gridPrefab);

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Color> colors = new List<Color>();
        Vector3 gridCenter = new Vector3(-gridSize * gridSquares * .5f, 0, -gridSize * gridSquares * .5f);
        for(int x = 0; x < gridSquares; x++)
        {
            for(int y = 0; y < gridSquares; y++)
            { 

                float xRatio = (float)x / (float)(gridSquares);
                float yRatio = (float)y / (float)(gridSquares);
                float offset = 1f / (float)gridSquares;
                Color color = terrainData.GetPixelBilinear(xRatio, yRatio);
                float height = terrainHeight * color.r;

                vertices.Add(gridCenter + new Vector3(x * gridSize, 
                    terrainData.GetPixelBilinear(xRatio, yRatio).r * terrainHeight,
                    y * gridSize));
                vertices.Add(gridCenter + new Vector3(x * gridSize + gridSize,
                    terrainData.GetPixelBilinear(xRatio + offset, yRatio).r * terrainHeight, 
                    y * gridSize));
                vertices.Add(gridCenter + new Vector3(x * gridSize, 
                    terrainData.GetPixelBilinear(xRatio, yRatio + offset).r * terrainHeight,
                    y * gridSize + gridSize));
                vertices.Add(gridCenter + new Vector3(x * gridSize + gridSize, 
                    terrainData.GetPixelBilinear(xRatio + offset, yRatio + offset).r * terrainHeight,
                    y * gridSize + gridSize));

                int index = x * 4 * gridSquares + y * 4;

                triangles.Add(index + 0);
                triangles.Add(index + 2);
                triangles.Add(index + 1);
                triangles.Add(index + 1);
                triangles.Add(index + 2);
                triangles.Add(index + 3);

                Vector3 norm = Vector3.Cross(vertices[vertices.Count - 1] - vertices[vertices.Count - 2],
                    vertices[vertices.Count - 1] - vertices[vertices.Count - 3]);
                Color terrainColor = Color.green;
                if(Mathf.Abs(Vector3.Angle(norm, Vector3.down)) > cliffAngle)
                {
                    terrainColor = cliffColor.Evaluate(Random.value);
                } else
                {
                    terrainColor = landColor.Evaluate(Random.value);
                }
                colors.Add(terrainColor);
                colors.Add(terrainColor);
                colors.Add(terrainColor);
                colors.Add(terrainColor);
            }
        }
        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.SetColors(colors);
        mesh.RecalculateNormals();

        MeshFilter mFilter = grid.GetComponent<MeshFilter>();
        MeshCollider mCollider = grid.GetComponent<MeshCollider>();

        mFilter.mesh = mesh;
        mCollider.sharedMesh = mesh;
    }
	
	// Update is called once per frame
	void Update () {
        if(offsetLerp > 0)
        {
            offsetLerp -= Time.deltaTime;
            background.transform.position = Vector3.Lerp(Vector3.zero, backgroundOffset, offsetLerp / introSpeed);
        } else
        {
            background.transform.position = Vector3.zero;
        }

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
        turtle.transform.localRotation = Quaternion.Euler(0, - turtleDir, 0);
        //move the clouds
        wind.windMain = tilt.magnitude * windPower;
        wind.transform.rotation = Quaternion.Euler(0, -turtleDir, 0);
	}

    private void FixedUpdate()
    {
        if (tilt.magnitude > 23)
        {
            Physics.gravity = new Vector3(tilt.x, Physics.gravity.y, tilt.y);
        }
        else
        {
            tilt = Vector2.zero;
            foreach (Weight w in weights)
            {
                tilt += new Vector2(w.transform.position.x, w.transform.position.z) * w.weight;
            }
            tilt *= weightMultiplier;
        }
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
