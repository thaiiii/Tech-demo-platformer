using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoal : MonoBehaviour
{
    private LevelCompleteMenu levelCompleteMenu;

    private void Awake()
    {
        levelCompleteMenu = FindAnyObjectByType<LevelCompleteMenu>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            levelCompleteMenu.CompleteLevel();
        }
    }
}
