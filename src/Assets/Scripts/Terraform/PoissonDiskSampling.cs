using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VariablePoissonDiskSampling {
    private float minRadius;
    private float maxRadius;
    private int sampleRejection;
    private int sampleLimit;
    private Vector2 sampleDomain;
    private Texture2D sampleDistribution;
    private SpatialGrid sampleGrid;
    System.Random prng;

    public VariablePoissonDiskSampling(float minRadius, float maxRadius, int sampleRejection, int sampleLimit, Vector2Int sampleDomain, Texture2D sampleDistribution, System.Random prng) {
        this.minRadius = minRadius;
        this.maxRadius = maxRadius;
        this.sampleRejection = sampleRejection;
        this.sampleLimit = sampleLimit;
        this.sampleDomain = sampleDomain;
        this.sampleDistribution = sampleDistribution;
        this.prng = prng;
        this.sampleGrid = new SpatialGrid(sampleDomain.x, sampleDomain.y, minRadius);
    }

    public List<Vector2> Generate() {
        sampleGrid.Clear();

        List<Vector2> points = new List<Vector2>();
        List<Vector2> active = new List<Vector2>();

        Vector2 x0 = GetRandomPointInDomain();
        sampleGrid.AddItem(x0, GetRadiusAt(x0));
        points.Add(x0);
        active.Add(x0);

        while (active.Count > 0) {
            int index = prng.Next(0, active.Count);
            Vector2 center = active[index];
            bool found = false;

            float radius = GetRadiusAt(center);
            for (int i = 0; i < sampleRejection; i++) {
                Vector2 xi = GetRandomPointInAnnulus(prng, center, radius);
                if (sampleGrid.IsValidPos(xi, radius)) {
                    points.Add(xi);
                    active.Add(xi);
                    sampleGrid.AddItem(xi, radius);
                    found = true;
                    break;
                }
            }

            if (!found) {
                active.RemoveAt(index);
            }
        }

        return points;

    }

    Vector2 GetRandomPointInAnnulus(System.Random prng, Vector2 pos, float radius) {
        float angle = (float)prng.NextDouble() * Mathf.PI * 2;
        Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        Vector2 point = pos + dir * radius;
        return point;
    }

    Vector2 GetRandomPointInDomain() {
        return new Vector2(prng.Next(0, (int)sampleDomain.x), prng.Next(0, (int)sampleDomain.y));
    }

    float GetRadiusAt(Vector2 pos) {
        return Mathf.Lerp(minRadius, maxRadius, sampleDistribution.GetPixelBilinear(pos.x / sampleDistribution.width, pos.y / sampleDistribution.height).r);
    }
}


public static class PoissonDiskSampling {
    // Reference paper: https://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph07-poissondisk.pdf
    public static List<Vector2> GeneratePoints(int seed, float radius, Vector2 domain, int k = 30) {
        System.Random prng = new System.Random(seed);
        // Step 0. Initialize data structures.
        float cellSize = radius / Mathf.Sqrt(2);

        int height = Mathf.CeilToInt(domain.x / cellSize);
        int width = Mathf.CeilToInt(domain.y / cellSize);
        Vector2[,] grid = new Vector2[height, width];

        List<Vector2> points = new List<Vector2>();
        List<Vector2> active = new List<Vector2>();

        // Step 1. Select initial sample, x_0, randomly chosen uniformly from the domain.
        Vector2 x0 = new Vector2(prng.Next(0, (int)domain.x), prng.Next(0, (int)domain.y));
        grid[(int)(x0.x / cellSize), (int)(x0.y / cellSize)] = x0;
        points.Add(x0);
        active.Add(x0);

        while (active.Count > 0) {
            int index = prng.Next(0, active.Count);
            Vector2 center = active[index];
            bool found = false;

            for (int i = 0; i < k; i++) {
                // Step 2: Generate up to k points chosen uniformly from the spherical annulus
                // between radius r and 2r around x_i. In this case x_i is the spawn center.
                // Find a suitable candidate or discard the active point as a suitable generator.
                Vector2 xi = GetRandomPointInAnnulus(prng, center, radius);
                if (IsValidPoint(xi, domain, cellSize, radius, grid)) {
                    points.Add(xi);
                    active.Add(xi);
                    grid[(int)(xi.x / cellSize), (int)(xi.y / cellSize)] = xi;
                    found = true;
                    break;
                }
            }

            if (!found) {
                active.RemoveAt(index);
            }
        }

        return points;
    }

    static Vector2 GetRandomPointInAnnulus(System.Random prng, Vector2 center, float radius) {
        float angle = (float)prng.NextDouble() * Mathf.PI * 2;
        Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        Vector2 point = center + dir * Mathf.Lerp(radius, 2*radius, (float)prng.NextDouble());
        return point;
    }

    static bool IsValidPoint(Vector2 candidate, Vector2 domain, float cellSize, float radius, Vector2[,] grid) {
        if (candidate.x < 0 || candidate.x >= domain.x || candidate.y < 0 || candidate.y >= domain.y) {
            return false;
        }

        // We check a 5x5 neighborhood.
        int cellX = (int)(candidate.x / cellSize);
        int cellY = (int)(candidate.y / cellSize);
        int startX = Mathf.Max(0, cellX - 2);
        int endX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
        int startY = Mathf.Max(0, cellY - 2);
        int endY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

        for (int x = startX; x <= endX; x++) {
            for (int y = startY; y <= endY; y++) {
                Vector2 point = grid[x, y];
                if (point != Vector2.zero && (((candidate - point).sqrMagnitude) < (radius * radius))) {
                    // If there's a point and it's inside the radius, then the candidate
                    // is no good.
                    return false;
                }
            }
        }
        return true;
    }
}
