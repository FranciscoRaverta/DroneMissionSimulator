using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class FertilityData : ScriptableObject {
    public AnimationCurve fertility;
    public int seed;
    public GameObject tree;
}