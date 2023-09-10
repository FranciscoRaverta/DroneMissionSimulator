using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fertility : MonoBehaviour
{
    public static Color[] GenerateFertilityColorMap(int size, float[,] vegetationProbabilityMap, FertilityData data) {
        Color[] colorMap = new Color[size * size];

        for (int x = 0; x < size; x++) {
            for (int z = 0; z < size; z++) {
                float sample = data.fertility.Evaluate(vegetationProbabilityMap[x, z]);
                colorMap[x * size + z] = Color.Lerp(Color.white, Color.green, sample);
            }
        }
        return colorMap;
    }

    public static void PlantTrees(GameObject terrain, List<Vector2> points, GameObject tree) {

        GameObject treesContainer = terrain.GetOrCreateGameObjectByName("Trees");
        treesContainer.transform.DestroyImmediateChildren();
        MeshFilter terrainMeshFilter = terrain.GetComponent<MeshFilter>();
        Mesh terrainMesh = terrainMeshFilter.sharedMesh;
        Vector3[] vertices = terrainMesh.vertices;
        Debug.Log(terrain.transform.TransformPoint(Vector3.zero));

        foreach (Vector2 point in points) {
            Vector3 localPos = new Vector3(point.x, 0, point.y);
            Vector3 worldPos = terrain.transform.TransformPoint(localPos);
            GameObject t = Instantiate(tree, worldPos, Quaternion.identity);
            t.transform.parent = treesContainer.transform;
        }
    }
}
