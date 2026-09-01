using UnityEngine;
using System.Collections;

public enum TimePhase
{
    Morning,   // 9��
    Noon,      // 15��
    Evening,   // 19��
    EventTime  // �޺� ���� (3~4�� ����)
}

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public TimePhase CurrentPhase { get; private set; } = TimePhase.Morning;

    [Header("State Management")]
    [Tooltip("시간대에 따라 오브젝트를 켜고 끌 TimeState 스크립트")]
    public TimeState timeStateController;

    [Header("Light Settings")]
    public Light directionalLight;

    public Color morningColor = new Color(1.0f, 0.95f, 0.8f);
    public Color noonColor = new Color(1.0f, 1.0f, 0.9f);
    public Color eveningColor = new Color(1.0f, 0.6f, 0.4f);
    public Color eventColor = new Color(0.25f, 0.3f, 0.45f); // �޺� ��

    public float morningIntensity = 0.9f;
    public float noonIntensity = 1.2f;
    public float eveningIntensity = 0.5f;
    public float eventIntensity = 0.15f;

    [Header("Skybox (���ϸ� ���)")]
    public Material morningSkybox;
    public Material noonSkybox;
    public Material eveningSkybox;
    public Material eventSkybox;

    [Header("Stars & Effects")]
    public GameObject stars;
    public GameObject eventPostVolume;

    [Header("BGM (����)")]
    public AudioSource bgmSource;
    public AudioClip morningBgm;
    public AudioClip noonBgm;
    public AudioClip eveningBgm;
    public AudioClip eventBgm;

    [Header("Transition")]
    public float transitionDuration = 5f; // 5�� ���� ������ ����

    private Coroutine transitionRoutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (directionalLight == null)
            directionalLight = FindAnyObjectByType<Light>();

        if (timeStateController == null)
        {
            timeStateController = Object.FindAnyObjectByType<TimeState>();
            if (timeStateController == null)
                Debug.LogWarning("TimeState 스크립트를 씬에서 찾을 수 없습니다. 인스펙터에 연결해주세요.");
        }

        OnPhaseChanged();
    }

    public void NextPhase()
    {
        CurrentPhase = (TimePhase)(((int)CurrentPhase + 1) % 4);
        OnPhaseChanged();
    }

    public void PrevPhase()
    {
        CurrentPhase = (TimePhase)(((int)CurrentPhase + 3) % 4);
        OnPhaseChanged();
    }

    private void OnPhaseChanged()
    {
        // 환경(빛, 하늘)이 변하기 *전에* // TimeState 스크립트의 UpdateState 함수를 호출하여
        // 오브젝트부터 켜고 끕니다.
        if (timeStateController != null)
        {
            timeStateController.UpdateState(CurrentPhase);
        }
        else
        {
            Debug.LogWarning("TimeManager에 TimeState 컨트롤러가 연결되지 않았습니다.");
        }

        if (directionalLight == null)
        {
            Debug.LogWarning("Directional Light�� ����Ǿ� ���� �ʽ��ϴ�.");
            return;
        }

        // ��ǥ �� ���
        Color targetColor;
        float targetIntensity;
        Vector3 targetRotation;
        Color targetFogColor;
        float targetFogDensity;
        Material targetSkybox = null;
        bool enableStars = false;
        bool enableEventPost = false;
        AudioClip targetBgm = null;

        switch (CurrentPhase)
        {
            case TimePhase.Morning:
                targetColor = morningColor;
                targetIntensity = morningIntensity;
                targetRotation = new Vector3(50, 30, 0);
                targetFogColor = morningColor * 0.7f;
                targetFogDensity = 0.005f;
                targetSkybox = morningSkybox;
                targetBgm = morningBgm;
                Debug.Log("������ ��ħ (9�� )");

                if (ShopUI.Instance != null)
                    ShopUI.Instance.ApplyDiscountForMorning();

                break;
                

            case TimePhase.Noon:
                targetColor = noonColor;
                targetIntensity = noonIntensity;
                targetRotation = new Vector3(90, 60, 0);
                targetFogColor = noonColor * 0.8f;
                targetFogDensity = 0.003f;
                targetSkybox = noonSkybox;
                targetBgm = noonBgm;
                Debug.Log("������ ���� (15�� )");
                break;

            case TimePhase.Evening:
                targetColor = eveningColor;
                targetIntensity = eveningIntensity;
                targetRotation = new Vector3(160, 15, 0);
                targetFogColor = new Color(0.25f, 0.15f, 0.15f);
                targetFogDensity = 0.01f;
                targetSkybox = eveningSkybox;
                targetBgm = eveningBgm;
                Debug.Log("������ ���� (19�� )");
                break;

            case TimePhase.EventTime:
            default:
                targetColor = eventColor;
                targetIntensity = eventIntensity;
                targetRotation = new Vector3(300, -30, 0); // �޺� ����
                targetFogColor = new Color(0.03f, 0.04f, 0.08f);
                targetFogDensity = 0.02f;
                targetSkybox = eventSkybox;
                targetBgm = eventBgm;
                enableStars = true;
                enableEventPost = true;
                Debug.Log("������ ��� �ð� (�޺��� ������ ���� )");
                break;
        }

        // ��/����Ʈ ���μ��� Ȱ��/��Ȱ���� ��ǥ ���� �������� ��� ����
        if (stars != null) stars.SetActive(enableStars);
        if (eventPostVolume != null) eventPostVolume.SetActive(enableEventPost);

        if (targetSkybox != null)
            RenderSettings.skybox = targetSkybox;

        ApplyBgm(targetBgm);

        // �̹� ���� ���� ��ȯ�� ������ ���
        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        // �� ��ȯ ����
        transitionRoutine = StartCoroutine(TransitionEnvironment(
            targetColor,
            targetIntensity,
            targetRotation,
            targetFogColor,
            targetFogDensity
        ));
    }

    private IEnumerator TransitionEnvironment(
        Color targetColor,
        float targetIntensity,
        Vector3 targetRotation,
        Color targetFogColor,
        float targetFogDensity
    )
    {
        float t = 0f;

        Color startColor = directionalLight.color;
        float startIntensity = directionalLight.intensity;
        Quaternion startRot = directionalLight.transform.rotation;
        Quaternion endRot = Quaternion.Euler(targetRotation);

        Color startFogColor = RenderSettings.fogColor;
        float startFogDensity = RenderSettings.fogDensity;

        Color startAmbient = RenderSettings.ambientLight;
        Color targetAmbient = targetColor * 0.4f;

        while (t < 1f)
        {
            t += Time.deltaTime / transitionDuration;
            float lerp = Mathf.SmoothStep(0f, 1f, t);

            directionalLight.color = Color.Lerp(startColor, targetColor, lerp);
            directionalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, lerp);
            directionalLight.transform.rotation = Quaternion.Slerp(startRot, endRot, lerp);

            RenderSettings.fogColor = Color.Lerp(startFogColor, targetFogColor, lerp);
            RenderSettings.fogDensity = Mathf.Lerp(startFogDensity, targetFogDensity, lerp);

            RenderSettings.ambientLight = Color.Lerp(startAmbient, targetAmbient, lerp);

            yield return null;
        }
    }

    private void ApplyBgm(AudioClip clip)
    {
        if (bgmSource == null || clip == null) return;

        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }
}
