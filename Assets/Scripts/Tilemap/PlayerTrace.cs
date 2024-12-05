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
        // Phát hiện va chạm dưới chân Player để đặt dấu trên ô dưới
        PlaceMark();

        // Phát hiện va chạm bên trái và phải
        //PlaceMark(Vector2.left);
        //PlaceMark(Vector2.right);
    }

    void PlaceMark()
    {
        // Lấy tọa độ của Player (có offset)
        Vector3 topOffset = new Vector3(0, -1.5f, 0);
        Vector3 bottomOffset = new Vector3(0, 0.7f, 0);
        Vector3 rightOffset = new Vector3(-0.6f, 0, 0);
        Vector3 leftOffset = new Vector3(0.6f, 0, 0);
        Vector3 playerPosition = transform.position;

        // Chuyển đổi vị trí của Player (có offset) sang tọa độ của các Tile cần mark
        for (int i = 0; i < tilemaps.Count  ; i++)
        {
            Vector3Int topTilePosition = tilemaps[i].WorldToCell(playerPosition + topOffset);
            Vector3Int bottomTilePosition = tilemaps[i].WorldToCell(playerPosition + bottomOffset);
            Vector3Int rightTilePosition = tilemaps[i].WorldToCell(playerPosition + rightOffset);
            Vector3Int leftTilePosition = tilemaps[i].WorldToCell(playerPosition + leftOffset);

            // Kiểm tra xem cá tile có phải là ô thuộc lớp "Floor" không
            if (tilemaps[i].HasTile(topTilePosition) && tilemaps[i].GetTile(topTilePosition) != null)
            {
                // Đặt dấu vết lên Tile
                if (markTopTilemap.GetTile(topTilePosition) == null)
                {
                    markTopTilemap.SetTile(topTilePosition, markTopTile);
                }
            }

            if (tilemaps[i].HasTile(bottomTilePosition) && tilemaps[i].GetTile(bottomTilePosition) != null)
            {
                // Đặt dấu vết lên Tile
                if (markBottomTilemap.GetTile(bottomTilePosition) == null)
                {
                    markBottomTilemap.SetTile(bottomTilePosition, markBottomTile);
                }
            }

            if (tilemaps[i].HasTile(rightTilePosition) && tilemaps[i].GetTile(rightTilePosition) != null)
            {
                // Đặt dấu vết lên Tile
                if (markRightTilemap.GetTile(rightTilePosition) == null)
                {
                    markRightTilemap.SetTile(rightTilePosition, markRightTile);
                }
            }

            if (tilemaps[i].HasTile(leftTilePosition) && tilemaps[i].GetTile(leftTilePosition) != null)
            {
                // Đặt dấu vết lên Tile
                if (markLeftTilemap.GetTile(leftTilePosition) == null)
                {
                    markLeftTilemap.SetTile(leftTilePosition, markLeftTile);
                }
            }
        }
    }



}
