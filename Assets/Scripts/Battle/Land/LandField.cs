using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandField : MonoBehaviour
{

    public int row = 16;
    public int colume = 8;
    public float landWidth = 5.0f;
    public float landHeight = 10.0f;

    public float landPosY = 2.0f;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private Vector3[] vertices;
    private Vector2[] uv;
    private int[] triangles;
    // Start is called before the first frame update
    void Start()
    {
        GameObject plane = GameController.gameControllerInstance.FindChildByName(transform.parent,"Plane");
        plane.GetComponent<MeshRenderer>().enabled = false;
        //plane.GetComponent<BoxCollider>().size = new Vector3(landWidth,0.1f,landHeight);
        // GameObject line = GameController.gameControllerInstance.FindChildByName(transform.parent,"linemid");
        // if(line)
        // {
        //     line.transform.localScale = new Vector3(landWidth,0.1f,0.1f);
        //     line.transform.position = new Vector3(0,0.1f/2,0);
        // }
        //transform.parent.localScale = Vector3.one;
        meshFilter = transform.GetComponent<MeshFilter>();
        meshRenderer = transform.GetComponent<MeshRenderer>();
        if(colume < 2)
        {
            colume = 8;
        }
        row = colume*2;
        if(landWidth < 5.0f)
        {
            landWidth = 5.0f;
        }
        landHeight = landWidth*2;
        GenerateLand();
    }

    private void GenerateLand()
    {
        meshFilter.mesh.Clear();
        SetVertices();
        SetUV();
        SetTriangles();
        meshFilter.mesh.RecalculateNormals();
    }

    private void SetVertices()
    {
        vertices = new Vector3[row * colume * 4];
        float dx = (landWidth/colume);
        float dz = (landHeight/row);
        float starX = -landWidth/2;
        float starZ = landHeight/2;
        int index = 0;
        for(int i = 0;i<row;i++)
        {
            starX = -landWidth/2;
            for(int j = 0;j<colume;j++)
            {
                vertices[index].x = starX;
                vertices[index].y = landPosY;
                vertices[index++].z = starZ;

                vertices[index].x = starX+dx;
                vertices[index].y = landPosY;
                vertices[index++].z = starZ;

                vertices[index].x = starX+dx;
                vertices[index].y = landPosY;
                vertices[index++].z = starZ-dz;

                vertices[index].x = starX;
                vertices[index].y = landPosY;
                vertices[index++].z = starZ-dz;

                starX += dx;
            }
            starZ-= dz;
        }
        meshFilter.mesh.vertices = vertices;
    }

    private void SetUV()
    {
        uv = new Vector2[vertices.Length];
        int i;
        for(i = 0;i<vertices.Length/2;i+=4)
        {
            uv[i] = Vector2.zero;
            uv[i + 1] = Vector2.right;
            uv[i + 2] = Vector2.one;
            uv[i + 3] = Vector2.up;
        }
        for(;i<vertices.Length;i+=4)
        {
            uv[i] = Vector2.right;
            uv[i + 1] = Vector2.one;
            uv[i + 2] = Vector2.up;
            uv[i + 3] = Vector2.zero;
        }
        meshFilter.mesh.uv = uv;
    }
    private void SetTriangles()
    {
        triangles = new int[row * colume *6];
        for (int t = 0, i = 0; t < triangles.Length; t += 6, i += 4)
        {
            triangles[t]     = i;
            triangles[t + 1] = i + 1;
            triangles[t + 2] = i + 3;
            triangles[t + 3] = i + 3;
            triangles[t + 4] = i + 1;
            triangles[t + 5] = i + 2;
        }
        meshFilter.mesh.triangles = triangles;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
