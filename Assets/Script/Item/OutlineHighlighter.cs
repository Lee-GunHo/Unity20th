using UnityEngine;

public class OutlineHighlighter : MonoBehaviour
{
    [Header("하이라이트 색상")]
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField, Range(0f, 5f)] private float emissionIntensity = 1.5f;

    private Renderer[] renderers;
    private bool isHighlighted;

    private void Awake()
    {
        // 자기 자신 + 자식까지 전체 Renderer 가져오기
        renderers = GetComponentsInChildren<Renderer>(includeInactive: true);

        if (renderers == null || renderers.Length == 0)
        {
            Debug.LogWarning($"[OutlineHighlighter] Renderer를 찾지 못했습니다. ({gameObject.name})");
        }
    }

    public void SetHighlight(bool enable)
    {
        if (isHighlighted == enable)
            return;

        isHighlighted = enable;

        if (renderers == null) return;

        foreach (var rend in renderers)
        {
            if (rend == null) continue;

            foreach (var mat in rend.materials)
            {
                if (mat == null) continue;

                if (enable)
                {
                    if (mat.HasProperty("_EmissionColor"))
                    {
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", highlightColor * emissionIntensity);
                    }
                }
                else
                {
                    if (mat.HasProperty("_EmissionColor"))
                    {
                        // 원래 값 복원까지 필요하면 따로 저장해야 함. 지금은 그냥 끔.
                        mat.SetColor("_EmissionColor", Color.black);
                        mat.DisableKeyword("_EMISSION");
                    }
                }
            }
        }
    }
}
