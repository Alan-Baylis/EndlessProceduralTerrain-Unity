using UnityEngine;
using System.Collections;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer; //Plane to display the texture on
    public MeshFilter meshFilter; //Mesh Filter of Plane
    public MeshRenderer meshRenderer; //Mesh Renderer of Plane

    public void DrawTexture(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1.0f, texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
