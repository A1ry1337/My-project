using UnityEngine;
using UnityEngine.SceneManagement;

public class InstrumentsSceneScript : MonoBehaviour
{
    public void BackClick() {
        SceneManager.LoadScene("TasksScene");
    }
}