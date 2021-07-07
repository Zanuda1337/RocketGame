using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowController : MonoBehaviour
{
    [SerializeField] private Button _leftArrow;
    [SerializeField] private Button _rightArrow;
    [SerializeField] private List<GameObject> _pages;
    [SerializeField] private Animator _animator;
    private int _lastPage;
    private int _firstPage;
    [SerializeField] private float _delay = 0.14f;
    [SerializeField] private AppearanceButton _button;

    public int CurrentPage = 0;

    public void Start()
    {
        _firstPage = 0;
        _lastPage = _pages.Count - 1;
        BlockArrow(_rightArrow, _lastPage);
        BlockArrow(_leftArrow, _firstPage);
    }

    public void RightPage()
    {
        StartCoroutine(Right()); 
        /*CurrentPage += 1;
        ManagePages();
        _animator.SetTrigger("Hide");*/
    }
    public void LeftPage()
    {
        StartCoroutine(Left());
        /*CurrentPage -= 1;
        ManagePages();
        _animator.SetTrigger("Hide");*/
    }
    public IEnumerator Right()
    {
        _animator.speed = 1 / Time.timeScale;
        CurrentPage += 1;
        BlockArrow(_rightArrow, _lastPage);
        BlockArrow(_leftArrow, _firstPage);
        _animator.SetTrigger("HideLeft");
        yield return new WaitForSeconds(_delay * Time.timeScale);
        ManagePages();
        _animator.SetTrigger("ShowRight");
    }
    public IEnumerator Left()
    {
        _animator.speed = 1 / Time.timeScale;
        CurrentPage -= 1;
        BlockArrow(_rightArrow, _lastPage);
        BlockArrow(_leftArrow, _firstPage);
        _animator.SetTrigger("HideRight");
        yield return new WaitForSeconds(_delay * Time.timeScale);
        ManagePages();
        _animator.SetTrigger("ShowLeft");
    }
    public void ManagePages()
    {
        BlockArrow(_rightArrow, _lastPage);
        BlockArrow(_leftArrow, _firstPage);
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
}
