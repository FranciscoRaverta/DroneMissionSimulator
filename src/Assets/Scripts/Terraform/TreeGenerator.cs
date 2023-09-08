using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour {
    public static void GenerateTrees(float[,] noiseMap, int size, Tree[] trees) {
        GameObject terrain = GameObject.Find("Terrain");
        GameObject treesContainer = terrain.GetOrCreateGameObjectByName("Trees");
        treesContainer.transform.DestroyImmediateChildren();
        MeshFilter terrainMeshFilter = terrain.GetComponent<MeshFilter>();
        Mesh terrainMesh = terrainMeshFilter.sharedMesh;
        Vector3[] vertices = terrainMesh.vertices;
        Tree tree = trees[0];
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                float currentHeight = noiseMap[i, j];
                if (currentHeight < 0.6f && Random.value < 0.03f) {
                    Vector3 vWorldPos = terrain.transform.TransformPoint(vertices[i * size + j]);
                    GameObject t = Instantiate(tree.mesh, vWorldPos, Quaternion.identity);
                    t.transform.parent = treesContainer.transform;
                }
            }
        }
    }
}

[System.Serializable]
public struct Tree {
    public float maxHeight;
    public float minHeight;
    [Range(0, 1)] public float spawnProbability;
    public GameObject mesh;
}