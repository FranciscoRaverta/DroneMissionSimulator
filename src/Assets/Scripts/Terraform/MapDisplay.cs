using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    [SerializeField] GameObject terrain;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Material material;

    private void Create() {
        GameObject go = new GameObject("Terrain");
        go.transform.position = Vector3.zero;

        meshFilter = go.AddComponent<MeshFilter>();
        meshRenderer = go.AddComponent<MeshRenderer>();
        if (!material) {
            material = new Material(Shader.Find("Standard"));
        }
        meshRenderer.sharedMaterial = material;
        terrain = go;
    }

    public void Draw(MeshData meshData, Texture2D texture) {
        if (!terrain) {
            Create();
        }
        DrawMesh(meshData, texture);
    }

    public void DrawTexture(Texture2D texture) {
        meshRenderer.sharedMaterial.mainTexture = texture;
        meshRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture) {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
