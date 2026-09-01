using UnityEngine;
using TMPro;
using System;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject root;            // 말풍선 패널(comnu)
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI bodyText;

    private string[] lines;
    private int index;
    private bool isPlaying;
    private Action onDialogueEnd;                        // 대화 종료 후 실행할 콜백


    public bool IsPlaying => isPlaying;
    public GameObject Root => root;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (root != null)
            root.SetActive(false);
    }

    public void StartDialogue(string npcName, string[] dialogueLines, Action onFinished = null)
    {
        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            // 대사가 없으면 바로 콜백만 실행
            onFinished?.Invoke();
            return;
        }

        lines = dialogueLines;
        index = 0;
        isPlaying = true;
        onDialogueEnd = onFinished;

        if (root != null)
            root.SetActive(true);

        if (nameText != null)
            nameText.text = npcName;

        if (bodyText != null)
            bodyText.text = lines[0];

        Debug.Log($"[DialogueUI] 대화 시작: {npcName}, 줄 수: {lines.Length}");
    }

    public void NextLine()
    {
        if (!isPlaying || lines == null) return;

        index++;

        if (index >= lines.Length)
        {
            EndDialogue();
            return;
        }

        if (bodyText != null)
            bodyText.text = lines[index];

        Debug.Log($"[DialogueUI] NextLine, index={index}/{lines.Length}");
    }

    public void EndDialogue()
    {
        if (!isPlaying) return;

        Debug.Log("[DialogueUI] 대화 종료");
        isPlaying = false;

        // 말풍선(comnu) 끄기
        if (root != null)
            root.SetActive(false);

        // 상태 초기화
        var cb = onDialogueEnd;
        onDialogueEnd = null;
        lines = null;
        index = 0;

        // 대화 완전 종료 후 상점 열기 등 콜백 실행
        cb?.Invoke();
    }
}
