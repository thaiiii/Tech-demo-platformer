using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class BreakableTileManager : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile[] crackStates;
    public float timeToBreak = 0.5f;
    public float respawnDelay = 5f;
    public LayerMask tileLayer;

    private Dictionary<Vector3Int, Coroutine> activeBreakTimers = new Dictionary<Vector3Int, Coroutine>();      //Quản lý các block đang bị phá
    private List<Vector3Int> brokenTiles = new List<Vector3Int>();      //Lưu các block đã bị phá


    private void Update()
    {
        DetectPlayerCollisionWithTiles();
    }

    private void DetectPlayerCollisionWithTiles()
    {
        Player player = FindAnyObjectByType<Player>();
        Vector3 playerPosition = player.transform.position; // Vị trí của người chơi
        float rayLength = 0.7f; // Độ dài của tia Raycast

        // Phóng một tia xuống dưới từ vị trí người chơi (hoặc bạn có thể phóng theo hướng khác)
        RaycastHit2D hitDownward = Physics2D.Raycast(playerPosition, Vector3.down, rayLength, tileLayer);
        RaycastHit2D hitForward = Physics2D.Raycast(playerPosition, Vector3.right * player.transform.localScale.x, rayLength, tileLayer);
        //Debug.DrawLine(playerPosition, playerPosition + Vector3.down * rayLength);
        //Debug.DrawLine(playerPosition, playerPosition + Vector3.right * player.transform.localScale.x * rayLength);

        if (hitDownward.collider != null)
        {
            // Tính toán vị trí tile trong tilemap
            Vector3Int tilePos = tilemap.WorldToCell(hitDownward.point);
            
            // Kiểm tra tile và chưa xử lý, bắt đầu phá hủy tile
            if (tilemap.HasTile(tilePos) && !activeBreakTimers.ContainsKey(tilePos))
            {
                Coroutine breakCoroutine = StartCoroutine(BreakTileAfterDelay(tilePos));
                activeBreakTimers[tilePos] = breakCoroutine;
            }
        }

        if (hitForward.collider != null)
        {
            // Tính toán vị trí tile trong tilemap
            Vector3Int tilePos = tilemap.WorldToCell(hitForward.point);

            // Kiểm tra tile và chưa xử lý, bắt đầu phá hủy tile
            if (tilemap.HasTile(tilePos) && !activeBreakTimers.ContainsKey(tilePos))
            {
                Coroutine breakCoroutine = StartCoroutine(BreakTileAfterDelay(tilePos));
                activeBreakTimers[tilePos] = breakCoroutine;
            }
        }
    }

    private IEnumerator BreakTileAfterDelay(Vector3Int tilePos)
    {
        //Hiển thị trạng thái nứt
        for (int i = 0; i < crackStates.Length; i++)
        {
            yield return new WaitForSeconds(timeToBreak / crackStates.Length);
            tilemap.SetTile(tilePos, crackStates[i]);
        }

        //Xóa block sau khi nứt hoàn toàn 
        tilemap.SetTile(tilePos, null);
        brokenTiles.Add(tilePos);
        activeBreakTimers.Remove(tilePos);

        //Hồi phục block sau thời gian chờ
        yield return new WaitForSeconds(respawnDelay);
        RestoreTile(tilePos);

    
    }

    private void RestoreTile(Vector3Int tilePos)
    {
        //Khôi phục block về trạng thái ban đầu
        if(!tilemap.HasTile(tilePos)) //Kiểm tra nếu tọa độ nó trống
        {
            tilemap.SetTile(tilePos, crackStates[0]);
            brokenTiles.Remove(tilePos);
        }
    
    }


    public void ResetAllTile()
    {
        //Dừng mọi coroutine trong script này 
        foreach (var kvp in activeBreakTimers)
        {
            StopCoroutine(kvp.Value);
        }
        activeBreakTimers.Clear();

        //Khôi phục tất cả block đã bị phá
        foreach (var tilePos in brokenTiles)
        {
            RestoreTile(tilePos);
        }
        brokenTiles.Clear();
    }

    
}
