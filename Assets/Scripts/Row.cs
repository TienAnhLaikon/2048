using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Row : MonoBehaviour
{
// row là một hàng và nó sẽ chứa 4 ô nên ta phải tạo 1 danh sách 



    public  Cell[] cells { get; private set; }

    private void Awake()
    {
        cells = GetComponentsInChildren<Cell>();
    }
}
