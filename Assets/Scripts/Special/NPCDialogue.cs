using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCDialogue : MonoBehaviour
{
    public string[] dialogues; // Danh sách các câu thoại của NPC
    private int currentDialogueIndex = 0;

    private Camera mainCamera;
    public GameObject dialogueBox; // Hộp thoại UI
    public TextMeshProUGUI dialogueText; // Nội dung câu thoại
    public Transform dialogueAnchor; //Điển neo của hội thoại
    public GameObject interactionMark; // Biểu tượng cảnh báo "Press E"

    private bool isPlayerNearby = false; // Kiểm tra người chơi đã đến gần chưa
    public bool isInConversation = false; // Trạng thái hội thoại

    public Transform player; // Người chơi
    public float interactionDistance = 2f; // Khoảng cách tương tác

    private Player playerController; // Tham chiếu script điều khiển người chơi

    private void Start()
    {
        // Tắt hộp thoại ban đầu
        dialogueBox.SetActive(false);
        interactionMark.SetActive(false); // Tắt biểu tượng cảnh báo ban đầu
        playerController = player.GetComponent<Player>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Kiểm tra khoảng cách với người chơi
        float distance = Vector2.Distance(transform.position, player.position);
        isPlayerNearby = distance <= interactionDistance;

        // Hiển thị hộp thoại khi cần
        if (isInConversation)
        {
            UpdateDialogueBoxPosition();
        }
        if (isPlayerNearby)
        {
            if (!isInConversation)
                ShowInteractionMark();
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!isInConversation)
                    StartConversation();
                else
                    NextDialogue();
            }
        }
        else
            interactionMark.SetActive(false);

    }

    private void StartConversation()
    {
        isInConversation = true;
        dialogueBox.SetActive(true);
        interactionMark.SetActive(false);
        currentDialogueIndex = 0;
        ShowDialogue();
        playerController.LockMove(false); // Vô hiệu hóa di chuyển của người chơi
    }

    private void NextDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex < dialogues.Length)
        {
            ShowDialogue();
        }
        else
        {
            EndConversation();
        }
    }

    public void EndConversation()
    {
        isInConversation = false;
        dialogueBox.SetActive(false);
        playerController.UnlockMove(true); // Kích hoạt lại di chuyển
    }

    private void ShowDialogue()
    {
        dialogueText.text = dialogues[currentDialogueIndex];
    }

    private void UpdateDialogueBoxPosition()
    {
        // Chuyển đổi vị trí từ thế giới sang không gian màn hình
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(dialogueAnchor.position);
        dialogueBox.transform.position = screenPosition;
    }

    private void ShowInteractionMark()
    {
        interactionMark.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        // Vẽ khoảng cách tương tác
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
