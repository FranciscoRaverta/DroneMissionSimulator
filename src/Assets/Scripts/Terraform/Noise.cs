using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int size, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset) {
        float[,] noiseMap = new float[size, size];

        // Le agregamos al sampling del Perlin noise un offset 2D aleatorio
        // fijando la seed y uno arbitrario para desplazar el sampling manualmente.
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++) {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }


        if (scale <= 0) {
            scale = 0.0001f;
        }

        // Para normalizar
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        // Para que al cambiar la escala haga zoom en el medio
        float halfSize = size / 2f;

        for (int x = 0; x < size; x++) {
            for (int z = 0; z < size; z++) {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++) {
                    float sampleX = (x - halfSize + octaveOffsets[i].x) / scale * frequency;
                    float sampleZ = (z - halfSize + octaveOffsets[i].y) / scale * frequency;
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ) * 2 - 1;  // Usamos rango (-1, 1)

                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                maxNoiseHeight = Mathf.Max(maxNoiseHeight, noiseHeight);
                minNoiseHeight = Mathf.Min(minNoiseHeight, noiseHeight);

                noiseMap[x, z] = noiseHeight;
            }
        }

        // Normalizamos
        for (int x = 0; x < size; x++) {
            for (int z = 0; z < size; z++) {
                noiseMap[x, z] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, z]);
            }
        }

        return noiseMap;
    }
}
