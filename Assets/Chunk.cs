using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Chunk : MonoBehaviour
{
    private Voxel[,,] chunk = new Voxel[5, 5, 5];


    
    // Start is called before the first frame update
    void Start()
    {
        int r = 0;
        for (int x = 0; x < chunk.GetLength(0); x++)
        {
            for (int y = 0; y < chunk.GetLength(1); y++)
            {
                for (int z = 0; z < chunk.GetLength(2); z++)
                {
                    chunk[x, y, z] = new Voxel();
                    if (Random.Range(0,2) == 1 && false)
                    {
                        chunk[x, y, z].type = 0;
                    }

                    r++;

                }
            }
        }
                                
        int[][] faces = new int[][]
        {
            new int[] { 2, 1, 0, 2, 3, 1 }, // Back face
            new int[] { 4, 5, 6, 5, 7, 6 }, // Front face
            new int[] { 0, 1, 4, 1, 5, 4 }, // Bottom face
            new int[] { 6, 3, 2, 6, 7, 3 }, // Top face
            new int[] { 4, 2, 0, 4, 6, 2 }, // Left face
            new int[] { 1, 3, 5, 3, 7, 5 }  // Right face
        };
        Dictionary<int[], List<Vector3[]>> sharedFaces = new Dictionary<int[], List<Vector3[]>>();
        for (int x = 0; x < chunk.GetLength(0); x++)
        {
            for (int y = 0; y < chunk.GetLength(1); y++)
            {
                for (int z = 0; z < chunk.GetLength(2); z++)
                {
                    Vector3[] vertices = new Vector3[]
                    {
                        new Vector3(-1 + x * 2, -1 + y * 2, -1 + z * 2), // 0: Bottom-left-back
                        new Vector3(1 + x * 2, -1 + y * 2, -1 + z * 2), // 1: Bottom-right-back
                        new Vector3(-1 + x * 2, 1 + y * 2, -1 + z * 2), // 2: Top-left-back
                        new Vector3(1 + x * 2, 1 + y * 2, -1 + z * 2), // 3: Top-right-back
                        new Vector3(-1 + x * 2, -1 + y * 2, 1 + z * 2), // 4: Bottom-left-front
                        new Vector3(1 + x * 2, -1 + y * 2, 1 + z * 2), // 5: Bottom-right-front
                        new Vector3(-1 + x * 2, 1 + y * 2, 1 + z * 2), // 6: Top-left-front
                        new Vector3(1 + x * 2, 1 + y * 2, 1 + z * 2) // 7: Top-right-front
                    };
                    if (chunk[x, y, z].type == 1)
                    {
                        int[] dz = { -1, 1, 0, 0, 0, 0 };
                        int[] dy = { 0, 0, -1, 1, 0, 0 };
                        int[] dx = { 0, 0, 0, 0, -1, 1 };
                        List<int[]> facesToDraw = new List<int[]>();
                        int index = 0;

                        for (int i = 0; i < 6; i++)
                        {
                            int nz = z + dz[i];
                            int ny = y + dy[i];
                            int nx = x + dx[i];

                            try
                            {
                                if (chunk[nx, ny, nz].type == 0)
                                {
                                    facesToDraw.Add(faces[index]);
                                    
                                    
                                    
                                    //print(sharedFaces.Count);
                                       
                                }
                            }
                            catch (Exception e)
                            {
                                facesToDraw.Add(faces[index]);
                                
                                if (sharedFaces.ContainsKey(faces[index]))
                                {
                                    sharedFaces[faces[index]].Add(vertices); 
                                }
                                else
                                {
                                    sharedFaces.Add(faces[index],new List<Vector3[]>());    
                                }
                                
                            }
                            index++;
                        }



                        int[][] facesArray = facesToDraw.ToArray();
                        chunk[x, y, z].verts = vertices;
                        chunk[x, y, z].faces = facesArray;

                        // Define the faces of the cube (each face consists of two triangles)

                        createCube(facesArray, vertices);
                    }


                }
            }
        }
        print(sharedFaces.Keys.Count);
        foreach (var face in sharedFaces.Keys)
        {
            List<Vector3> verts = new List<Vector3>();
            
            for (int i = 0; i < sharedFaces[face].Count; i++)
            {
                
            }
        }
        
    }
    
    static bool AreCubesAdjacent(Vector3[] verts1, Vector3[] verts2)
    {
        if (verts1.Length != 8 || verts2.Length != 8)
        {
            throw new ArgumentException("Both arrays must represent 8 vertices for cubes.");
        }

        // Find how many vertices are shared
        int sharedVertices = 0;
        
        foreach (var v1 in verts1)
        {
            foreach (var v2 in verts2)
            {
                if (v1 == v2)
                {
                    sharedVertices++;
                }
            }
        }

        // If they share exactly 4 vertices, they are adjacent
        return sharedVertices == 4;
    }

    void createCube(int[][] faces,Vector3[] vertices)
    {
        // Create a new mesh
        Mesh mesh = new Mesh();
        mesh.name = "Cube";

        // Set the vertices
        mesh.vertices = vertices;

        // Combine all faces' triangles into one array
        int[] triangles = new int[faces.Length * 6]; // Each face has 6 indices (2 triangles * 3 indices per triangle)
        int index = 0;
        foreach (int[] face in faces)
        {
            foreach (int vertexIndex in face)
            {
                triangles[index++] = vertexIndex;
            }
        }
        mesh.triangles = triangles;
        
        // Optionally set normals
        mesh.RecalculateNormals();
        GameObject cube = new GameObject();
        //cube.AddComponent<Transform>();
        // Assign the mesh to the MeshFilter component
        cube.AddComponent<MeshFilter>();
        MeshFilter meshFilter =  cube.GetComponent<MeshFilter>();
        cube.AddComponent<MeshRenderer>();
        meshFilter.mesh = mesh;
    }

    bool compareTriangles(Vector3 tri1, Vector3 tri2)
    {
        return (((tri1.x == tri2.z && tri1.z == tri2.x) || (tri1.x == tri2.x && tri1.z == tri2.z)) && tri1.y == tri2.y);
    }

    // Update is called once per frame
    void Update()
    {
    }

   
}