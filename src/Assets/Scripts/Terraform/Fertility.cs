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
    }
    public void SetData(FertilityData fertilityData) {
        this.fertilityData = fertilityData;
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

    public void UpdateFertility() {
        this.probabilityMap = GenerateProbabilityMap();
    }

    private float[,] GenerateProbabilityMap() {
        return Noise.GenerateNoiseMap(size, noiseData.seed, noiseData.scale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, noiseData.offset);
    }

    public List<Vector2> SamplePointsInDomain() {
        Vector2 domain = new Vector2(size, size);
        return PoissonDiskSampling.GeneratePoints(fertilityData.seed, 10, domain);
    }

    public List<Vector3> GenerateTreeDistribution(List<Vector2> samplePoints, CustomTerrain customTerrain) {
        return samplePoints.ConvertAll<Vector3>(point => customTerrain.SurfacePointToWorldPos(point));
    }

    public void PlantTrees(CustomTerrain customTerrain) {
        UpdateFertility();

        // Destroy previous trees
        GameObject treesContainer = customTerrain.TerrainObject.GetOrCreateGameObjectByName("Trees");
        treesContainer.transform.DestroyImmediateChildren();
        MeshFilter terrainMeshFilter = customTerrain.TerrainObject.GetComponent<MeshFilter>();
        List<Vector2> samplePoints = SamplePointsInDomain();
        List<Vector3> spawnPoints = GenerateTreeDistribution(samplePoints, customTerrain);

        // Spawn new trees according to fertility map
        System.Random prng = new System.Random(fertilityData.seed);
        for (int i = 0; i < samplePoints.Count; i++) {
            Vector2 samplePoint = samplePoints[i];
            Vector3 spawnPoint = spawnPoints[i];
            if ((float)prng.NextDouble() < fertilityData.fertility.Evaluate(probabilityMap[(int)samplePoint.x, (int)samplePoint.y])) {
                float treeSample = (float)prng.NextDouble();
                GameObject t = Instantiate(fertilityData.treeData.Evaluate(treeSample), spawnPoint, Quaternion.identity);
                t.transform.RotateAround(t.transform.position, t.transform.up, Mathf.Rad2Deg * (float)prng.NextDouble() * 2 * Mathf.PI);
                t.transform.parent = treesContainer.transform;
            }
        }
    }
}
