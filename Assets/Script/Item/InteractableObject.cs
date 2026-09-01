using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("Trigger Settings")]
    public string playerTag = "Player";

    [Header("Teleport Settings")]
    [Tooltip("이곳이 Yard로 가는 포탈이라면 체크하세요.")]
    public bool inYard = false; // 'public'으로 변경
    public Vector3 yardPos = new Vector3(994, 4, 510);
    public Vector3 gasPos = new Vector3(358.5f, 4, 179);

    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.E;

    private bool isPlayerInRange = false;
    // 플레이어의 CharacterController를 저장할 변수
    private CharacterController playerController;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = true;
            // 플레이어의 CharacterController 컴포넌트를 가져옴
            playerController = other.GetComponent<CharacterController>();

            // 디버그 메시지가 실제 키와 일치하도록 수정
            Debug.Log($"플레이어가 범위 안으로 들어옴. {interactKey}키로 상호작용 가능.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = false;
            // 플레이어가 나가면 참조를 비움
            playerController = null;
            Debug.Log("플레이어가 범위를 벗어남.");
        }
    }

    void Update()
    {
        // playerController가 null이 아닐 때만 (플레이어가 범위 안에 있고, CC가 있을 때)
        if (isPlayerInRange && playerController != null && Input.GetKeyDown(interactKey))
        {
            Vector3 targetPosition;

            if (inYard)
            {
                targetPosition = yardPos;
                inYard = false;
            }
            else
            {
                targetPosition = gasPos;
                inYard = true;
            }

            // CharacterController를 이용한 안전한 텔레포트
            playerController.enabled = false; // 컨트롤러 잠시 끄기
            playerController.transform.position = targetPosition; // 위치 강제 설정
            playerController.enabled = true;  // 컨트롤러 다시 켜기

            Debug.Log("플레이어 텔레포트: " + targetPosition);
        }
    }
}