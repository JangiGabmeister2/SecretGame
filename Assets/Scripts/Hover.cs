using System.Collections;
using UnityEngine;

public class Hover : MonoBehaviour
{
    [SerializeField] private float _relativePos;

    private void OnEnable()
    {
        StartCoroutine(UpAndDown());
    }

    private IEnumerator UpAndDown()
    {
        Vector2 pos = transform.position;

        transform.position = new Vector2(pos.x, transform.localPosition.y - _relativePos);

        yield return new WaitForSeconds(1f);

        transform.position = pos;

        yield return new WaitForSeconds(1f);

        StartCoroutine(UpAndDown());
    }
}
