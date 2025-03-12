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
    private void Awake()
    {
        animator = GetComponent<Animator>();
        allItems = new List<ItemBase>(FindObjectsOfType<ItemBase>());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!isActivated)
            {
                //Save time
                SaveTime();
                //Save info player
                SavePlayer(collision.gameObject);
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
                //Save trạng thái ConveyorBelt
                SaveAllConveyorBelts();
                //Save trạng thái các block
                SaveAllBlocks();
                //Save trạng thái các cannon
                SaveAllCannons();
                //Save trạng thái tất cả laser
                SaveAllLasers();
                //Save trạng thái Gun
                SaveAllGun();
                //Save trạng thái moving trap
                SaveAllMovingTraps();

                //Save trạng thái Enemy
                SaveAllEnemy();
            }
            ////Bật checkpoint nào được chạm vào vào tắt các checkpoint khác (viết sau cùng)
            SaveCheckpoint();
        }
    }
    private void SaveCheckpoint()
    {
        List<Checkpoint> checkpointList = new List<Checkpoint>(FindObjectsOfType<Checkpoint>());
        foreach (Checkpoint checkpoint in checkpointList)
        {
            checkpoint.isActivated = false;
            checkpoint.animator.SetBool("isActivated", checkpoint.isActivated);
        }
        isActivated = true;
        animator.SetBool("isActivated", isActivated);
    }
    private void SaveTime()
    {
        FindObjectOfType<GameTimer>().savedTime = FindObjectOfType<GameTimer>().elapsedTime;
    }
    private void SavePlayer(GameObject player)
    {
        player.GetComponent<Player>().startPosition = transform.position + Vector3.up * 3f + Vector3.right * 1.5f;
        List<HealthComponent> healthComponents = new List<HealthComponent>(FindObjectsOfType<HealthComponent>());
        foreach (HealthComponent health in healthComponents)
        {
            health.savedCurrentHealth = health.currentHealth;
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
            if (block.isPermanent)
            {
                block.savedBlockStatus = block.blocker.activeSelf;
            }
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
        foreach (LaserEmitter emitter in allLaserEmitter)
        {
            if (emitter.disablePermanently && !emitter.isLaserActivate)
                emitter.savedActivationStatus = false;
            emitter.savedLaserAngle = emitter.laserAngle;
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
    private void SaveAllGun()
    {
        List<GunTrap> gunTraps = new List<GunTrap>(FindObjectsOfType<GunTrap>());
        foreach (GunTrap gun in gunTraps)
        {
            gun.savedBulletAngle = gun.bulletAngle;
        }
    }
    private void SaveAllEnemy()
    {
        List<Enemy> enemies = new List<Enemy>(FindObjectsOfType<Enemy>());
        foreach (Enemy enemy in enemies)
        {
            enemy.savedDeadStatus = enemy.isEnemyDead;
            enemy.savedPosition = enemy.transform.position;

        }
    }
}
