using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateSamplePlatform : MonoBehaviour
{
    [Header("Blocks")]
    [SerializeField] private GameObject plainBlock;
    [SerializeField] private GameObject pointBlock;
    [SerializeField] private GameObject savePointBlock;
    [SerializeField] private GameObject plain;
    [SerializeField][Range(0f, 1f)] private float pointBlockRate = 0.2f;

    [Header("PlatformSetting")]
    [SerializeField] private float blockOffset = 2f;
    [SerializeField] private Vector2Int size = new Vector2Int(5, 5);
    [SerializeField] private bool isThisSavePoint = false;

    [ContextMenu("MakeMapInHorizontial")]
    private void MakeMap()
    {
        GameObject parent = new GameObject($"Map {size.x} by {size.y}" + 
            (isThisSavePoint ? "with Save" : string.Empty));

        Vector3 position = new Vector3(
            -((size.x - 1) * blockOffset) / 2f,
            0f,    -((size.y - 1) * blockOffset) / 2f);

        for (int i = 0; i < size.x; ++i)
        {
            for(int j = 0; j < size.y; ++j)
            {
                GameObject block;
                if(Random.Range(0, 10) / 10f <= pointBlockRate)
                {
                    block = Instantiate(pointBlock, parent.transform);
                }
                else
                {
                    block = Instantiate(plainBlock, parent.transform);
                }

                block.transform.localPosition = position;
                position.z += blockOffset;


                if (i == 0 || j == 0 || i == size.x - 1 || j == size.y - 1)
                {
                    continue;
                }

                block.GetComponentInChildren<Collider>().enabled = false;
            }
            position.x += blockOffset;
            position.z = -((size.y - 1) * blockOffset) / 2f;
        }

        if(isThisSavePoint)
        {
            var savePoint = Instantiate(savePointBlock, parent.transform);
            savePoint.transform.localPosition = Vector3.zero;

            savePoint.GetComponentInChildren<Collider>().enabled = false;
        }

        var p = Instantiate(plain, parent.transform);
        p.transform.localPosition = new Vector3(0f, plain.transform.position.y, 0f);
        p.transform.localScale = new Vector3(size.x - 1, 1f, size.y - 1);
    }

    [ContextMenu("MakeMapInVertical")]
    private void MakeMapV()
    {
        GameObject parent = new GameObject($"Map Vertical {size.x} by {size.y}" +
            (isThisSavePoint ? "with Save" : string.Empty));

        Vector3 position = new Vector3(
            -((size.x - 1) * blockOffset) / 2f,
            -((size.y - 1) * blockOffset) / 2f, 0f);

        for (int i = 0; i < size.x; ++i)
        {
            for (int j = 0; j < size.y; ++j)
            {
                GameObject block;
                if (Random.Range(0, 10) / 10f <= pointBlockRate)
                {
                    block = Instantiate(pointBlock, parent.transform);
                }
                else
                {
                    block = Instantiate(plainBlock, parent.transform);
                }

                block.transform.localPosition = position;
                position.y += blockOffset;


                if (i == 0 || j == 0 || i == size.x - 1 || j == size.y - 1)
                {
                    continue;
                }

                block.GetComponentInChildren<Collider>().enabled = false;
            }
            position.x += blockOffset;
            position.y = -((size.y - 1) * blockOffset) / 2f;
        }
    }
}
