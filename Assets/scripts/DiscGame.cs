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

    bool[,] flatTerrain; //true where the grid is flat

    public GameObject treePrefab;

    Tree[,] trees;

    public TextMesh loseText;

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
        int triIndex = 0;
        flatTerrain = new bool[gridSquares,gridSquares];
        trees = new Tree[gridSquares, gridSquares];
        for(int x = 0; x < gridSquares; x++)
        {
            for(int y = 0; y < gridSquares; y++)
            {
                float gridDistance = new Vector2((x + .5f - gridSquares / 2) * gridSize,
                    (y + .5f - gridSquares / 2) * gridSize).magnitude;
                if (gridDistance <= gridSize * gridSquares / 2f)
                {

                    float xRatio = (float)x / (float)(gridSquares);
                    float yRatio = (float)y / (float)(gridSquares);
                    float offset = 1f / (float)gridSquares;
                    float height1 = terrainHeight * terrainData.GetPixelBilinear(xRatio, yRatio).r;
                    float height2 = terrainHeight * terrainData.GetPixelBilinear(xRatio + offset, yRatio).r;
                    float height3 = terrainHeight * terrainData.GetPixelBilinear(xRatio, yRatio + offset).r;
                    float height4 = terrainHeight * terrainData.GetPixelBilinear(xRatio + offset, yRatio + offset).r;

                    vertices.Add(gridCenter + new Vector3(x * gridSize,
                        height1,
                        y * gridSize));
                    vertices.Add(gridCenter + new Vector3(x * gridSize + gridSize,
                        height2,
                        y * gridSize));
                    vertices.Add(gridCenter + new Vector3(x * gridSize,
                        height3,
                        y * gridSize + gridSize));
                    vertices.Add(gridCenter + new Vector3(x * gridSize + gridSize,
                        height4,
                        y * gridSize + gridSize));

                    triangles.Add(triIndex + 0);
                    triangles.Add(triIndex + 2);
                    triangles.Add(triIndex + 1);
                    triangles.Add(triIndex + 1);
                    triangles.Add(triIndex + 2);
                    triangles.Add(triIndex + 3);

                    triIndex += 4;

                    flatTerrain[x, y] = true;

                    if(height1 < 1.5f || height2 < 1.5f || height3 < 1.5f || height4 <1.5f)
                    {
                        flatTerrain[x, y] = false;
                    }

                    Vector3 norm = Vector3.Cross(vertices[vertices.Count - 1] - vertices[vertices.Count - 2],
                        vertices[vertices.Count - 1] - vertices[vertices.Count - 3]);
                    Color terrainColor = Color.green;
                    if (Mathf.Abs(Vector3.Angle(norm, Vector3.down)) > cliffAngle)
                    {
                        terrainColor = cliffColor.Evaluate(Random.value);
                        flatTerrain[x, y] = false;
                    }
                    else
                    {
                        terrainColor = landColor.Evaluate(Random.value);
                    }
                    //if(flatTerrain[x, y]) { terrainColor = Color.red; }
                    colors.Add(terrainColor);
                    colors.Add(terrainColor);
                    colors.Add(terrainColor);
                    colors.Add(terrainColor);

                    Color centerColor = terrainData.GetPixelBilinear(xRatio + offset * .5f, yRatio + offset * .5f);
                    if (centerColor.g > .5f && flatTerrain[x, y])
                    {
                        addTree(x, y, (height1 + height2 + height3 + height4) / 4f);
                    }
                } else
                {
                    //outside the circle.
                    flatTerrain[x, y] = false;
                }

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
            Physics.gravity = new Vector3(tilt.x, 0, tilt.y);
            if(loseText.color.a < 1)
            {
                loseText.color = new Color(1, 0, 0, loseText.color.a + .05f);
            }
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

    void addTree(int x, int y, float height)
    {
        Transform t = GameObject.Instantiate(treePrefab).transform;
        t.position = new Vector3(
            (-x - Random.Range(.3f, .7f)) * gridSize + gridSize * gridSquares / 2, 
            height, 
            (-y - Random.Range(.3f, .7f)) * gridSize + gridSize * gridSquares / 2);
        t.rotation = Quaternion.EulerAngles(new Vector3(0, Random.Range(0, 360), 0));
        trees[x, y] = t.GetComponent<Tree>();
        AddWeight(trees[x, y]);
    }
}
