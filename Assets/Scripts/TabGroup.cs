using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    private List<TabButton> _tabButtons;
    private TabButton _selectedTab;
    [SerializeField] private TabButton _startTab;
    [SerializeField] private List<GameObject> _objectsToSwap;
    [SerializeField] private ArrowController _arrowController;
    [SerializeField] private int _tabToReset;
    private bool _isTabForced = false;


    public void Subscribe(TabButton button)
    {
        if (_tabButtons == null)
        {
            _tabButtons = new List<TabButton>();
        }

        _tabButtons.Add(button);
    }
    public void Start()
    {
        if (_startTab!=null)
        {
            OnTabSelected(_startTab);
        }
    }
    public void Update()
    {
        if (Rocket.instance != null && _startTab != null && Rocket.instance.Status != Rocket.State.Playing && !_isTabForced)
        {
            OnTabSelected(_startTab);
            _isTabForced = true;
        }
    }
    public void OnTabSelected(TabButton button)
    {
        if (_selectedTab != null)
        {
            _selectedTab.Deselect();
        }
        _selectedTab = button;
        _selectedTab.Select();
        ResetTabs();
        button.Background.sprite = button.Selected;
        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < _objectsToSwap.Count; i++)
        {
            if (i == index)
            {
                if (_tabToReset == i)
                {
                _arrowController.CurrentPage = 0;
                _arrowController.ManagePages();
                }
                _objectsToSwap[i].SetActive(true);
            }
            else
            {
                _objectsToSwap[i].SetActive(false);
            }
        }
    }
    public void ResetTabs()
    {
        foreach (TabButton button in _tabButtons)
        {
            if (_selectedTab!=null && button==_selectedTab) { continue; }
            button.Background.sprite = button.Idle;
        }
    }
}
