using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifePanel : MonoBehaviour
{
    Image[] lifeImages;

    public Color disableColor;

    private void Awake()
    {
        lifeImages = new Image[transform.childCount];
        for (int i=0;i<transform.childCount;i++)
        {
            Transform child = transform.GetChild(i);
            lifeImages[i] = child.GetComponent<Image>();
        }
    }

    private void OnEnable()
    {
        Player player = GameManager.Instance.Player;
        if(player != null)
        {
            player.onLifeChange += OnLifeChange;
        }
    }

    private void OnDisable()
    {
        if(GameManager.Instance != null )
        {
            Player player = GameManager.Instance.Player;
            player.onLifeChange -= OnLifeChange;
        }
    }

    private void OnLifeChange(int life)
    {
        for(int i=0;i<life;i++)
        {
            lifeImages[i].color = Color.white;
        }
        for(int i=life;i<lifeImages.Length;i++)
        {
            lifeImages[i].color = disableColor;
        }

    }
}
