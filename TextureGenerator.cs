using UnityEngine;
using System.Collections;

public static class TextureGenerator 
{
    //Generate a 2D texture using an array of colors
    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
    {
        //Initialize the texture
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();

        return texture;
    }

    //Generate a 2D texture using a 2D array of float values
    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        //Initialize the texture and color map
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        Color[] colorMap = new Color[width * height];

        //Loop through the heightmap and save them to a color map as B&W values
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        //Generate a texture using a B&W color map
        return TextureFromColorMap(colorMap, width, height);
    }
}
