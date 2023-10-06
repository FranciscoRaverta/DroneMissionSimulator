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
            customTerrain = new CustomTerrain(size, terrainMaterial, terrainData, terrainNoiseData);
            fertility = new Fertility(size, noiseDataFertility, fertilityData);
            customTerrain.UpdateTerrain();
            fertility.UpdateFertility();
            initialized = true;
        }
    }

    public void GenerateTrees() {
        if (initialized) {
            fertility.SetData(noiseDataFertility);
            fertility.SetData(fertilityData);
            fertility.PlantTrees(customTerrain);
            fertility.PlantGrass(customTerrain);
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
        if (autoUpdate) {
            fertility.SetData(noiseDataFertility);
            fertility.SetData(fertilityData);
            fertility.UpdateFertility();
            DrawPreview();
        }
    }
    public void OnFertilityUpdated() { 
        if (autoUpdate) {
            fertility.SetData(noiseDataFertility);
            fertility.SetData(fertilityData);
            fertility.UpdateFertility();
            DrawPreview();
        }
    }

    public void DrawPreview() {
        if (initialized) {
            customTerrain.UpdateTerrain();
            fertility.UpdateFertility();
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
