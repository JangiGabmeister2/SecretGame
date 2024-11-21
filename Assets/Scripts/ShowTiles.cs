using UnityEngine;

public class ShowTiles : MonoBehaviour
{
    SpriteRenderer sr => GetComponent<SpriteRenderer>();

    public void BecomeVisible()
    {
        sr.color = new Color(255, 18, 0, 255);
    }
}
