using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddScreenMenuController : MonoBehaviour
{
    [SerializeField] private Image _mapImage;
    [SerializeField] private Button _addButton;
    // Start is called before the first frame update
    void Start()
    {
        _addButton.onClick.AddListener(WatchAdd);
    }

    private void WatchAdd()
    {
        GameManager.SINGLETON.MainMenuAddReward();
        gameObject.SetActive(false);
    }

    public void Initialize(Image mapImage)
    {
        _mapImage.sprite = mapImage.sprite;
    }
}
