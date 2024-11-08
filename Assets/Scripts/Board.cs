using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    // Board sẽ quản lý Grid tương tác với nó để xử lý game
    private Grid grid;

    public Tile tilePrefab;
    public GameManager gameManager;
    public TileState[] tileStates;
    private List<Tile> tiles;
    private bool waiting;
    void Awake()
    {
        grid = GetComponentInChildren<Grid>();
        tiles = new List<Tile>(16);
    }

    // tạo ra một Tile
    public void CreateTile()
    {
        Tile tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates[0], 2);
        tile.Spawn(grid.GetRandomEmptyCellInGrid());
        tiles.Add(tile);
    }
    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        // lưu trữ một tham chiếu đến newCell mà cell sẽ dịch chuyển đến
        Cell newCell = null;
        // lấy ra cell kề cận gần cell hiện tại và kiểm tra xem cell đó có occupied không
        Cell adjacent = grid.GetAdjacentCell(tile.cell, direction);
        while (adjacent != null)
        {
            if (adjacent.occupied)
            {
                // merging right here
                if (CanMerge(tile, adjacent.tile))
                {
                    Merge(tile, adjacent.tile);
                    return true;
                }
                break;
            }
            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);

        }
        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }
        return false;
    }
    private void MoveTiles(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool changed = false;
        for (int x = startX; x >= 0 && x < grid.width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < grid.height; y += incrementY)
            {
                Cell cell = grid.GetCell(x, y);
                if (cell.occupied)
                {
                    changed |= MoveTile(cell.tile, direction);
                }
            }
        }
        if (changed)
        {
            StartCoroutine(WaitingForChanges());
        }
    }
    public bool CanMerge(Tile a, Tile b)
    {
        return a.number == b.number && !b.locked;
    }
    public void Merge(Tile a, Tile b)
    {
        // merge thì ta sẽ hợp nhất ô a vào ô B
        // tức là ô A chạy đến ô B lấy vị trí của ô B và ô A sẽ biến mất
        tiles.Remove(a);
        // tile a chạy đến tile b và biến mất
        a.Merge(b.cell);
        int index = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length - 1);
        int number = b.number * 2;
        b.SetState(tileStates[index], number);

        // increaseScore in here
        gameManager.IncreaseScore(number);
    }
    // hàm lấy ra chỉ số hiện tại của state trong mảng
        public int IndexOf(TileState state)
    {
        for (int i =0; i < tileStates.Length; i++)
        {
            if (state == tileStates[i]) return i;
        }
        return -1;
    }
    // Update is called once per frame
    public bool CheckForGameOver()
    {
        if (tiles.Count != grid.sizeOfGrid) return false;

        foreach (var tile in tiles) {
            Cell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            Cell down = grid.GetAdjacentCell(tile.cell,Vector2Int.down);
            Cell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            Cell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if (up!= null && CanMerge(tile, up.tile)) return false;
            else if (down!= null && CanMerge(tile, down.tile)) return false;
            else if (left != null && CanMerge(tile, left.tile)) return false;
            else if (right != null && CanMerge(tile, right.tile)) return false;
        }
        return true;
    }
    private IEnumerator WaitingForChanges()
    {
        waiting = true;
        yield return new WaitForSeconds(0.1f);
        waiting = false;
        // to do: create a new tile

        foreach (var tile in tiles)
        {
            tile.locked = false;
        }
        if (tiles.Count != grid.sizeOfGrid)
        {
            CreateTile();
        }
        // to do: check for game over
        if (CheckForGameOver())
        {
            gameManager.GameOver();
        }
    }
    public void ClearBoard()
    {
        foreach (var tile in tiles)
        {
            Destroy(tile.gameObject);
        }
        foreach (var cell in grid.cells)
        {
            cell.tile = null;
        }
        tiles.Clear();
    }
    private void Update()
    {
        if (!waiting)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                MoveTiles(Vector2Int.up, 0, 1, 1, 1);
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                MoveTiles(Vector2Int.down, 0, 1, grid.height - 2, -1);
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                MoveTiles(Vector2Int.left, 1, 1, 0, 1);
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                MoveTiles(Vector2Int.right, grid.width - 2, -1, 0, 1);
        }
    }
}

