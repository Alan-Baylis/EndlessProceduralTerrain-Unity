using UnityEngine;
using System.Collections;

public static class Noise
{
    //Generate a 2D array of floats using Perlin Noise
    public static float[,] GenerateNoiseMap(int seed, int width, int height, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        //Initialize
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffset = new Vector2[octaves];
        float[,] noiseMap = new float[width, height];
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;
        float halfWidth = width / 2.0f;
        float halfHeight = height / 2.0f;

        //Verify the scale is larger than 0
        if(scale <= 0)
        {
            scale = 0.0001f;
        }

        //Generate octave offsets
        for(int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offSety = prng.Next(-100000, 100000) + offset.y;

            octaveOffset[i] = new Vector2(offsetX, offSety);
        }
        
        //Loop through each coordinate and calculate a noise value
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                float amplitude = 1.0f;
                float frequency = 1.0f;
                float noiseHeight = 0.0f;

                //Loop through and apply each octave
                for(int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffset[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffset[i].y;

                    float perlinVal = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinVal * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                //Track the lowest and highest values for normalization
                if(noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if(noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                //Apply the calculated value
                noiseMap[x, y] = noiseHeight;
            }
        }

        //Loop through each value and normalize it between 0 and 1
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}
