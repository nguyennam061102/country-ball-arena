using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollViewManager : MonoBehaviour
{
    public UIScrollView scrollView;
    public UIGrid grid;
    //public GameObject itemPrefab;
    //public int numberOfItems = 10; // Số lượng item ban đầu
    //public float itemHeight = 100f; // Chiều cao của mỗi item
    //private int currentItemIndex = 0;

    void Start()
    {
        // Khởi tạo ScrollView với số lượng item ban đầu
        //for (int i = 0; i < numberOfItems; i++)
        //{
        //    GameObject newItem = NGUITools.AddChild(contentPanel.gameObject, itemPrefab);
        //    newItem.name = "Item " + i;
        //    newItem.transform.localPosition = new Vector3(0, -i * itemHeight, 0);
        //}
    }
    [Button]
    void Test()
    {
        // Kiểm tra nếu item đầu tiên đã được kéo khuất
        
            // Di chuyển item đầu tiên xuống cuối danh sách
            Transform firstItem = grid.transform.GetChild(0);
            firstItem.SetAsLastSibling();
        grid.Reposition();
        scrollView.ResetPosition();
            //firstItem.localPosition = new Vector3(0, -numberOfItems * itemHeight, 0);
            //currentItemIndex++;

    }
}
