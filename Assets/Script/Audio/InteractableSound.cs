using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class InteractableSound : MonoBehaviour, IInteractable
{
    [Header("UI Prompt")]
    [SerializeField] private string promptText = "[E] 상호작용";
    public string GetPromptText() => promptText;   // IInteractable 구현

    [Header("Clips")]
    public AudioClip[] clips;
    [Range(0f, 1f)] public float volume = 0.9f;
    [Range(0f, 0.5f)] public float pitchRandomRange = 0.05f;
    public bool preventOverlap = true;     // 재생 중 중복 방지
    public float cooldown = 0f;            // 연타 방지

    [Header("3D Settings")]
    public bool use3D = true;
    public float minDistance = 1f;
    public float maxDistance = 35f;

    [Header("Optional: 부분재생(트림)")]
    public bool useTrim = false;
    public float startSec = 0f;            // 예: 0.00
    public float durationSec = 0.15f;      // 예: 0.15초만 재생

    private AudioSource _src;
    private float _lastPlayTime = -999f;

    void Awake()
    {
        _src = GetComponent<AudioSource>();
        _src.playOnAwake = false;
        _src.loop = false;

        if (use3D)
        {
            _src.spatialBlend = 1f;
            _src.rolloffMode = AudioRolloffMode.Logarithmic;
            _src.minDistance = minDistance;
            _src.maxDistance = maxDistance;
        }
        else
        {
            _src.spatialBlend = 0f; // 2D
        }
    }

    // IInteractable 구현: PlayerInteraction은 컨텍스트용 파라미터(사용 안 해도 됨)
    public void Interact(PlayerInteraction interactor)
    {
        if (Time.time < _lastPlayTime + cooldown) return;
        if (clips == null || clips.Length == 0) return;

        int idx = Random.Range(0, clips.Length);
        AudioClip clip = clips[idx];

        float jitter = Random.Range(-pitchRandomRange, pitchRandomRange);
        _src.pitch = Mathf.Clamp(1f + jitter, 0.5f, 1.5f);

        if (useTrim)
        {
            StartCoroutine(PlayTrimmed(clip, Mathf.Max(0f, startSec), Mathf.Max(0.01f, durationSec)));
        }
        else
        {
            if (preventOverlap && _src.isPlaying) return;
            _src.PlayOneShot(clip, volume);
        }

        _lastPlayTime = Time.time;
    }

    private IEnumerator PlayTrimmed(AudioClip clip, float start, float length)
    {
        if (preventOverlap && _src.isPlaying) yield break;

        _src.clip = clip;
        _src.time = Mathf.Clamp(start, 0f, Mathf.Max(0f, clip.length - 0.01f));
        _src.volume = volume;
        _src.Play();
        yield return new WaitForSeconds(Mathf.Min(length, Mathf.Max(0f, clip.length - _src.time)));
        _src.Stop();
        _src.clip = null;
    }
}
