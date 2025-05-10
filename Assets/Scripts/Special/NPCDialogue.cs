using UnityEngine;
using TMPro;
using System.Collections;

public class NPCDialogue : MonoBehaviour
{
    public string[] dialogues; // Danh sách các câu thoại của NPC
    private int currentDialogueIndex = 0;

    private GameObject dialogueCanvas;
    private GameObject dialogueBox; // Hộp thoại UI
    private TextMeshProUGUI dialogueText; // Nội dung câu thoại
    private GameObject interactionMark; // Biểu tượng cảnh báo "Press E"

    private bool isPlayerNearby = false; // Kiểm tra người chơi đã đến gần chưa
    public bool isInConversation = false; // Trạng thái hội thoại
    private bool canProceed = true;

    public float interactionDistance = 2f; // Khoảng cách tương tác

    private PlayerAbilities player; // Tham chiếu script điều khiển người chơi

    private void Start()
    {
        dialogueCanvas = transform.Find("Dialogue").gameObject;
        dialogueBox = dialogueCanvas.transform.Find("DialogueBox").gameObject;
        dialogueText = dialogueBox.transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();
        interactionMark = transform.Find("InteractionMark").gameObject;
        // Tắt hộp thoại ban đầu
        dialogueBox.SetActive(false);
        interactionMark.SetActive(false); // Tắt biểu tượng cảnh báo ban đầu
    }
    private void Update()
    {
        if (player != null)
        {
            // Hiển thị hộp thoại khi cần
            if (isPlayerNearby)
            {
                player.isNearNPC = true;
                if (!isInConversation)
                    interactionMark.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E) && player.CanTalkToNPC() && canProceed)
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
                player = null;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        player = collision.gameObject.GetComponent<PlayerAbilities>();
        isPlayerNearby = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        isPlayerNearby=false;
    }


    private void StartConversation()
    {
        player.GetComponent<Animator>().SetFloat("xVelocity", 0f) ;
        StartCoroutine(DelayNextDialog());
        AudioManager.Instance.PlaySFX("mumble");
        if (player.transform.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            dialogueCanvas.GetComponent<RectTransform>().localScale = new Vector3(
                -Mathf.Abs(dialogueCanvas.GetComponent<RectTransform>().localScale.x),
                dialogueCanvas.GetComponent<RectTransform>().localScale.y,
                dialogueCanvas.GetComponent<RectTransform>().localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            dialogueCanvas.GetComponent<RectTransform>().localScale = new Vector3(
                Mathf.Abs(dialogueCanvas.GetComponent<RectTransform>().localScale.x),
                dialogueCanvas.GetComponent<RectTransform>().localScale.y,
                dialogueCanvas.GetComponent<RectTransform>().localScale.z);
        }
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
        player.GetComponent<Player>().LockMove(false); // Vô hiệu hóa di chuyển của người chơi
    }
    private void NextDialogue()
    {
        StartCoroutine(DelayNextDialog());
        AudioManager.Instance.PlaySFX("mumble");
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
        player.GetComponent<Player>().UnlockMove(true); // Kích hoạt lại di chuyển
    }
    private void ShowDialogue() => dialogueText.text = dialogues[currentDialogueIndex];
    IEnumerator DelayNextDialog()
    {
        canProceed = false;
        yield return new WaitForSeconds(1.1f);
        canProceed = true;
    }
}
