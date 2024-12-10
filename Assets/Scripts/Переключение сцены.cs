using UnityEngine;
using UnityEngine.SceneManagement;  // Для работы с сценами

public class SceneSwitcher : MonoBehaviour
{
    // Функция для переключения сцены
    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
