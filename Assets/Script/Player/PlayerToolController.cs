using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerToolController : MonoBehaviour
{
    [Header("Hand Socket")]
    [SerializeField] private Transform handSocket;          // 손 위치(프리팹 부모)

    [Header("Input")]
    [SerializeField] private InputActionReference useToolAction; // 사용 키 (마우스 좌클릭 등)

    private IToolUsable currentTool;

    private void OnEnable()
    {
        if (useToolAction != null)
        {
            useToolAction.action.performed += OnUseTool;
            useToolAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (useToolAction != null)
        {
            useToolAction.action.performed -= OnUseTool;
            useToolAction.action.Disable();
        }
    }

    private void OnUseTool(InputAction.CallbackContext ctx)
    {
        currentTool?.Use();
    }

    // 인벤토리가 "이 도구를 들었다"라고 알려줄 때 호출
    public void EquipExistingTool(IToolUsable tool)
    {
        if (currentTool == tool) return;

        if (currentTool != null)
            currentTool.OnUnequip();

        currentTool = tool;

        if (currentTool != null)
            currentTool.OnEquip(this);
    }

    public void ClearTool()
    {
        if (currentTool != null)
            currentTool.OnUnequip();

        currentTool = null;
    }

    // 필요하면 프리팹 직접 장착용도 유지 가능
    public void EquipToolPrefab(GameObject toolPrefab)
    {
        ClearTool();

        if (toolPrefab == null || handSocket == null)
            return;

        var instance = GameObject.Instantiate(toolPrefab, handSocket);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;

        var tool = instance.GetComponent<IToolUsable>();
        if (tool == null)
        {
            Debug.LogError("[PlayerToolController] 프리팹에 IToolUsable이 없습니다.");
            return;
        }

        currentTool = tool;
        currentTool.OnEquip(this);
    }

    public Transform HandSocket => handSocket;
}
