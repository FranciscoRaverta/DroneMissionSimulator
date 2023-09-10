using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PoissonDiskSampling
{
    public static List<Vector2> GeneratePoints(float radius, Vector2 domain, int k = 30) {
        // Step 0. Initialize data structures.
        float cellSize = radius / Mathf.Sqrt(2);

        int height = Mathf.CeilToInt(domain.x / cellSize);
        int width = Mathf.CeilToInt(domain.y / cellSize);
        Vector2[,] grid = new Vector2[height, width];

        List<Vector2> points = new List<Vector2>();
        List<Vector2> active = new List<Vector2>();

        // Step 1. Select initial sample, x_0, randomly chosen uniformly from the domain.
        Vector2 x0 = new Vector2(Random.Range(0, domain.x), Random.Range(0, domain.y));
        grid[(int)(x0.x / cellSize), (int)(x0.y / cellSize)] = x0;
        points.Add(x0);
        active.Add(x0);

        while (active.Count > 0) {
            int spawnIndex = Random.Range(0, active.Count);
            Vector2 spawnCenter = active[spawnIndex];
            bool candidateFound = false;

            for (int i = 0; i < k; i++) {
                // Step 2: Generate up to k points chosen uniformly from the spherical annulus
                // between radius r and 2r around x_i. In this case x_i is the spawn center.
                // Find a suitable candidate or discard the active point as a suitable generator.
                float angle = Random.value * Mathf.PI * 2;
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                Vector2 candidate = spawnCenter + dir * Random.Range(radius, 2 * radius);
                if (IsValidPoint(candidate, domain, cellSize, radius, grid)) {
                    points.Add(candidate);
                    active.Add(candidate);
                    grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = candidate;
                    candidateFound = true;
                    break;
                }
            }

            if (!candidateFound) {
                active.RemoveAt(spawnIndex);
            }
        }

        return points;
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
