using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartManager : MonoBehaviour
{
    // 현재 활성화된 씬을 다시 로드하는 함수
    public void RestartCurrentScene()
    {
        // 현재 활성화된 씬의 buildIndex를 가져옴
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 현재 씬 재로딩
        SceneManager.LoadScene(currentSceneIndex);
    }
}
