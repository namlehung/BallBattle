using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIndicator : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] vertices;
    private Vector2[] uv;
    private int[] triangles;
    private Transform transformchild;
    // Start is called before the first frame update
    void Start()
    {
        GameObject child = new GameObject("indicator");
        //child.transform.parent = transform;
        //MeshFilter meshFilter = child.AddComponent<MeshFilter>() as MeshFilter;
        //mesh  = meshFilter.mesh;
        //mesh.Clear();


        mesh = new Mesh();
        
        SetVertices();
        SetUV();
        SetTriangles();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
       
        transformchild = child.transform;
        transformchild.position = Vector3.zero;
        //transformchild.rotation = Quaternion.identity;
        transformchild.localScale = Vector3.one*0.2f;
        transformchild.parent = transform;
        transformchild.localPosition = new Vector3(0,0.3f,0.3f);

        MeshFilter mf = transformchild.gameObject.AddComponent<MeshFilter>() as MeshFilter;
        mf.mesh = mesh;
        MeshRenderer mr = transformchild.gameObject.AddComponent<MeshRenderer>() as MeshRenderer;
        mr.material.color = Color.red;
    }

private void SetVertices()
    {
        vertices = new Vector3[3];// + 4 + 3 + 4 + 4];
        vertices[0] = new Vector3(0,0.5f,0.5f);
        vertices[1] = new Vector3(0.5f,0.5f,-0.5f);
        vertices[2] = new Vector3(-0.5f,0.5f,-0.5f);

        // vertices[3] = new Vector3(0,0.5f,0.5f);
        // vertices[4] = new Vector3(0,-0.5f,0.5f);
        // vertices[5] = new Vector3(0.5f,-0.5f,-0.5f);
        // vertices[6] = new Vector3(0.5f,0.5f,-0.5f);

        // vertices[7] = new Vector3(0,-0.5f,0.5f);
        // vertices[8] = new Vector3(0.5f,-0.5f,-0.5f);
        // vertices[9] = new Vector3(-0.5f,-0.5f,-0.5f);

        // vertices[10] = new Vector3(0,-0.5f,0.5f);
        // vertices[11] = new Vector3(-0.5f,-0.5f,-0.5f);
        // vertices[12] = new Vector3(-0.5f,0.5f,-0.5f);
        // vertices[13] = new Vector3(0,0.5f,0.5f);

        // vertices[14] = new Vector3(-0.5f,-0.5f,-0.5f);
        // vertices[15] = new Vector3(-0.5f,0.5f,-0.5f);
        // vertices[16] = new Vector3(0.5f,0.5f,-0.5f);
        // vertices[17] = new Vector3(0.5f,-0.5f,-0.5f);

        mesh.vertices = vertices;
    }

    private void SetUV()
    {
        uv = new Vector2[vertices.Length];
        int i =0;
        uv[i] = Vector2.zero;
        uv[i + 1] = Vector2.right;
        uv[i + 2] = Vector2.one;
        // for(i = 0;i<vertices.Length;i+=4)
        // {
        //     uv[i] = Vector2.zero;
        //     uv[i + 1] = Vector2.right;
        //     uv[i + 2] = Vector2.one;
        //     uv[i + 3] = Vector2.up;
        // }
        mesh.uv = uv;
    }
    private void SetTriangles()
    {
        triangles = new int[3];//+6+3+6+6];
        int i =0;
        triangles[i++] = 0;
        triangles[i++] = 1;
        triangles[i++] = 2;

        // triangles[i++] = 3;
        // triangles[i++] = 4;
        // triangles[i++] = 5;
        // triangles[i++] = 5;
        // triangles[i++] = 6;
        // triangles[i++] = 3;

        // triangles[i++] = 7;
        // triangles[i++] = 8;
        // triangles[i++] = 9;

        // triangles[i++] = 10;
        // triangles[i++] = 12;
        // triangles[i++] = 11;
        // triangles[i++] = 10;
        // triangles[i++] = 13;
        // triangles[i++] = 12;

        // triangles[i++] = 14;
        // triangles[i++] = 15;
        // triangles[i++] = 16;
        // triangles[i++] = 14;
        // triangles[i++] = 16;
        // triangles[i++] = 17;

        mesh.triangles = triangles;
    }

    // Update is called once per frame
    void Update()
    {
        //transformchild.position = Vector3.zero;
    }
}
