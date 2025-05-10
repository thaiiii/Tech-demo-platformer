using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerTrace : MonoBehaviour
{
    [Header("Tilemaps")]
    private List<Tilemap> tilemaps = new List<Tilemap>(); // Cac Tilemap ma ban se danh dau khi di qua (nơi Player di chuyển)

    private Tilemap floorTilemap;
    private Tilemap normalWallTilemap;
    private Tilemap glassWallTilemap;

    private Tilemap markTopTilemap; // Tilemap dùng để đặt dấu vết trên
    private Tilemap markBottomTilemap; // Tilemap dùng để đặt dấu vết dưới
    private Tilemap markRightTilemap; // Tilemap dùng để đặt dấu vết phải
    private Tilemap markLeftTilemap; // Tilemap dùng để đặt dấu vết trái


    [Header("Marks prite")]
    public TileBase markTopTile; // Tile dấu vết phía trên
    public TileBase markBottomTile; // Tile dấu vết phía dưới
    public TileBase markRightTile; // Tile dấu vết phía phải
    public TileBase markLeftTile; // Tile dấu vết phía trái
    
    public LayerMask floorLayer;

    private float playerScale;
    private void Awake()
    {
        //Tìm các tilemap có thể đánh dấu trong scene
        GameObject grid = GameObject.Find("Grid");
        GameObject walls = grid.transform.Find("Wall").gameObject;              
        GameObject marks = grid.transform.Find("Mark").gameObject;              
        
        floorTilemap = grid.transform.Find("Floor")?.GetComponent<Tilemap>();               
        normalWallTilemap = walls.transform.Find("NormalWall")?.GetComponent<Tilemap>();
        glassWallTilemap = walls.transform.Find("GlassWall")?.GetComponent<Tilemap>();
        markTopTilemap = marks.transform.Find("MarkTopTilemap")?.GetComponent<Tilemap>();
        markBottomTilemap = marks.transform.Find("MarkBottomTilemap")?.GetComponent<Tilemap>();
        markRightTilemap = marks.transform.Find("MarkRightTilemap")?.GetComponent<Tilemap>();
        markLeftTilemap = marks.transform.Find("MarkLeftTilemap")?.GetComponent<Tilemap>();
    }

    private void Start()
    {
        tilemaps.Add(floorTilemap);
        tilemaps.Add(normalWallTilemap);
        tilemaps.Add(glassWallTilemap);
    }

    private void Update()
    {
        PlaceMark();
    }

    void PlaceMark()
    {
        // Lấy tọa độ, scale của Player (có offset)
        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider == null)
        {
            Debug.LogWarning("Player doesn't have a Collider2D attached!");
            return;
        }

        Bounds bounds = playerCollider.bounds;
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        // Offset: cách các cạnh của collider
        Vector3 bottomOffset = new Vector3(0, extents.y + 0.05f, 0);
        Vector3 topOffset = new Vector3(0, -extents.y - 0.05f, 0);
        Vector3 leftOffset = new Vector3(extents.x + 0.05f, 0, 0);
        Vector3 rightOffset = new Vector3(-extents.x - 0.05f, 0, 0);

        

        for (int i = 0; i < tilemaps.Count; i++)
        {
            Vector3Int topTile = tilemaps[i].WorldToCell(center + topOffset);
            Vector3Int bottomTile = tilemaps[i].WorldToCell(center + bottomOffset);
            Vector3Int rightTile = tilemaps[i].WorldToCell(center + rightOffset);
            Vector3Int leftTile = tilemaps[i].WorldToCell(center + leftOffset);


            if (tilemaps[i].HasTile(topTile) && markTopTilemap.GetTile(topTile) == null)
                markTopTilemap.SetTile(topTile, markTopTile);

            if (tilemaps[i].HasTile(bottomTile) && markBottomTilemap.GetTile(bottomTile) == null)
                markBottomTilemap.SetTile(bottomTile, markBottomTile);

            if (tilemaps[i].HasTile(rightTile) && markRightTilemap.GetTile(rightTile) == null)
                markRightTilemap.SetTile(rightTile, markRightTile);

            if (tilemaps[i].HasTile(leftTile) && markLeftTilemap.GetTile(leftTile) == null)
                markLeftTilemap.SetTile(leftTile, markLeftTile);
        }
    }



}
