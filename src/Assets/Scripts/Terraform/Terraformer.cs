using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terraformer : MonoBehaviour
{
    public enum Preview { None, Height, Vegetation, Fertility }
    public Preview preview = Preview.None;

    // General properties
    public GameObject terrain;
    public Material terrainMaterial;
    public int size = 256;
    public Preview previewMode;
    public bool autoUpdate;

    // Land properties
    public NoiseData noiseDataLand;
    public float heightMultiplier;
    public AnimationCurve heightCurve = AnimationCurve.Constant(0, 1, 1);

    // Vegetation properties
    public NoiseData noiseDataFertility;
    public FertilityData fertilityData;
    public TreeData treeData;

    // Private properties
    private float[,] heightMap;
    private float[,] vegetationProbabilityMap;
    [HideInInspector] public bool noiseDataLandFoldout;
    [HideInInspector] public bool noiseDataFertilityFoldout;
    [HideInInspector] public bool fertilityDataFoldout;

    // Debug gizmos
    private List<Vector2> spawnPointsGizmos;

    public void GenerateLand() {
        if (terrain == null) {
            terrain = MeshGenerator.Create(terrainMaterial);
        }
        MeshFilter mf = terrain.GetComponent<MeshFilter>();
        heightMap = Noise.GenerateNoiseMap(size, noiseDataLand.seed, noiseDataLand.scale, noiseDataLand.octaves, noiseDataLand.persistance, noiseDataLand.lacunarity, noiseDataLand.offset);
        mf.sharedMesh = MeshGenerator.GenerateTerrainMesh(heightMap, heightMultiplier, heightCurve).CreateMesh();
    }

    public void GenerateTrees() {
        vegetationProbabilityMap = Noise.GenerateNoiseMap(size, noiseDataFertility.seed, noiseDataFertility.scale, noiseDataFertility.octaves, noiseDataFertility.persistance, noiseDataFertility.lacunarity, noiseDataFertility.offset);
        Vector2 domain = new Vector2(size, size);
        List<Vector2> spawnPoints = PoissonDiskSampling.GeneratePoints(50, domain);
        spawnPointsGizmos = spawnPoints;
        Fertility.PlantTrees(terrain, spawnPoints, fertilityData.tree);
    }

    public void OnNoiseDataLandUpdated() {
        if (autoUpdate) {
            GenerateLand();
        }
    }
    public void OnNoiseDataFertilityUpdated() {
        if (autoUpdate) {
            GenerateTrees();
            DrawPreview();
        }
    }
    public void OnFertilityUpdated() { 
        if (autoUpdate) {
            DrawPreview();
        }
    }

    public void DrawPreview() {
        if (previewMode == Preview.Height) {
            terrainMaterial.mainTexture = TextureGenerator.TextureFromHeightMap(heightMap);
        } else if (previewMode == Preview.Vegetation) {
            terrainMaterial.mainTexture = TextureGenerator.TextureFromHeightMap(vegetationProbabilityMap);
        } else if (previewMode == Preview.Fertility) {
            terrainMaterial.mainTexture = TextureGenerator.TextureFromColorMap(Fertility.GenerateFertilityColorMap(size, vegetationProbabilityMap, fertilityData), size, size);
        } else { 
            terrainMaterial.mainTexture = null;
        }
    }

    private void OnValidate() {
        if (autoUpdate) {
            GenerateLand();
            GenerateTrees();
            DrawPreview();
        }
    }

    private void OnDrawGizmos() {
        foreach (Vector2 p in spawnPointsGizmos) {
            Vector3 worldPos = new Vector3(p.x, 5, p.y);
            Gizmos.DrawSphere(worldPos, 2);
            Gizmos.DrawWireSphere(worldPos, 50);
        }
    }
}
