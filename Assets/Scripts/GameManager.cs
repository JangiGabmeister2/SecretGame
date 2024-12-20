using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<ShowTiles> secretTiles;
    [SerializeField] private List<Tilemap> levels;
    [SerializeField] private List<Tilemap> hiddenTiles;

    private void Start()
    {
        DisableAllLevels();

        foreach (var level in levels)
        {
            if (level.name == "G_1")
            {
                level.gameObject.SetActive(true);
            }
        }
    }

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

    public void ShowAllHiddenTiles()
    {
        foreach(var level in hiddenTiles)
        {
            level.gameObject.SetActive(true);
        }
    }
}
