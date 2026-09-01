using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactableMask = ~0;

    private Camera cam;

    // 현재 보고 있는 상호작용 대상
    private IInteractable currentTarget;

    // 하이라이트용
    private OutlineHighlighter currentHighlight;

    // 대화 종료 직후 중복 입력 방지
    private float blockUntil = 0f;

    // ───────── 아이템 상태 ─────────
    public bool HasPipeWrench { get; private set; }

    public IInteractable CurrentTarget => currentTarget;

    private void Awake()
    {
        cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("[PlayerInteraction] Main Camera를 찾을 수 없습니다. 카메라에 MainCamera 태그가 있는지 확인하세요.");
        }
    }

    private void Update()
    {
        DetectInteractable();
    }

    /// <summary>
    /// 화면 중앙에서 Raycast로 IInteractable을 찾고,
    /// 대상이 바뀔 때마다 하이라이트를 켜고/끄는 함수.
    /// </summary>
    private void DetectInteractable()
    {
        if (cam == null)
        {
            ClearHighlight();
            currentTarget = null;
            return;
        }

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableMask))
        {
            // 충돌 콜라이더에서 IInteractable 찾기 (부모까지 포함)
            if (hit.collider.TryGetComponent(out IInteractable interactable) ||
                hit.collider.GetComponentInParent<IInteractable>() != null)
            {
                if (interactable == null)
                    interactable = hit.collider.GetComponentInParent<IInteractable>();

                // 타겟이 바뀐 경우에만 하이라이트 갱신
                if (currentTarget != interactable)
                {
                    ClearHighlight();

                    currentTarget = interactable;

                    // 같은 오브젝트 / 부모에서 OutlineHighlighter 찾기
                    currentHighlight =
                        hit.collider.GetComponent<OutlineHighlighter>() ??
                        hit.collider.GetComponentInParent<OutlineHighlighter>();

                    if (currentHighlight != null)
                        currentHighlight.SetHighlight(true);
                }

                return;
            }
        }

        // 레이캐스트 실패 또는 IInteractable 아님 → 하이라이트 해제
        ClearHighlight();
        currentTarget = null;
    }

    /// <summary>
    /// 현재 하이라이트 끄기
    /// </summary>
    private void ClearHighlight()
    {
        if (currentHighlight != null)
        {
            currentHighlight.SetHighlight(false);
            currentHighlight = null;
        }

        currentTarget = null;
    }

    /// <summary>
    /// F키 등 상호작용 입력 콜백 (Input System에서 연결)
    /// </summary>
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // 0) 상점 열려 있으면: F = 상점 닫기
        if (ShopUI.Instance != null && ShopUI.Instance.IsOpen)
        {
            ShopUI.Instance.Close();
            Debug.Log("[PlayerInteraction] 상점 닫기");
            return;
        }

        // 너무 빠른 중복 입력 방지
        if (Time.time < blockUntil)
            return;

        var ui = DialogueUI.Instance;

        Debug.Log(
            $"[OnInteract] ui={(ui != null)}, " +
            $"playing={(ui != null && ui.IsPlaying)}, " +
            $"rootOn={(ui != null && ui.Root != null && ui.Root.activeSelf)}"
        );

        // 1) 대화창이 떠 있으면: 다음 줄 or 종료
        if (ui != null && (ui.IsPlaying || (ui.Root != null && ui.Root.activeSelf)))
        {
            ui.NextLine();

            // 이 입력으로 대화 종료되면 잠시 쿨타임
            if (!ui.IsPlaying && (ui.Root == null || !ui.Root.activeSelf))
            {
                blockUntil = Time.time + 0.2f;
            }

            return;
        }

        // 2) 평상시: 보고 있는 대상과 상호작용
        if (currentTarget != null)
        {
            Debug.Log("[PlayerInteraction] 대상과 상호작용 시도");
            currentTarget.Interact(this);
        }
        else
        {
            Debug.Log("[PlayerInteraction] 대상이 없습니다.");
        }
    }

    // ───────── 파이프렌치 관련 메서드 ─────────
    public void GivePipeWrench()
    {
        HasPipeWrench = true;
        Debug.Log("[PlayerInteraction] 파이프렌치 획득!");
        // 여기서 인벤토리 UI, 아이콘 표시 등 추가 가능
    }

    public void RemovePipeWrench()
    {
        HasPipeWrench = false;
        Debug.Log("[PlayerInteraction] 파이프렌치 사용 종료/버림");
    }
}
