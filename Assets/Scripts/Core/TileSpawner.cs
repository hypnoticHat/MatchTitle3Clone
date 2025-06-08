using UnityEngine;
using System.Collections.Generic;

public class TileSpawner : MonoBehaviour
{
    public string mapFileName = "beatmap";
    public Transform[] lanes;

    private List<BeatTileInfo> tilesToSpawn;
    private int index = 0;

    void Start()
    {
        var rawMap = BeatmapReader.ReadBeatmapWithTime(mapFileName);
        tilesToSpawn = BeatmapReader.ConvertToBeatTileInfo(rawMap);
        Debug.Log("Loaded tile count: " + tilesToSpawn.Count);

    }

    void Update()
    {
        if (GameManager.Instance.State != GameState.Playing) return;

        float musicTime = AudioManager.Instance.GetPlaybackTime();

        while (index < tilesToSpawn.Count && tilesToSpawn[index].time <= musicTime)
        {
            SpawnTile(tilesToSpawn[index]);
            index++;
        }
    }

    void SpawnTile(BeatTileInfo info)
    {
        GameObject obj = PoolManager.Instance.GetTile(info.isLong);
        obj.transform.position = lanes[info.lane].position;

        if (info.isLong)
        {
            Vector3 scale = obj.transform.localScale;
            scale.y = info.lengthY;
            obj.transform.localScale = scale;
        }

        obj.GetComponent<Tile>().Activate();
    }

    public void ResetSpawner()
    {
        index = 0;
        var rawMap = BeatmapReader.ReadBeatmapWithTime(mapFileName);
        tilesToSpawn = BeatmapReader.ConvertToBeatTileInfo(rawMap);
    }
}
