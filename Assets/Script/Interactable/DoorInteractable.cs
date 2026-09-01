using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    private bool isOpen = false;

    [Header("SFX")]
    public AudioClip[] openClips;
    public AudioClip[] closeClips;
    [Range(0f, 1f)] public float volume = 0.9f;
    [Range(0f, 0.5f)] public float pitchRandomRange = 0.03f;
    public bool stopIfPlaying = true;

    private AudioSource _src;

    void Awake()
    {
        _src = GetComponent<AudioSource>();
        if (_src == null) _src = gameObject.AddComponent<AudioSource>();

        _src.playOnAwake  =false;
        _src.loop = false;
        _src.dopplerLevel = 0f;

    }
    public string GetPromptText()
    {
        return isOpen ? "F 키: 문 닫기" : "F 키: 문 열기";
    }

    public void Interact(PlayerInteraction player)
    {
        isOpen = !isOpen;
        // 여기에 애니메이션 / 회전 / 사운드
        Debug.Log(isOpen ? "[Door] 문 열림" : "[Door] 문 닫힘");
    }
    private void PlayDoorSfx(bool opened)
    {
        var pool = opened ? openClips : closeClips;
        if (pool == null || pool.Length == 0)
        {
            return;
        }

        var clip = pool[Random.Range(0, pool.Length)];
        if (clip == null) return;

        if (stopIfPlaying && _src.isPlaying)
            _src.Stop();

        float jitter = Random.Range(-pitchRandomRange, pitchRandomRange);
        _src.pitch = Mathf.Clamp(1f + jitter, 0.5f,1.5f);
        _src.PlayOneShot(clip, volume);
    }
}
