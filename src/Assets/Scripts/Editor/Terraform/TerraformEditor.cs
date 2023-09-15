using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Terraformer))]
[CanEditMultipleObjects]

public class TerraformerEditor : Editor {
    Terraformer terraformer;

    // General
    SerializedProperty terrain;
    SerializedProperty terrainMaterial;
    SerializedProperty size;
    SerializedProperty previewMode;
    SerializedProperty autoUpdate;

    // Land properties
    SerializedProperty noiseDataLand;
    SerializedProperty terrainData;

    // Tree properties
    SerializedProperty noiseDataFertility;
    SerializedProperty fertilityData;
    SerializedProperty treeData;

    // Data editors
    Editor terrainDataEditor;
    Editor noiseDataLandEditor;
    Editor noiseDataFertilityEditor;
    Editor fertilityZonesEditor;


    void OnEnable() {
        terraformer = (Terraformer)target;
        terraformer.Initialize();
        // General
        terrain = serializedObject.FindProperty("terrain");
        terrainMaterial = serializedObject.FindProperty("terrainMaterial");
        size = serializedObject.FindProperty("size");
        previewMode = serializedObject.FindProperty("previewMode");
        autoUpdate = serializedObject.FindProperty("autoUpdate");

        // Land
        noiseDataLand = serializedObject.FindProperty("terrainNoiseData");
        noiseDataFertility = serializedObject.FindProperty("noiseDataFertility");
        fertilityData = serializedObject.FindProperty("fertilityData");
        terrainData = serializedObject.FindProperty("terrainData");

        // Trees
        treeData = serializedObject.FindProperty("treeData");
    }
    public override void OnInspectorGUI() {
        GUIStyle richTextStyle = new GUIStyle();
        richTextStyle.richText = true;
        serializedObject.Update();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("<color=#d85d5d><b>General</b></color>", richTextStyle);
        EditorGUILayout.PropertyField(terrainMaterial, new GUIContent("Material"));
        EditorGUILayout.PropertyField(size);
        EditorGUILayout.PropertyField(previewMode);
        EditorGUILayout.PropertyField(autoUpdate);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("<color=lime><b>Terrain</b></color>", richTextStyle);
        EditorGUILayout.PropertyField(noiseDataLand, new GUIContent("Noise Data"));
        EditorGUILayout.PropertyField(terrainData);
        if (GUILayout.Button("Generate terrain")) {
            terraformer.GenerateTerrain();
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("<color=lime><b>Fertility</b></color>", richTextStyle);
        EditorGUILayout.PropertyField(noiseDataFertility, new GUIContent("Noise Data"));
        EditorGUILayout.PropertyField(fertilityData, new GUIContent("Fertility Data"));
        EditorGUILayout.PropertyField(treeData, new GUIContent("Tree Data"));
        if (GUILayout.Button("Plant trees")) {
            terraformer.GenerateTrees();
        }

        serializedObject.ApplyModifiedProperties();

        // Draw editor for data assets. We also want to autoupdate when values change.
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("<color=orange><b>Data</b></color>", richTextStyle);
        DrawDataEditor(terraformer.terrainNoiseData, terraformer.OnNoiseDataLandUpdated, ref terraformer.noiseDataLandFoldout, ref noiseDataLandEditor);
        DrawDataEditor(terraformer.terrainData, terraformer.OnNoiseDataLandUpdated, ref terraformer.terrainDataFoldout, ref terrainDataEditor);
        DrawDataEditor(terraformer.noiseDataFertility, terraformer.OnNoiseDataFertilityUpdated, ref terraformer.noiseDataFertilityFoldout, ref noiseDataFertilityEditor);
        DrawDataEditor(terraformer.fertilityData, terraformer.OnFertilityUpdated, ref terraformer.fertilityDataFoldout, ref fertilityZonesEditor);
    }

    void DrawDataEditor(Object data, System.Action onDataUpdated, ref bool foldout, ref Editor editor) {
        if (data != null) {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, data);

            using (var check = new EditorGUI.ChangeCheckScope()) {
                if (foldout) {
                    CreateCachedEditor(data, null, ref editor);
                    editor.OnInspectorGUI();
                    if (check.changed) {
                        if (onDataUpdated != null) {
                            onDataUpdated();
                        }
                    }
                }
            }
        }
    }
}
