using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCDialogue : MonoBehaviour
{
    public string[] dialogues; // Danh sách các câu thoại của NPC
    private int currentDialogueIndex = 0;

    private GameObject dialogueBox; // Hộp thoại UI
    private TextMeshProUGUI dialogueText; // Nội dung câu thoại
    private GameObject interactionMark; // Biểu tượng cảnh báo "Press E"

    private bool isPlayerNearby = false; // Kiểm tra người chơi đã đến gần chưa
    public bool isInConversation = false; // Trạng thái hội thoại

    public float interactionDistance = 2f; // Khoảng cách tương tác

    private Player player; // Tham chiếu script điều khiển người chơi

    private void Start()
    {
        GameObject dialogueCanvas = transform.Find("Dialogue").gameObject;
        dialogueBox = dialogueCanvas.transform.Find("DialogueBox").gameObject;
        dialogueText = dialogueBox.transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();
        interactionMark = transform.Find("InteractionMark").gameObject;
        player = FindAnyObjectByType<Player>();

        // Tắt hộp thoại ban đầu
        dialogueBox.SetActive(false);
        interactionMark.SetActive(false); // Tắt biểu tượng cảnh báo ban đầu
    }
    private void Update()
    {
        // Kiểm tra khoảng cách với người chơi
        float distance = Vector2.Distance(transform.position, player.transform.position);
        isPlayerNearby = distance <= interactionDistance;

        // Hiển thị hộp thoại khi cần
        if (isPlayerNearby)
        {
            player.GetComponent<PlayerAbilities>().isNearNPC = true;
            if (!isInConversation)
                interactionMark.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!isInConversation)
                    StartConversation();
                else
                    NextDialogue();
            }
        }
        else
        {
            player.GetComponent<PlayerAbilities>().isNearNPC = false;
            interactionMark.SetActive(false);
        }
    }
    private void StartConversation()
    {
        player.GetComponent<PlayerAbilities>().isTalking = true;
        player.GetComponent<HealthComponent>().healthUI.enabled = false;
        GameTimer gameTimer = FindObjectOfType<GameTimer>();
        Canvas stageUI = GameObject.Find("General UI").transform.Find("StageUI").gameObject.GetComponent<Canvas>();
        gameTimer.PauseTimer();
        stageUI.enabled = false;

        isInConversation = true;
        dialogueBox.SetActive(true);
        interactionMark.SetActive(false);
        currentDialogueIndex = 0;
        ShowDialogue();
        player.LockMove(false); // Vô hiệu hóa di chuyển của người chơi
    }
    private void NextDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex < dialogues.Length)
            ShowDialogue();
        else
            EndConversation();
    }
    public void EndConversation()
    {
        player.GetComponent<PlayerAbilities>().isTalking = false;
        player.GetComponent<HealthComponent>().healthUI.enabled = true;
        GameTimer gameTimer = FindObjectOfType<GameTimer>();
        Canvas stageUI = GameObject.Find("General UI").transform.Find("StageUI").gameObject.GetComponent<Canvas>();
        gameTimer.ResumeTimer();
        stageUI.enabled = true;

        isInConversation = false;
        dialogueBox.SetActive(false);
        player.UnlockMove(true); // Kích hoạt lại di chuyển
    }
    private void ShowDialogue() => dialogueText.text = dialogues[currentDialogueIndex];


    private void OnDrawGizmos()
    {
        // Vẽ khoảng cách tương tác
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
