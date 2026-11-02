# Pocket Watch: 미스터리 사건

> 회중시계로 시간대를 앞뒤로 돌리며, 아침·점심·저녁·새벽마다 달라지는 세계를 탐색하는 Unity 미스터리 게임 (게임잼)

![Unity](https://img.shields.io/badge/Unity-6000.0.62f1-black)
![C#](https://img.shields.io/badge/C%23-239120)
![URP](https://img.shields.io/badge/Render-URP-lightgrey)

---

## 게임 소개

플레이어는 회중시계를 들고 맵을 돌아다닙니다. 시계를 쓰면 시간대가 바뀌고, 시간대에 따라 조명과 하늘, 음악뿐 아니라 나타나는 오브젝트와 상점 가격까지 달라집니다. 시간대는 아침 → 점심 → 저녁 → 새벽 순으로 순환합니다.

## 개발 정보

| 항목 | 내용 |
|---|---|
| 기간 | 2025.11 (Unity 게임잼) |
| 팀 | 5인 |
| 엔진 | Unity 6 (6000.0.62f1), URP |
| 언어 | C# |
| 입력 | Unity Input System |

## 담당 파트 — 이건호

**총괄 기획**과 함께 시간 시스템, 텔레포트, NPC 상점을 구현했습니다.

### 1. enum으로 관리하는 시간대

```csharp
// Assets/Script/UI/TimeManager.cs
public enum TimePhase
{
    Morning,   // 9시
    Noon,      // 15시
    Evening,   // 19시
    EventTime  // 새벽 (3~4시)
}
```

시간대를 문자열이나 숫자 대신 enum으로 정의해, 시간 조건을 쓰는 기믹과 UI가 같은 기준을 공유합니다. `TimeManager`가 현재 시간대를 갖고 다음·이전 시간대로 순환시킵니다.

### 2. 시간대 전환 연출

시간대가 바뀌면 `TimeManager`가 목표 값을 정하고 코루틴으로 5초 동안 부드럽게 바꿉니다.

- 태양광(Directional Light)의 색·밝기·각도 (`Color.Lerp`, `Quaternion.Slerp`, `SmoothStep`)
- 안개 색과 밀도, 주변광
- 스카이박스, 별, 새벽 전용 후처리 볼륨
- 시간대별 BGM

전환 중에 다시 시간이 바뀌면 이전 코루틴을 멈추고 새 전환을 시작해, 연출이 겹치지 않게 했습니다.

### 3. 회중시계 도구

`PocketWatchTool`(`IToolUsable` 구현)은 장착한 상태에서 **좌클릭으로 이전 시간대, 우클릭으로 다음 시간대**로 바꿉니다. 1.5초 쿨타임을 두어 시간대를 연속으로 넘기지 못하게 했습니다.

### 4. 시간대별 오브젝트 — 인스펙터 등록 방식

```csharp
// Assets/Script/UI/TimeState/TimeState.cs
public StateObjectSet morningObjects;   // 켤 목록 + 끌 목록
public StateObjectSet noonObjects;
public StateObjectSet eveningObjects;
public StateObjectSet eventTimeObjects;
```

시간대마다 켤 오브젝트와 끌 오브젝트를 인스펙터에 등록해 두면 `TimeState`가 적용합니다. 기획자가 코드를 고치지 않고 시간대별 마을 모습을 바꿀 수 있고, 등록된 오브젝트가 삭제된 경우도 건너뛰도록 처리했습니다.

### 5. NPC 상점과 텔레포트

- `ShopNPCInteractable`이 플레이어와 상호작용하면 상점 UI를 엽니다. 필요한 컴포넌트(지갑, 인벤토리, 대화 UI)가 없으면 경고를 남기고 멈춥니다
- `ShopUI`는 지갑에서 돈을 쓸 수 있을 때만 아이템을 지급하고, 아침 시간대에는 특정 상품 가격을 바꿉니다
- 상호작용 오브젝트로 지정된 장소 사이를 텔레포트합니다. 이동 순간 `CharacterController`를 잠시 꺼서 위치가 되돌아가지 않게 했습니다

## 배운 점

- enum으로 상태를 정의하면 여러 시스템이 같은 기준을 읽기 쉽고 안전하게 공유할 수 있습니다
- 기획과 구현을 함께 맡으면서, 기획 단계에서 기술 난이도와 개발 시간을 먼저 따져야 한다는 것을 배웠습니다

## 폴더 구조

```
Assets/Script/
├── UI/            시간(TimeManager, TimeState), 상점, 인벤토리 UI
├── ability/       회중시계 등 도구
├── Interactable/  상호작용 인터페이스, 문, NPC
├── NPC/           상점 NPC, 대화 UI
├── Item/          아이템, 줍기, 텔레포트 오브젝트
└── Player/        이동, 상호작용, 인벤토리, 지갑
```

## 실행 방법

1. Unity Hub에서 **Unity 6000.0.62f1**로 프로젝트를 엽니다
2. `Assets/Scenes/StartScn/StartMenu.unity`를 열고 Play 합니다
