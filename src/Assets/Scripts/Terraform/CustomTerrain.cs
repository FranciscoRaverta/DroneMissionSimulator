using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTerrain : MonoBehaviour {
    const string NAME = "Custom Terrain";
    const string TREE_CONTAINER = "Trees";

    int size;
    private GameObject terrainGameObject;
    private Material material;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    private Mesh mesh;
    private NoiseData noiseData;
    private CustomTerrainData terrainData;
    private float[,] heightMap;

    // Getters
    public GameObject TerrainObject { get => terrainGameObject; }
    public float[,] TerrainHeightMap { get => heightMap; }

    public CustomTerrain(int size, Material terrainMaterial, CustomTerrainData terrainData, NoiseData noiseData) {
        this.material = terrainMaterial;
        this.noiseData = noiseData;
        this.size = size;
        this.terrainData = terrainData;
        InitializeTerrainObject();
    }

    private void InitializeTerrainObject() {
        terrainGameObject = GameObject.Find(NAME);
        if (terrainGameObject == null) {
            terrainGameObject = CreateTerrainObject(material);
        } else {
            meshFilter = terrainGameObject.GetComponent<MeshFilter>();
            meshRenderer = terrainGameObject.GetComponent<MeshRenderer>();
            meshCollider = terrainGameObject.GetComponent<MeshCollider>();
        }
    }

    private GameObject CreateTerrainObject(Material terrainMaterial) {
        GameObject go = new GameObject(NAME);
        go.transform.position = Vector3.zero;

        meshFilter = go.AddComponent<MeshFilter>();
        meshRenderer = go.AddComponent<MeshRenderer>();
        meshCollider = go.AddComponent<MeshCollider>();
        meshRenderer.sharedMaterial = terrainMaterial;

        int layer = LayerMask.NameToLayer("Terrain");
        go.layer = layer;
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

    public Vector3 SurfacePointToWorldPos(Vector2 point) {
        float maxHeight = mesh.bounds.max.y + 10;
        float dst = Mathf.Abs(mesh.bounds.max.y - mesh.bounds.min.y) + 20;
        Vector3 offset = new Vector3(-mesh.bounds.size.x, 0, -mesh.bounds.size.z) * 0.5f;
        Vector3 projectPoint = new Vector3(point.x, maxHeight, point.y) + offset;
        RaycastHit hit;
        int mask = 1 << LayerMask.NameToLayer("Terrain");

        if (Physics.Raycast(projectPoint, Vector3.down, out hit, dst, mask)) {
            return hit.point;
        }

        return hit.point;
    }

    public void UpdateTerrain() {
        if (terrainGameObject == null) {
            InitializeTerrainObject();
        }
        heightMap = GenerateHeightMap();
        mesh = GenerateMesh();
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }
}
