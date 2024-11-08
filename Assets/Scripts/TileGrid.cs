using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    // Grid lưới là bảng lưới nằm trong Board chứa 16 ô (Cell) để chứa các ô Tile để ta chơi
    // nên Grid sẽ phải tìm đến các Rows và Cells

    /* Tại sao ta cần quản lý các Cells trong Grid?
     Tại vì ở mỗi Row chỉ là 1 hàng riêng lẻ chứa 4 ô
    nhưng ở Grid ta có thể quản lý cả 4 hàng và 16 ô luôn
    nên xử lý sẽ thống nhất và dễ dàng hơn rất nhiều

        Grid xử lý Rows và Cells
     */
    public Row[] rows { get; private set; }
    public Cell[] cells { get; private set; }

    public int sizeOfGrid => cells.Length;
    public int height => rows.Length;
    public int width => sizeOfGrid / height;
    private void Awake()
    {
        rows = GetComponentsInChildren<Row>();
        cells = GetComponentsInChildren<Cell>();
    }
    // riêng Grid ta cần dùng Start để chạy lần đầu tiên qua 4 hàng 16 ô để gán hết vị trí

    private void Start()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < rows[y].cells.Length; x++)
            {
                rows[y].cells[x].coordinates = new Vector2Int(x,y);
            }
        }
    }
    public Cell GetCell(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height) return rows[y].cells[x];
        else return null;
    }
    public Cell GetRandomEmptyCellInGrid()
    {
        // random 1 số tron khoảng từ 0 đến 16 cho index
        int index = Random.Range(0, sizeOfGrid);
        int startingIndex = index; // lưu lại chỉ số ban đầu để kiểm tra ta đã duyệt qua toàn bộ mảng cells hay chưa
        // lặp qua các ô để tìm ô random
        // ta sẽ duyệt qua 16 ô để tìm ra ô random trong 16 ô nhưng phải Rỗng không chứa tile nào nếu cứ chứa tile là tăng index duyệt tiếp
        
        while (cells[index].occupied)
        {
            // Nếu ô hiện tại bị chiếm ta tăng index để kiểm tra ô tiếp theo
            index++;
            if (index >= sizeOfGrid) index = 0; // giả sử Index nó bằng hoặc vượt quá 16 thì ta quay lại 0 để tìm lại
            if (index == startingIndex) return null; // nếu nó bằng chỉ số ban đầu tức là nó đã duyệt hết 1 vòng kh có kết quả rồi nên trả về null
        }
        return cells[index];
    }

    public Cell GetAdjacentCell(Cell cell, Vector2Int direction)
    {
        // hàm này sẽ lấy ra Cell kề cận của Cell hiện tại
        // ta cần phải có hướng (Direction) của nó để tìm ra hướng tiếp theo
        // lấy ra vị trí Cell hiện tại
        Vector2Int coordinates = cell.coordinates;
        coordinates.x += direction.x;
        coordinates.y -= direction.y;
        return GetCell(coordinates.x, coordinates.y);
    }
}
