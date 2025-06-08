using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }

    private List<Tile> tilesInZone = new List<Tile>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddTileToZone(Tile tile)
    {
        if (!tilesInZone.Contains(tile))
        {
            tilesInZone.Add(tile);
        }
    }

    public void RemoveTileFromZone(Tile tile)
    {
        if (tilesInZone.Contains(tile))
        {
            tilesInZone.Remove(tile);
        }
    }

    public Tile GetTopmostTileInZone()
    {
        if (tilesInZone.Count == 0) return null;

        Tile closest = tilesInZone[0];
        float minY = closest.transform.position.y;

        foreach (var tile in tilesInZone)
        {
            if (tile.transform.position.y < minY)
            {
                closest = tile;
                minY = tile.transform.position.y;
            }
        }

        return closest;
    }

    public void ClearZone()
    {
        foreach (var tile in FindObjectsOfType<Tile>(true))
        {
            tile.StopAllCoroutines();
            tile.gameObject.SetActive(false);
        }
        tilesInZone.Clear();
    }


}