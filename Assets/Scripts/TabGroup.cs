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
    [SerializeField] private Animator _animator;
    private bool isStart = true;
    
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
        /*if (_selectedTab != null)
        {
            _selectedTab.Deselect();
        }
        _selectedTab = button;
        _selectedTab.Select();
        ResetTabs();
        button.Background.sprite = button.Selected;
        int index = button.transform.GetSiblingIndex();

        StartCoroutine(Hide(0));
        yield return new WaitForSeconds(1f);

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
                //StartCoroutine(Show(i));
            }
            else
            {
                _objectsToSwap[i].SetActive(false);
                //StartCoroutine(Hide(i));
            }
        }
        StartCoroutine(Show(0));
        yield return null;*/
        if (button == _selectedTab) return;
        StartCoroutine(Coroutine(button));
    }
    public IEnumerator Coroutine(TabButton button)
    {
        if (isStart)
        {
            if (_selectedTab != null)
            {
                _selectedTab.Deselect();
            }
            _selectedTab = button;
            _selectedTab.Select();
            ResetTabs();
            button.Background.sprite = button.Selected;
            yield return null;
            isStart = false;
        }
        else
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
            _animator.speed = 1 / Time.timeScale;
            //StartCoroutine(Hide(0));
            _animator.SetTrigger("Hide");
            yield return new WaitForSeconds(0.145f * Time.timeScale);

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
                    //StartCoroutine(Show(i));
                }
                else
                {
                    _objectsToSwap[i].SetActive(false);
                    //StartCoroutine(Hide(i));
                }
            }
            //StartCoroutine(Show(0));
            _animator.SetTrigger("Show");
            yield return null;

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
    public IEnumerator Show(int index = 0)
    {
        _animator.SetTrigger("Show");
        yield return new WaitForSeconds(1f);
        //_objectsToSwap[index].SetActive(true);
    }
    public IEnumerator Hide(int index)
    {
        _animator.SetTrigger("Hide");
        yield return new WaitForSeconds(1f);
        //_objectsToSwap[index].SetActive(false);
    }
}
