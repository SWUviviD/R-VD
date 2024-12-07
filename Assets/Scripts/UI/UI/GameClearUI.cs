using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameClearUI : MonoBehaviour
{
    [SerializeField] private Transform panel;
    [SerializeField] private Transform btnHoverImage;

    [SerializeField] private EventTrigger restartEventTrigger;

    public void Init()
    {
        panel.gameObject.SetActive(false);
        btnHoverImage.gameObject.SetActive(false);
    }

    public void OnGameClear()
    {
        panel.gameObject.SetActive(true);
        btnHoverImage.gameObject.SetActive(false);
    }
}
