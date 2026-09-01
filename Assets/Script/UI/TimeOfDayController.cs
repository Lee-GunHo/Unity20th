using UnityEngine;

public class TimeOfDayController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Range(0, 24)] public float hour = 12f; // 0~24 사이 시간대 입력
    public Light sunLight;                  // Directional Light 연결
    public Gradient skyColor;               // Inspector에서 하늘색 변화 지정

    void Update()
    {
        // 실제 시간 흐름은 없음. 시간대 값으로만 연출.
        float rotation = Mathf.Lerp(-90f, 270f, hour / 24f);
        sunLight.transform.rotation = Quaternion.Euler(rotation, 0f, 0f);

        // 하늘 색상 바꾸기 (선택)
        RenderSettings.ambientLight = skyColor.Evaluate(hour / 24f);
    }
}
