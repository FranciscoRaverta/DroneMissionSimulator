using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fertility : MonoBehaviour
{
    private int size;
    private NoiseData noiseData;
    private FertilityData fertilityData;
    private float[,] probabilityMap;

    // Getters
    public float[,] ProbabilityMap { get => probabilityMap; }

    public Fertility(int size, NoiseData noiseData, FertilityData fertilityData) {
        this.size = size;
        this.noiseData = noiseData;
        this.fertilityData = fertilityData;
        this.probabilityMap = GenerateProbabilityMap();
    }

    public void SetData(NoiseData noiseData) {
        this.noiseData = noiseData;
        this.probabilityMap = GenerateProbabilityMap();
    }
    public void SetData(FertilityData fertilityData) {
        this.fertilityData = fertilityData;
        this.probabilityMap = GenerateProbabilityMap();
    }

    public Color[] GenerateFertilityColorMap() {
        Color[] colorMap = new Color[size * size];
        for (int x = 0; x < size; x++) {
            for (int z = 0; z < size; z++) {
                float sample = fertilityData.fertility.Evaluate(probabilityMap[x, z]);
                colorMap[z * size + x] = Color.Lerp(Color.red, Color.green, sample);
            }
        }
        return colorMap;
    }

    private float[,] GenerateProbabilityMap() {
        return Noise.GenerateNoiseMap(size, noiseData.seed, noiseData.scale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, noiseData.offset);
    }

    public void PlantTrees(CustomTerrain customTerrain, List<Vector2> points) {
        probabilityMap = GenerateProbabilityMap();
        GameObject treesContainer = customTerrain.TerrainObject.GetOrCreateGameObjectByName("Trees");
        treesContainer.transform.DestroyImmediateChildren();
        MeshFilter terrainMeshFilter = customTerrain.TerrainObject.GetComponent<MeshFilter>();
        Mesh terrainMesh = terrainMeshFilter.sharedMesh;
        Vector3 offset = new Vector3(-terrainMesh.bounds.size.x, 10, -terrainMesh.bounds.size.z) / 2;
        Vector3[] vertices = terrainMesh.vertices;

        foreach (Vector2 point in points) {
            if (Random.value < fertilityData.fertility.Evaluate(probabilityMap[(int)point.x, (int)point.y])) {
                Vector3 localPos = new Vector3(point.x, 0, point.y);
                Vector3 worldPos = customTerrain.TerrainObject.transform.TransformPoint(localPos) + offset;
                GameObject t = Instantiate(fertilityData.tree, worldPos, Quaternion.identity);
                t.transform.RotateAround(t.transform.position, t.transform.up, Mathf.Rad2Deg * Random.value * 2 * Mathf.PI);
                t.transform.parent = treesContainer.transform;
            }
        }
    }
}
