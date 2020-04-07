﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPreview : MonoBehaviour
{
    public enum DrawMode
    {
        NOISE_MAP,
        MESH,
        FALL_OF_MAP,
        BIOME
    }

    [Header("General Settings")]

    [SerializeField] DrawMode _drawMode;
    [SerializeField, Tooltip("Should the terrainmap update in the editor?")] bool _autoUpdate;

    [SerializeField] MeshSettings _meshSettings;
    [SerializeField] HeightMapSettings _heightMapSettings;
    [SerializeField] TextureData _textureData;
    [SerializeField] Material _terrainMaterial;

    [Header("Mesh Settings")]

    [SerializeField, Range(0, MeshSettings.NUMBER_SUPPORTED_LODS - 1)] int _editorPreviewLevelOfDetail;

    [Header("Drop")]

    [SerializeField] Renderer _textureRenderer;
    [SerializeField] MeshFilter _meshFilter;
    [SerializeField] MeshRenderer _meshRenderer;

    [Header("Biome Testing")]

    [SerializeField] NoiseMergeType _noiseMergeType;

    [SerializeField] Biome _biome;

    Transform _biomeContainer;

    public bool DoAutoUpdate() { return _autoUpdate; }

    public float GetScale() { return _meshSettings.MeshScale; }

    public void DrawMapInEditor()
    {
        //Used to store prefab objects in edit mode and delete them when changes are made (is a bit buggy)
        if (_biomeContainer != null)
            DestroyImmediate(_biomeContainer.gameObject);
        _biomeContainer = new GameObject().transform;
        _biomeContainer.parent = transform;

        //Apply material to mesh
        _textureData.ApplyToMaterial(_terrainMaterial);
        _textureData.UpdateMeshHeights(_terrainMaterial, _heightMapSettings.MinHeight, _heightMapSettings.MaxHeight);

        //Generate the heightmap for the chunk at origin
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(_meshSettings.NumVertsPerLine, _meshSettings.NumVertsPerLine, _heightMapSettings, Vector2.zero);

        if (_drawMode == DrawMode.NOISE_MAP)
            DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
        else if (_drawMode == DrawMode.MESH)
            DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.heightMap, _meshSettings, _editorPreviewLevelOfDetail));
        else if (_drawMode == DrawMode.FALL_OF_MAP)
            DrawTexture(TextureGenerator.TextureFromHeightMap( new HeightMap(FallofGenerator.GenerateFallofMap(_meshSettings.NumVertsPerLine), 0, 1)));
        else if (_drawMode == DrawMode.BIOME)
        {
            MeshData meshData = MeshGenerator.GenerateTerrainMesh(heightMap.heightMap, _meshSettings, _editorPreviewLevelOfDetail);
            DrawMesh(meshData);
            PrefabSpawner.SpawnOnChunk(_biome, heightMap, meshData, _meshSettings, _biomeContainer, Vector2.zero);
        }
    }

    //Draws on the plane (for noise display)
    public void DrawTexture(Texture2D texture)
    {
        _textureRenderer.sharedMaterial.mainTexture = texture;
        _textureRenderer.transform.localScale = new Vector3(texture.width / 10f, 1, texture.height / 10f);

        _textureRenderer.gameObject.SetActive(true);
        _meshFilter.gameObject.SetActive(false);
    }

    //Create the mesh from meshdata if that is to be displayed
    public void DrawMesh(MeshData meshData)
    {
        _meshFilter.sharedMesh = meshData.CreateMesh();

        _textureRenderer.gameObject.SetActive(false);
        _meshFilter.gameObject.SetActive(true);
    }

    //If changes are made, apply in edit mode
    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
            DrawMapInEditor();
    }

    void OnTextureValuesUpdated()
    {
        _textureData.ApplyToMaterial(_terrainMaterial);
    }

    //Primaraly used to init things in edit mode!
    private void OnValidate()
    {
        if (_meshSettings != null)
        {
            _meshSettings.OnValuesUpdated -= OnValuesUpdated;
            _meshSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (_heightMapSettings != null)
        {
            _heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
            _heightMapSettings.OnValuesUpdated += OnValuesUpdated;
            _heightMapSettings.NoiseSettingsData.OnValuesUpdated -= OnValuesUpdated;
            _heightMapSettings.NoiseSettingsData.OnValuesUpdated += OnValuesUpdated;
        }
        if (_biome != null)
        {
            _biome.OnValuesUpdated -= OnValuesUpdated;
            _biome.OnValuesUpdated += OnValuesUpdated;
            SubscribeToSpawnables(_biome.Spawnables);
        }
        if (_textureData != null)
        {
            _textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            _textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }
        if (_textureData != null)
        {
            _textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            _textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }
    }

    private void SubscribeToSpawnables(Spawnable[] spawnables)
    {
        for (int i = 0; i < spawnables.Length; i++)
        {
            spawnables[i].OnValuesUpdated -= OnValuesUpdated;
            spawnables[i].OnValuesUpdated += OnValuesUpdated;

            SubscribeToSpawnables(spawnables[i].SubSpawners);
        }
    }
}
