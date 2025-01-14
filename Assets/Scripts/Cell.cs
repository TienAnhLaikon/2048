using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{


    // tọa độ của ô này trong Grid(lưới)
    public Vector2Int coordinates { get; set; }
    // tile hiện tại nằm trong ô (cell) này
    public Tile tile { get; set; }

    // nếu Cell (ô) đang rỗng (không chứa gì cả) tức là tile trong này Null
    public bool empty => tile == null;

    // Nếu Cell (ô) đang không rỗng (có chứa tile) tức là tile không null 
    public bool occupied => tile != null;

}
