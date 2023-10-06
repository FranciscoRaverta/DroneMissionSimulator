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

    public List<Vector2> SamplePointsInDomain(float minRadius, float maxRadius, System.Random prng, int limit = 10000) {
        Texture2D tex = TextureGenerator.TextureFromHeightMap(probabilityMap);
        Vector2Int domain = new Vector2Int(size, size);
        VariablePoissonDiskSampling sampler = new VariablePoissonDiskSampling(minRadius, maxRadius, domain, tex, prng, 30, limit);
        return sampler.Generate();
        //return PoissonDiskSampling.GeneratePoints(fertilityData.seed, 10, domain);
    }

    public List<Vector3> ConvertSampleToWorldSpace(List<Vector2> samplePoints, CustomTerrain customTerrain) {
        return samplePoints.ConvertAll<Vector3>(point => customTerrain.SurfacePointToWorldPos(point));
    }

    public void PlantTrees(CustomTerrain customTerrain) {
        UpdateFertility();
        System.Random prng = new System.Random(fertilityData.seed);

        // Destroy previous trees
        GameObject treesContainer = customTerrain.TerrainObject.GetOrCreateGameObjectByName("Trees");
        treesContainer.transform.DestroyImmediateChildren();
        List<Vector2> samplePoints = SamplePointsInDomain(5, 30, prng);
        List<Vector3> spawnPoints = ConvertSampleToWorldSpace(samplePoints, customTerrain);

        // Spawn new trees according to fertility map
        for (int i = 0; i < samplePoints.Count; i++) {
            Vector3 spawnPoint = spawnPoints[i];
            float treeSample = (float)prng.NextDouble();
            GameObject t = Instantiate(fertilityData.treeData.Evaluate(treeSample), spawnPoint, Quaternion.identity);
            t.transform.RotateAround(t.transform.position, t.transform.up, Mathf.Rad2Deg * (float)prng.NextDouble() * 2 * Mathf.PI);
            t.transform.parent = treesContainer.transform;
        }
    }

    public void PlantGrass(CustomTerrain customTerrain) {
        UpdateFertility();
        System.Random prng = new System.Random(fertilityData.seed);

        GameObject grassContainer = customTerrain.TerrainObject.GetOrCreateGameObjectByName("Grass");
        grassContainer.transform.DestroyImmediateChildren();
        List<Vector2> samplePoints = SamplePointsInDomain(.2f, 1, prng, 25000);
        List<Vector3> spawnPoints = ConvertSampleToWorldSpace(samplePoints, customTerrain);

        for (int i = 0; i < samplePoints.Count; i++) {
            Vector3 spawnPoint = spawnPoints[i];
            GameObject t = Instantiate(fertilityData.treeData.grass, spawnPoint, Quaternion.identity);
            t.transform.RotateAround(t.transform.position, t.transform.up, Mathf.Rad2Deg * (float)prng.NextDouble() * 2 * Mathf.PI);
            t.transform.parent = grassContainer.transform;
        }
    }
}
