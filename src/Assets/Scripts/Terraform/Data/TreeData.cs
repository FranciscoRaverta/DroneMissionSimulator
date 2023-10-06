using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TreeData : ScriptableObject { 
    public Tree[] trees;
    public GameObject grass;

    public GameObject Evaluate(float t) {
        foreach (Tree tree in trees) {
            if (t <= tree.probability) {
                return tree.treeObject;
            }
        }
        return trees[trees.Length - 1].treeObject;
    }
}

[System.Serializable]
public struct Tree {
    [Range(0, 1)] public float probability;
    public GameObject treeObject;
}