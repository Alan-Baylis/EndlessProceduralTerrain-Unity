using UnityEngine;
using System.Collections;

public static class MeshGenerator 
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail)
    {
        //Initialize
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        AnimationCurve localHeightCurve = new AnimationCurve(heightCurve.keys); //Save local copy of Curve for threading
        int meshSimplificationIncrement = (levelOfDetail == 0)?1:levelOfDetail * 2;
        int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;
        float topLeftX = (width - 1) / -2.0f;
        float topLeftZ = (height - 1) / 2.0f;
        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
        int vertexIndex = 0;

        //Loop through the height map and create the vertices, UVs, and triangles
        for(int y = 0; y < height; y += meshSimplificationIncrement)
        {
            for(int x = 0; x < width; x += meshSimplificationIncrement)
            {
                //Create the vertex and UV at the current point
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, localHeightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                //Create triangles if we aren't on the far right or bottom edge
                if(x < (width - 1) && y < (height - 1))
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }
                
                vertexIndex++;
            }
        }

        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    int triangleIndex;
    public Vector2[] uvs;

    //Initialize vertices, triangles, and UVs for mesh
    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth* meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        uvs = new Vector2[meshWidth * meshHeight];
    }

    //Add a triangle to the triangle array
    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    //Generate and return a mesh using the created vertices, triangles, and UVs
    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }
}
