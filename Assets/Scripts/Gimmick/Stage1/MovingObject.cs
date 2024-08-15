using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField] private Transform moveingObject;
    [SerializeField] private float speed;
    [SerializeField] private Vector3 startPoint;
    [SerializeField] private Vector3 endPoint;
    private Vector3 curPoint;

    private void Start()
    {
        StartCoroutine(Move(startPoint, endPoint));
    }

    private IEnumerator Move(Vector3 start,  Vector3 end)
    {
        float elapsedSpeed = 0f;
        while(true)
        {
            elapsedSpeed += Time.deltaTime;
            if(elapsedSpeed >= speed)
            {
                transform.position = endPoint;
                break;
            }

            curPoint = Vector3.Lerp(start, end, elapsedSpeed / speed);
            transform.position = curPoint;

            yield return null;
        }

        StartCoroutine(Move(end, start));
    }
}
