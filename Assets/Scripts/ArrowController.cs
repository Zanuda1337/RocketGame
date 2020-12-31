﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowController : MonoBehaviour
{
    [SerializeField] private Button _leftArrow;
    [SerializeField] private Button _rightArrow;
    [SerializeField] private List<GameObject> _pages;
    private int _lastPage;
    private int _firstPage;

    public int CurrentPage = 0;

    public void Start()
    {
        _firstPage = 0;
        _lastPage = _pages.Count - 1;
    }

    public void RightPage()
    {
            CurrentPage += 1;
            ManagePages();
    }
    public void LeftPage()
    {
            CurrentPage -= 1;
            ManagePages();
    }
    public void ManagePages()
    {
        for (int i = 0; i < _pages.Count; i++)
        {
            if (CurrentPage!=i)
            {
            _pages[i].SetActive(false);
            }
        }
        _pages[CurrentPage].SetActive(true);
    }
    public void BlockArrow(Button arrow, int page)
    {
        if (CurrentPage != page)
        {
            arrow.interactable = true;
        }
        else
        {
            arrow.interactable = false;
        }
    }
    public void Update()
    {
        BlockArrow(_rightArrow, _lastPage);
        BlockArrow(_leftArrow, _firstPage);
    }
}