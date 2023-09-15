using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terraformer : MonoBehaviour
{
    public enum Preview { None, Height, Vegetation, Fertility }
    public Preview preview = Preview.None;

    // General properties
    public Material terrainMaterial;
    public int size = 256;
    public Preview previewMode;
    public bool autoUpdate;

    // Land properties
    public NoiseData terrainNoiseData;
    public CustomTerrainData terrainData;

    // Vegetation properties
    public NoiseData noiseDataFertility;
    public FertilityData fertilityData;
    public TreeData treeData;

    // State
    [System.NonSerialized] private bool initialized = false;

    // Private properties
    private CustomTerrain customTerrain;
    private Fertility fertility;
    [HideInInspector] public bool noiseDataLandFoldout;
    [HideInInspector] public bool noiseDataFertilityFoldout;
    [HideInInspector] public bool fertilityDataFoldout;
    [HideInInspector] public bool terrainDataFoldout;

    public void Initialize() {
        if (!initialized) {
            Debug.Log("Initializing");
            customTerrain = new CustomTerrain(size, terrainMaterial, terrainData, terrainNoiseData);
            fertility = new Fertility(size, noiseDataFertility, fertilityData);
            initialized = true;
        }

    }

    public void GenerateTrees() {
        if (initialized) {
            fertility.SetData(noiseDataFertility);
            fertility.SetData(fertilityData);
            Vector2 domain = new Vector2(size, size);
            List<Vector2> spawnPoints = PoissonDiskSampling.GeneratePoints(10, domain);
            fertility.PlantTrees(customTerrain, spawnPoints);
        }

    }

    public void GenerateTerrain() {
        if (initialized) {
            customTerrain.SetData(terrainNoiseData);
            customTerrain.SetData(terrainData);
            customTerrain.UpdateTerrain();
        }
    }

    public void OnNoiseDataLandUpdated() {
        if (autoUpdate) {
            GenerateTerrain();
        }
    }
    public void OnNoiseDataFertilityUpdated() {
        if (autoUpdate && initialized) {
            fertility.SetData(noiseDataFertility);
            fertility.SetData(fertilityData);
            DrawPreview();
        }
    }
    public void OnFertilityUpdated() { 
        if (autoUpdate && initialized) {
            fertility.SetData(noiseDataFertility);
            fertility.SetData(fertilityData);
            DrawPreview();
        }
    }

    public void DrawPreview() {
        if (initialized) {
            if (previewMode == Preview.Height) {
                terrainMaterial.mainTexture = TextureGenerator.TextureFromHeightMap(customTerrain.TerrainHeightMap);
            } else if (previewMode == Preview.Vegetation) {
                terrainMaterial.mainTexture = TextureGenerator.TextureFromHeightMap(fertility.ProbabilityMap);
            } else if (previewMode == Preview.Fertility) {
                terrainMaterial.mainTexture = TextureGenerator.TextureFromColorMap(fertility.GenerateFertilityColorMap(), size, size);
            } else {
                terrainMaterial.mainTexture = null;
            }
        }
    }

    private void OnValidate() {
        if (autoUpdate) {
            DrawPreview();
        }
    }
}
