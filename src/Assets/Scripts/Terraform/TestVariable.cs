using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestVariable : MonoBehaviour
{
    public NoiseData noiseData;
    public List<Vector2> samplePoints;
    private void Start()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(256, noiseData.seed, noiseData.scale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, noiseData.offset);
        Texture2D texture = TextureGenerator.TextureFromHeightMap(noiseMap);
        System.Random prng = new System.Random(noiseData.seed);
        VariablePoissonDiskSampling sampler = new VariablePoissonDiskSampling(5, 10, 30, 10000, new Vector2Int(256, 256), texture, prng);
        samplePoints = sampler.Generate();
    }

    private void OnDrawGizmos() {
        if (samplePoints != null) {
            foreach (Vector2 p in samplePoints) {
                Gizmos.DrawSphere(new Vector3(p.x, 0, p.y), 2);
            }
        }
    }
}
