using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public TileState state { get; private set; }
    
    public Cell cell { get; private set; }

    public int number { get; set; }    
    // Locked để xử lý tránh việc bị merge liên tiếp
    public bool locked { get; set; }
    private Image background;
    private TextMeshProUGUI text;
    private void Awake()
    {
        background = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    // hàm set State cho Tile mỗi khi cập nhật merge
    public void SetState(TileState newState, int newNumber)
    {
        this.state = newState;
        this.number = newNumber;

        background.color = state.backgroundColor;
        text.color = state.textColor;
        text.text = number.ToString();
    }

    // sinh ra tile tại 1 ô Cell cụ thể
    public void Spawn (Cell cell)
    {
        // this.cell kiểm tra xem ô tile này có đang gắn vào ô Cell nào không
        if (this.cell != null) { this.cell.tile = null; }
        
        this.cell = cell;        // gán ô Cell mới cho Tile
        // gán cho ô (cell) mà tile này sinh ra ở đó chứa Tile hiện tại này luôn
        this.cell.tile = this; // gán Tile hiện tại cho ô Cell mới

        /*Nó tạo ra liên kết 2 chiều giữa Cell và Tile
         Cell (ô) biết Tile nào đang nằm bên trong nó (this.cell.tile).
        Tile biết được nó đang nằm ở trong Cell nào (this.cell).
    
         */
        // di chuyển ô Tile đến vị trí của ô Cell đó
        // Điều này đảm bảo rằng khi ta sinh ra Tile ở ô nào thì nó sẽ nằm giữa đúng ô đó
        transform.position = cell.transform.position;

    }
    // hàm này thì thay vì sinh ra ô Tile thì nó xử lý việc tile hiện tại di chuyển đến một Ô Cell cụ thể nào đó
    public void MoveTo(Cell cell)
    {

        // Nên ta cũng kiểm tra tile này đang nằm trong Cell nào không?
        if (this.cell != null)
        {
            this.cell.tile = null; // lập tức xóa liên kết ô trước đi
        }
        // cập nhật đến với ô mới
        this.cell = cell; // cập nhật ô cell trong Tile
        this.cell.tile = this; // cập nhật Tile đang nằm trong Cell đó
        StartCoroutine(Animate(cell.transform.position, false));
    }
    // hàm này để xử lý hoạt ảnh animation di chuyển tile đến ô mới 1 cách mượt mà
    public IEnumerator Animate(Vector3 to, bool merging)
    {
        float elapsed = 0f;
        float duration = 0.1f;
        // lấy ra vị trí hiện tại là From để đến to
        Vector3 from = transform.position;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(from, to, elapsed/duration);   
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = to;
        // nếu là merge hợp nhất thì ta sẽ tiêu hủy tile đi
        if (merging)
        {
            Destroy(gameObject);
        }
    }
    

    // hàm này để xử lý việc khi hợp nhất ô đến một cell cụ thể
    // bản chất của tile hiện tại hợp nhất với tile cụ thể đang ở trong Cell tham số
    // nên ta tạo hoạt ảnh di chuyển đến và lập tức xóa nó đi 
    // xóa tile trong cell và xóa cell trong tile này
    public void Merge(Cell cell)
    {
        // xóa ô cell cũ. Ô hiện tại không còn chứa tile này nữa
        if (this.cell != null) this.cell.tile = null;
        // vì tile hiện tại sẽ bị xóa vì hợp nhất nên ô hiện tại của nó cũng không chứa tile nào
        this.cell = null;
        cell.tile.locked = true;
        // hoạt ảnh chạy đến ô cell và biến mất
        StartCoroutine(Animate(cell.transform.position, true));

    }

}
