﻿using UnityEngine;

public class UIStarsContainer : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _starImages;

    public void ShowCollectedStars(int stars)
    {
        for (int i = 0; i < stars; i++)
        {
            _starImages[i].SetActive(true);
        }
    }
}