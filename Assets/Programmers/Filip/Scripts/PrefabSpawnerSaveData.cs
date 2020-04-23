﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class PrefabSpawnerSaveData
{
    static Dictionary<ChunkCoordIndex, StoredSaveData> _storedSaveDataDic = new Dictionary<ChunkCoordIndex, StoredSaveData>();

    public static void Save()
    {
        List<StoredSaveData> saveData = _storedSaveDataDic.Select(value => value.Value).ToList();
        Serialization.Save(Saving.FileNames.PREFAB_SPAWNING, saveData);
    }

    public static void Load()
    {
        _storedSaveDataDic = new Dictionary<ChunkCoordIndex, StoredSaveData>();

        List<StoredSaveData> saveData = (List<StoredSaveData>)Serialization.Load(Saving.FileNames.PREFAB_SPAWNING);

        if (saveData != null)
        {
            for (int i = 0; i < saveData.Count; i++)
            {
                AddPickup(saveData[i]);
            }
        }
    }

    public static void AddPickup(StoredSaveData saveDataToStore)
    {
        try
        {
            _storedSaveDataDic.Add(saveDataToStore.ChunkCoordIndex, saveDataToStore);
        }
        catch
        {
            Debug.LogError("There is already a pickup saved at this chunk coord and index!? Probably you have set the size of a pickup to 0, IT MUST BE ABOVE 0!  :  " + saveDataToStore.ChunkCoordIndex.ChunkCoord + "    " + saveDataToStore.ChunkCoordIndex.ItemIndex);
        }
    }

    //Used to say the pickup may spawn again
    public static void RemovePickup(StoredSaveData saveDataToStore)
    {
        if (_storedSaveDataDic.ContainsKey(saveDataToStore.ChunkCoordIndex))
            _storedSaveDataDic.Remove(saveDataToStore.ChunkCoordIndex);
    }

    //Checks if it should spawn depending on key (used in spawning script only)
    public static bool ContainsChunkCoordIndex(ChunkCoordIndex chunkCoordIndex)
    {
        return _storedSaveDataDic.ContainsKey(chunkCoordIndex);
    }
}
