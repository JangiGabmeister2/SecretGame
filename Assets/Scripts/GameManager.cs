using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<ShowTiles> secretTiles;
    [SerializeField] private List<Tilemap> levels;

    public void ShowSecretTiles()
    {
        foreach (var tile in secretTiles)
        {
            tile.BecomeVisible();
        }
    }

    public void DisableAllLevels()
    {
        foreach (var level in levels)
        {
            level.gameObject.SetActive(false);
        }
    }
}
