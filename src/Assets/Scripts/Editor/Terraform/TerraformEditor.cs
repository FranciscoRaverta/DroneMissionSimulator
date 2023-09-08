using UnityEngine;
using UnityEditor;
//using EditorGUITable;

[CustomEditor(typeof(MapGenerator))]
[CanEditMultipleObjects]

public class MapGeneratorEditor : Editor
{

    // Land properties
    SerializedProperty mapWidth;
    SerializedProperty mapHeight;
    SerializedProperty octaves;
    SerializedProperty lacunarity;
    SerializedProperty noiseScale;
    SerializedProperty heightMultiplicationFactor;
    SerializedProperty heightDampeningFactor;
    SerializedProperty offset;
    SerializedProperty seed;
    SerializedProperty regions;

    // Vegetation properties
    SerializedProperty trees;
    SerializedProperty treeSpacing;

    // Private properties
    private Color[] colorMap;
    private float[,] noiseMap;

    // Showing foldouts
    bool showLand = false;
    bool showVeg = false;


    void OnEnable()
    {
        mapWidth = serializedObject.FindProperty("mapWidth");
        mapHeight = serializedObject.FindProperty("mapHeight");
        octaves = serializedObject.FindProperty("octaves");
        noiseScale = serializedObject.FindProperty("noiseScale");
        lacunarity = serializedObject.FindProperty("lacunarity");
        heightMultiplicationFactor = serializedObject.FindProperty("heightMultiplicationFactor");
        heightDampeningFactor = serializedObject.FindProperty("heightDampeningFactor");
        offset = serializedObject.FindProperty("offset");
        seed = serializedObject.FindProperty("seed");
        regions = serializedObject.FindProperty("regions");
        trees = serializedObject.FindProperty("trees");
        treeSpacing = serializedObject.FindProperty("treeSpacing");
    }
    Vector2 scrollPos;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        MapGenerator mapGen = (MapGenerator) target;

        Rect r = EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(r.width), GUILayout.Height(r.height));
        EditorGUI.indentLevel++;


        showLand = EditorGUILayout.Foldout(showLand, "Land");
        if (showLand)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Set vertices for mesh", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(mapWidth);
            EditorGUILayout.PropertyField(mapHeight);
            GUILayout.Label("Perlin noise properties", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(octaves);
            EditorGUILayout.PropertyField(lacunarity);
            EditorGUILayout.PropertyField(noiseScale);
            EditorGUILayout.PropertyField(heightMultiplicationFactor);
            EditorGUILayout.PropertyField(heightDampeningFactor);
            EditorGUILayout.PropertyField(offset);
            EditorGUILayout.PropertyField(seed);
            EditorGUILayout.PropertyField(regions);
            if (GUILayout.Button("Generate land"))
            {
                mapGen.GenerateMap();
            }
        }

        showVeg = EditorGUILayout.Foldout(showVeg, "Vegetation");
        if (showVeg) {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Tree properties", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(trees);
            EditorGUILayout.PropertyField(treeSpacing);
            if (GUILayout.Button("Generate trees")) {
                mapGen.GenerateTrees();
            }
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
