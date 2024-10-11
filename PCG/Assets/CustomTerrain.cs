using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[ExecuteInEditMode]


public class CustomTerrain : MonoBehaviour
{
    public Vector2 randomHeightRange = new Vector2(0, 0.1f);
    public Texture2D heightMapImage;
    public Vector3 heightMapScale = new Vector3(1, 1, 1);

    public Terrain terrain;
    public TerrainData terrainData;

    int hmr { get {  return terrainData.heightmapResolution; } }

    float[,] GetHeights()
    {
        return (terrainData.GetHeights(0, 0, hmr, hmr));
    }

    public void OnEnable()
    {
        Debug.Log("Init Terrain Data");
        terrain = this.GetComponent<Terrain>();
        terrainData = Terrain.activeTerrain.terrainData;
    }

    void Start()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        AddTag(tagsProp, "Terrain");
        AddTag(tagsProp, "Cloud");
        AddTag(tagsProp, "Shore");

        tagManager.ApplyModifiedProperties();
        this.gameObject.tag = "Terrain";

    }

    void AddTag(SerializedProperty tagsProp, string newTag)
    {
        bool found = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(newTag)) { found = true; break; }
        }

        if (!found)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty newTagProp = tagsProp.GetArrayElementAtIndex(0);
            newTagProp.stringValue = newTag;

        }
    }

    public void ResetTerrain()
    {
        float[,] heightMap = new float[hmr, hmr];
        /*for (int x = 0; x < hmr; x++)
        {
            for (int z = 0; z < hmr; z++)
            {
                heightMap[x, z] += 0;
            }
        }*/
        terrainData.SetHeights(0, 0, heightMap);
        
    }

    public void RandomTerrain()
    {
        float[,] heightMap = GetHeights();
        for(int x = 0; x < hmr; x++)
        {
            for(int z = 0; z < hmr; z++)
            {
                heightMap[x, z] += UnityEngine.Random.Range(randomHeightRange.x, randomHeightRange.y);
            }
        }
        terrainData.SetHeights(0,0, heightMap);
    }

    public void LoadTexture()
    {
        float[,] heightMap;
        heightMap = new float[hmr, hmr];
        for(int x = 0; x < hmr; x++)
        {
            for(int z = 0; z < hmr; z++)
            {
                heightMap[x, z] = heightMapImage.GetPixel((int)(x * heightMapScale.x), (int)(z * heightMapScale.z)).grayscale * heightMapScale.y;
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }
}
