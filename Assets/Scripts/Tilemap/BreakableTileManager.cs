using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BreakableTileManager : MonoBehaviour
{
    private Tilemap tilemap;
    public Tile[] crackStates; // Các trạng thái nứt
    public float timeToBreak = 1f; // Thời gian để vỡ hoàn toàn
    public float respawnDelay = 3f; // Thời gian hồi phục
    public LayerMask tileLayer; // Layer của tilemap

    private Dictionary<Vector3Int, Coroutine> breakingTiles = new Dictionary<Vector3Int, Coroutine>();
    private Transform playerTransform;
    private Collider2D playerCollider;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        Player player = FindAnyObjectByType<Player>();
        if (player != null)
        {
            playerTransform = player.transform;
            playerCollider = player.GetComponent<Collider2D>();
        }
    }

    private void FixedUpdate()
    {
        DetectPlayerOnTile();
    }

    private void DetectPlayerOnTile()
    {
        if (playerTransform == null || playerCollider == null) return;

        // Lấy vị trí chính xác của chân người chơi
        Vector3 frontPosition = new Vector3(playerTransform.position.x + playerCollider.bounds.size.x /2f , playerCollider.bounds.min.y - 0.1f, 0);
        Vector3 backPosition = new Vector3(playerTransform.position.x - playerCollider.bounds.size.x /2f , playerCollider.bounds.min.y - 0.1f, 0);

        // Chuyển vị trí chân về tọa độ trong tilemap
        Vector3Int frontTilePos = tilemap.WorldToCell(frontPosition);
        Vector3Int backTilePos = tilemap.WorldToCell(backPosition);
        // Nếu tile có tồn tại và chưa trong danh sách vỡ thì bắt đầu quá trình
        if (tilemap.HasTile(frontTilePos) && !breakingTiles.ContainsKey(frontTilePos))
        {
            Coroutine coroutine = StartCoroutine(BreakTile(frontTilePos));
            breakingTiles[frontTilePos] = coroutine;
        } 
        if (tilemap.HasTile(backTilePos) && !breakingTiles.ContainsKey(backTilePos))
        {
            Coroutine coroutine = StartCoroutine(BreakTile(backTilePos));
            breakingTiles[backTilePos] = coroutine;
        }
    }

    private IEnumerator BreakTile(Vector3Int tilePos)
    {
        for (int i = 0; i < crackStates.Length; i++)
        {
            yield return new WaitForSeconds(timeToBreak / crackStates.Length);
            tilemap.SetTile(tilePos, crackStates[i]);
        }

        // Xóa tile sau khi nứt hoàn toàn
        tilemap.SetTile(tilePos, null);
        breakingTiles.Remove(tilePos);

        yield return new WaitForSeconds(respawnDelay);
        RestoreTile(tilePos);
    }

    private void RestoreTile(Vector3Int tilePos)
    {
        tilemap.SetTile(tilePos, crackStates[0]);
    }
}
