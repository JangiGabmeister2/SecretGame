using UnityEngine;

public class ShowTiles : MonoBehaviour
{
    [SerializeField] private Sprite _sprite;

    SpriteRenderer sr => GetComponent<SpriteRenderer>();

    public void BecomeVisible()
    {
        sr.sprite = _sprite;
    }
}
