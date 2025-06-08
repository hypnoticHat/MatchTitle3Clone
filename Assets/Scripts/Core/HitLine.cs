using UnityEngine;
using System.Collections.Generic;

public class HitLine : MonoBehaviour
{
    public static HitLine Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Tile tile = other.GetComponent<Tile>();
        tile?.OnEnterHitZone();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Tile tile = other.GetComponent<Tile>();
        tile?.OnExitHitZone();
    }
}
