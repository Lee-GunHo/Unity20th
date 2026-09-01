using UnityEngine;

public class TimeState : MonoBehaviour
{
    [System.Serializable]
    public class StateObjectSet
    {
        [Tooltip("이 시간대에 활성화(Set Active True)할 오브젝트 목록")]
        public GameObject[] objectsToActivate;

        [Tooltip("이 시간대에 비활성화(Set Active False)할 오브젝트 목록")]
        public GameObject[] objectsToDeactivate;
    }

    // --- 인스펙터 창에 노출될 설정 변수들 ---
    [Header("시간대별 오브젝트 설정")]
    public StateObjectSet morningObjects;
    public StateObjectSet noonObjects;
    public StateObjectSet eveningObjects;
    public StateObjectSet eventTimeObjects;
    // ----------------------------------------

    
    /// <summary>
    /// TimeManager에서 이 함수를 호출하여 시간대 상태를 변경합니다.
    /// </summary>
    /// <param name="newPhase">새롭게 변경될 시간대 (TimeManager.TimePhase)</param>
    public void UpdateState(TimePhase newPhase)
    {
        Debug.Log("TimeState 변경: " + newPhase.ToString());

        // 현재 상태에 맞는 오브젝트 세트를 선택
        StateObjectSet targetSet = null;

        switch (newPhase)
        {
            case TimePhase.Morning:
                targetSet = morningObjects;
                break;

            case TimePhase.Noon:
                targetSet = noonObjects;
                break;

            case TimePhase.Evening:
                targetSet = eveningObjects;
                break;

            case TimePhase.EventTime:
                targetSet = eventTimeObjects;
                break;
        }

        // 선택된 세트의 오브젝트들을 활성화/비활성화
        if (targetSet != null)
        {
            ApplyStateSet(targetSet);
        }
    }

    /// <summary>
    /// 실제로 오브젝트들을 켜고 끄는 작업을 수행하는 함수
    /// </summary>
    private void ApplyStateSet(StateObjectSet stateSet)
    {
        // 활성화할 오브젝트들 켜기
        if (stateSet.objectsToActivate != null)
        {
            foreach (GameObject obj in stateSet.objectsToActivate)
            {
                if (obj != null) // 오브젝트가 실수로 삭제되었을 경우 대비
                {
                    obj.SetActive(true);
                }
            }
        }

        // 비활성화할 오브젝트들 끄기
        if (stateSet.objectsToDeactivate != null)
        {
            foreach (GameObject obj in stateSet.objectsToDeactivate)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }
    }
}