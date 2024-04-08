using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShopScrollHead : MonoBehaviour
{
    public UIScrollView scrollView;
    public GameObject[] GOElements;

    public int FocusedElementIndex { get; set; }

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        ScaleElement();
        InfiniteScroll();
    }

    public void Initialize()
    {
        GetElements();
        SetConfig();
        SetFocusElement();
    }

    private void GetElements()
    {
        GOElements = new GameObject[scrollView.transform.childCount];
 
        for (int i = 0; i < scrollView.transform.childCount; i++)
        {
            GOElements[i] = scrollView.transform.GetChild(i).gameObject;
           
        }
    }

    private void SetConfig()
    {
        FocusedElementIndex = 0;
        scrollView.onDragStarted += OnStartScroll;
        scrollView.onDragFinished += OnEndScroll;
    }

    private void SetFocusElement()
    {
        Snap(FocusedElementIndex);
    }

    private void ScaleElement()
    {
        for (int i = 0; i < GOElements.Length; i++)
        {
            // Scale elements as needed
        }
    }

    private void Snap(int elementIndex)
    {
        FocusedElementIndex = elementIndex;
        scrollView.MoveRelative(new Vector3(-scrollView.transform.GetChild(FocusedElementIndex).localPosition.x, 0f, 0f));
    }

    private void OnStartScroll()
    {
        // Actions when scrolling starts
    }

    private void OnEndScroll()
    {
        // Actions when scrolling ends
        Snap(FocusedElementIndex);
    }

    private void InfiniteScroll()
    {
        for (int i = 0; i < GOElements.Length; i++)
        {
            // Check for infinite scroll and reposition elements if needed
        }
    }
}
