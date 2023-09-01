using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColorMap, Mesh };
    public DrawMode drawMode;

    [MinAttribute(1)] public int mapWidth;
    [MinAttribute(1)] public int mapHeight;
    public float noiseScale;

    [MinAttribute(0)] public int octaves;
    [Range(0, 1)] public float persistance;
    [MinAttribute(1)] public float lacunarity;
    [SerializeField] public float heightMultiplicationFactor;
    [SerializeField] public AnimationCurve heightDampeningFactor;

    public int seed;
    public Vector2 offset;

    public bool autoUpdate;

    public TerrainType[] regions;

    public void GenerateMap() {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
        Color[] colorMap = new Color[mapWidth * mapHeight];

        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                float currentHeight = noiseMap[x, y];
                foreach (TerrainType t in regions) {
                    if (currentHeight <= t.height) {
                        colorMap[y * mapWidth + x] = t.color;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();

        if (drawMode == DrawMode.NoiseMap) {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        } else if (drawMode == DrawMode.ColorMap) {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        } else if (drawMode == DrawMode.Mesh) {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, heightMultiplicationFactor, heightDampeningFactor), TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        }
    }
}

[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public Color color;
}