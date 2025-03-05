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
            SaveAllItems();
            //Save inventory hien tai
            FindAnyObjectByType<InventoryManager>().SaveInventory();
            //Save các clone hiện tại
            SaveAllClones();
            //Save cac robot hiện tại
            SaveAllRobots();
            //Save trạng thái các quạt
            SaveAllFans();
            //Save trạng thái các block
            SaveAllBlocks();
            //Save trạng thái các cannon
            SaveAllCannons();
            //Save trạng thái tất cả laser
            SaveAllLasers();
            //Save trạng thái moving trap
            SaveAllMovingTraps();
            //Save trạng thái ConveyorBelt
            SaveAllConveyorBelts();
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
    private void SaveAllItems()
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
    private void SaveAllFans()
    {
        List<Fan> allFans = new List<Fan>(FindObjectsOfType<Fan>());
        foreach (Fan fan in allFans)
        {
            if (fan.disablePermanently && !fan.isFanActivated)
            {
                fan.savedActivationStatus = false;
            }
        }
    }
    private void SaveAllBlocks()
    {
        List<UnblockSwitch> allBlocks = new List<UnblockSwitch>(FindObjectsOfType<UnblockSwitch>());
        foreach (UnblockSwitch block in allBlocks)
        {
            if (block.blocker.activeSelf == false && block.isPermanent)
            {
                block.savedBlockStatus = false;
            }
        }
    }
    private void SaveAllCannons()
    {
        List<Cannon> allCannons = new List<Cannon>(FindObjectsOfType<Cannon>());
        foreach (Cannon cannon in allCannons)
        {
            if (cannon.disablePermanently && !cannon.isCannonActivated)
                cannon.savedActivationStatus = false;
        }
    }
    private void SaveAllLasers()
    {
        List<LaserEmitter> allLaserEmitter = new List<LaserEmitter>(FindObjectsOfType<LaserEmitter>());
        foreach(LaserEmitter emitter in allLaserEmitter)
        {
            if (emitter.disablePermanently && !emitter.isLaserActivate) 
                emitter.savedActivationStatus = false;
        }
    }
    private void SaveAllMovingTraps()
    {
        List<MovingTrap> movingTraps = new List<MovingTrap>(FindObjectsOfType<MovingTrap>());
        foreach (MovingTrap trap in movingTraps)
        {
            trap.savedPosition = trap.transform.position;
            if (!trap.isMovingActivated && trap.disablePermanently)
                trap.savedActivatonStatus = false;
            if (trap.isMovingActivated)
                trap.savedActivatonStatus = true;
        }
    }
    private void SaveAllConveyorBelts()
    {
        List<ConveyorBelt> allConveyorBelts = new List<ConveyorBelt>(FindObjectsOfType<ConveyorBelt>());
        foreach (ConveyorBelt belt in allConveyorBelts)
        {
            belt.savedActivationStatus = belt.isActive;
            belt.savedDirection = belt.moveRight;
        }
    }
}
