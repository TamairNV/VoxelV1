using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel
{
    public int type = 1;
    public Vector3[] verts;
    public int[][] faces;

}

public class Face
{
    public Vector3[] face = new Vector3[2];
}
