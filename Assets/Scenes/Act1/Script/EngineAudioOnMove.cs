using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EngineAudioOnMove : MonoBehaviour
{
    [Header("Loop 사운드")]
    public AudioClip engineLoop;
    [Range(0f, 1f)] public float maxVolume = 1f;   // 2D면 감쇠 없으니 1 권장
    public float minPitch = 0.95f;
    public float maxPitch = 1.35f;

    [Header("페이드")]
    public float fadeIn = 0.15f;
    public float fadeOut = 0.25f;

    AudioSource _src;
    MoveCar _move;
    float _volVel;

    void Awake()
    {
        _move = GetComponent<MoveCar>();
        _src = GetComponent<AudioSource>();

        _src.playOnAwake = false;
        _src.loop = true;

        // === 완전 2D 설정 ===
        _src.spatialBlend = 0f;     // 2D (거리 감쇠/위치 무시)
        _src.dopplerLevel = 0f;     // 도플러 효과 끔
        // rolloffMode/minDistance/maxDistance 등은 2D에선 무시됨

        _src.volume = 0f;

        if (engineLoop != null) _src.clip = engineLoop;
    }

    void Update()
    {
        bool driving = _move != null && _move.enabled && _move.eventNum == 1 && _move.speed > 0.01f;

        if (driving)
        {
            if (!_src.isPlaying && _src.clip != null)
                _src.Play();

            // 속도(0~1 가정)로 피치 변화
            float speedNorm = Mathf.Clamp01(_move.speed);
            _src.pitch = Mathf.Lerp(minPitch, maxPitch, speedNorm);

            _src.volume = Mathf.SmoothDamp(_src.volume, maxVolume, ref _volVel, fadeIn);
        }
        else
        {
            _src.volume = Mathf.SmoothDamp(_src.volume, 0f, ref _volVel, fadeOut);
            if (_src.isPlaying && _src.volume <= 0.01f)
                _src.Stop();
        }
    }

    public void StopNow()
    {
        _src.Stop();
        _src.volume = 0f;
    }
}
