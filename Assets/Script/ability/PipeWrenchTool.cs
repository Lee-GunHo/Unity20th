using UnityEngine;

public class PipeWrenchTool : MonoBehaviour, IToolUsable
{
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask nutMask;

    private PlayerToolController owner;
    private Transform cam;
    private bool isEquipped = false;
    [Header("사운드")]
    public AudioClip useClip;
    [Range(0f, 1f)] public float useVolume = 1f;
    public Transform sfxPoint;

    // ───────── IToolUsable ─────────

    public void OnEquip(PlayerToolController owner)
    {
        this.owner = owner;
        cam = Camera.main != null ? Camera.main.transform : null;
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
        if (cam == null)
        {
            var main = Camera.main;
            if (main == null) return;
            cam = main.transform;
        }

        Ray ray = new Ray(cam.position, cam.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, nutMask))
        {
            var nut = hit.collider.GetComponent<LooseNut>();
            if (nut != null)
            {
                nut.Tighten();
                PlayUseSfx();
                Debug.Log("[PipeWrench] 나사를 조였습니다.");
                // TODO: 애니메이션 / 사운드
            }
        }
    }
    private void PlayUseSfx()
    {
        if (useClip == null) return;
        Vector3 pos = sfxPoint ? sfxPoint.position : transform.position;
        AudioSource.PlayClipAtPoint(useClip, pos, useVolume);
    }
}
