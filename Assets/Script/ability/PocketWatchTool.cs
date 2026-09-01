using UnityEngine;
using UnityEngine.InputSystem; // 클릭 입력용

public class PocketWatchTool : MonoBehaviour, IToolUsable
{
    [SerializeField] private float cooldown = 1.5f;

    [Header("사운드")]
    public AudioClip useClip;
    [Range(0f, 1f)] public float useVolume = 1f;
    public Transform sfxPoint;

    private PlayerToolController owner;
    private float nextAvailableTime;
    private bool isEquipped;

    public void OnEquip(PlayerToolController owner)
    {
        this.owner = owner;
        isEquipped = true;
        gameObject.SetActive(true);
    }

    public void OnUnequip()
    {
        isEquipped = false;
        owner = null;
        gameObject.SetActive(false);
    }

    public void Use()
    {
        if (!isEquipped) return;
        if (Time.time < nextAvailableTime) return;
        if (TimeManager.Instance == null) return;

        // 좌/우 클릭 감지
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // 시간 되돌리기
            TimeManager.Instance.PrevPhase();
            Debug.Log("[PocketWatch] 왼쪽 클릭 → 시간 되돌리기");
            PlayUseSfx();
            StartCooldown();
        }
        else if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            // 시간 앞으로
            TimeManager.Instance.NextPhase();
            Debug.Log("[PocketWatch] 오른쪽 클릭 → 시간 앞으로");
            PlayUseSfx();
            StartCooldown();
        }
    }

    private void StartCooldown()
    {
        nextAvailableTime = Time.time + cooldown;
    }

    private void PlayUseSfx()
    {
        if (useClip == null) return;
        Vector3 pos = Camera.main ? Camera.main.transform.position : (sfxPoint ? sfxPoint.position : transform.position);
        AudioSource.PlayClipAtPoint(useClip, pos, useVolume);
    }
}
