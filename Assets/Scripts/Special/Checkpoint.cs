using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool isActivated = false;
    Animator animator;
    private List<ItemBase> allItems;
    [SerializeField] private List<Robot> allRobots;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        allItems = new List<ItemBase>(FindObjectsOfType<ItemBase>());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().startPosition = transform.position + Vector3.up * 3f + Vector3.right * 1.5f;
            isActivated = true;
            animator.SetBool("isActivated", isActivated);

            //Tất cả các item đã bị ăn (inactive) sẽ được set isCheckpointPicked = true;
            CheckpointItem();
            //Save inventory hien tai
            FindAnyObjectByType<InventoryManager>().SaveInventory();
            //Save các clone hiện tại
            SaveAllClones();
            //Save cac robot hiện tại
            SaveAllRobots();
        }
    }

    private void SaveAllClones()
    {
        List<SlimeClone> allClones = new List<SlimeClone>(FindObjectsOfType<SlimeClone>());
        foreach (SlimeClone clone in allClones)
        {
            clone.SaveClone();
        }
    }
    private void CheckpointItem()
    {
        foreach (ItemBase item in allItems)
        {
            if (!item.gameObject.activeSelf)
                item.PickToSaveInventory();
        }
    }
    private void SaveAllRobots()
    {
        List<Robot> allRobots = new List<Robot>(FindObjectsOfType<Robot>());
        foreach (Robot robot in allRobots)
        {
            robot.SaveRobotStatus();
        }
    }
}
