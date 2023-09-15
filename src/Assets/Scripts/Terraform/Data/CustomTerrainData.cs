using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CustomTerrainData : ScriptableObject
{
    public float heightMultiplier;
    public AnimationCurve heightCurve = AnimationCurve.Constant(0, 1, 1);
}
