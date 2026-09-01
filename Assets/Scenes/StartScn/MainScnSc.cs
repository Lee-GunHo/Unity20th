using UnityEngine;
using UnityEngine.SceneManagement; // 씬을 관리하기 위해 꼭 필요

public class MainScnSc : MonoBehaviour
{
    // 이 함수를 버튼의 OnClick() 이벤트에 연결할 것
    // 함수 이름은 원하시는 대로 바꿔도 됌 (예: LoadDefaultScene)
    public void StartGame()
    {
        Debug.Log("Start 버튼 클릭 'DefaultMap'으로 이동합니다.");

        // "DefaultScene"이라는 이름의 씬을 불러옴
        SceneManager.LoadScene("DefaultMap");
    }

    // 게임 종료 버튼
    public void QuitGame()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }
}