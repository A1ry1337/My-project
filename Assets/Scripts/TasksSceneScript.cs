using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TasksSceneScript : MonoBehaviour
{
    // Список кнопок
    public Button taskButton1;
    public Button taskButton2;
    public Button taskButton3;
    public Button taskButton4;
    public Button taskButton5;

    // Ссылки на изображения задач
    public GameObject selectedTask1;
    public GameObject selectedTask2;
    public GameObject selectedTask3;
    public GameObject selectedTask4;
    public GameObject selectedTask5;

    // Персонаж, для которого выбираются задачи (например, Hero)
    public string characterName = "Hero";
    
    private string currentTask;

    void Start()
    {
        // Подключаем обработчики нажатий ко всем кнопкам
        taskButton1.onClick.AddListener(() => OnTaskButtonClicked(1));
        taskButton2.onClick.AddListener(() => OnTaskButtonClicked(2));
        taskButton3.onClick.AddListener(() => OnTaskButtonClicked(3));
        taskButton4.onClick.AddListener(() => OnTaskButtonClicked(4));
        taskButton5.onClick.AddListener(() => OnTaskButtonClicked(5));
    }

    // Метод для управления выбором задачи
    public void OnTaskButtonClicked(int taskNumber)
    {
        // Скрываем все изображения задач
        selectedTask1.SetActive(false);
        selectedTask2.SetActive(false);
        selectedTask3.SetActive(false);
        selectedTask4.SetActive(false);
        selectedTask5.SetActive(false);

        // Отображаем соответствующее изображение для выбранной задачи
        switch (taskNumber)
        {
            case 1:
                selectedTask1.SetActive(true);
                currentTask = "Task1";
                break;
            case 2:
                selectedTask2.SetActive(true);
                currentTask = "Task2";
                break;
            case 3:
                selectedTask3.SetActive(true);
                currentTask = "Task3";
                break;
            case 4:
                selectedTask4.SetActive(true);
                currentTask = "Task4";
                break;
            case 5:
                selectedTask5.SetActive(true);
                currentTask = "Task5";
                break;
        }

        // Логирование текущего состояния выбора
        Debug.Log("Выбрана задача: " + currentTask);
    }

    // Метод для добавления задачи в selectedTasks (MainSceneScript)
    void AddTaskToCharacter(string task)
    {
        // Получаем доступ к selectedTasks в MainSceneScript и добавляем задачу для текущего персонажа
        if (MainSceneScript.selectedTasks.ContainsKey(characterName))
        {
            // Добавляем задачу в список задач для этого персонажа
            MainSceneScript.selectedTasks[characterName].Add(task);
            Debug.Log($"{characterName} добавлена задача: {task}");
        }
    }

    public void OpenInstrument() {
        SceneManager.LoadScene("InstrumentsScene");
    }

    public void BackClick() {
        if (!string.IsNullOrEmpty(currentTask))
            AddTaskToCharacter(currentTask);

        SceneManager.LoadScene("MainScene");
    }
}
