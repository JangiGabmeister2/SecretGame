using UnityEngine;

public class PositionPlayer : MonoBehaviour
{
    [SerializeField] private Vector2 _position;
    [SerializeField] private bool justX, justY;

    private void OnValidate()
    {
        if (justX)
        {
            justY = false;
        }
        else if (justY)
        {
            justX = false;
        }
    }

    public void TeleportPlayer(GameObject player)
    {
        if (justX)
        {
            Vector2 pos = new Vector2(_position.x, player.transform.position.y);
            _position = pos;
        }
        else if (justY)
        {
            Vector2 pos = new Vector2(player.transform.position.x, _position.y);
            _position = pos;
        }
         
        player.transform.position = _position;
    }
}
