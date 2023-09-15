using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTerrain : MonoBehaviour {
    const string NAME = "Custom Terrain";

    int size;
    private GameObject terrainGameObject;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;
    private NoiseData noiseData;
    private CustomTerrainData terrainData;
    private float[,] heightMap;

    // Getters
    public GameObject TerrainObject { get => terrainGameObject; }
    public float[,] TerrainHeightMap { get => heightMap; }

    public CustomTerrain(int size, Material terrainMaterial, CustomTerrainData terrainData, NoiseData noiseData) {
        InitializeTerrainObject(terrainMaterial);
        this.noiseData = noiseData;
        this.size = size;
        this.terrainData = terrainData;
    }

    private void InitializeTerrainObject(Material terrainMaterial) {
        terrainGameObject = GameObject.Find(NAME);
        if (terrainGameObject == null) {
            terrainGameObject = CreateTerrainObject(terrainMaterial);
        } else {
            meshFilter = terrainGameObject.GetComponent<MeshFilter>();
            meshRenderer = terrainGameObject.GetComponent<MeshRenderer>();
        }
    }

    private GameObject CreateTerrainObject(Material terrainMaterial) {
        GameObject go = new GameObject(NAME);
        go.transform.position = Vector3.zero;

        meshFilter = go.AddComponent<MeshFilter>();
        meshRenderer = go.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = terrainMaterial;
        return go;
    }

    public void SetData(NoiseData noiseData) {
        this.noiseData = noiseData;
    }

    public void SetData(CustomTerrainData terrainData) {
        this.terrainData = terrainData;
    }

    private float[,] GenerateHeightMap() {
        return Noise.GenerateNoiseMap(size, noiseData.seed, noiseData.scale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, noiseData.offset);
    }

    private Mesh GenerateMesh() {
        return MeshGenerator.GenerateTerrainMesh(heightMap, terrainData.heightMultiplier, terrainData.heightCurve).CreateMesh();
    }


    public void UpdateTerrain() {
        heightMap = GenerateHeightMap();
        mesh = GenerateMesh();
        meshFilter.sharedMesh = mesh;
    }
}
