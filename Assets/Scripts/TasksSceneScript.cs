using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class TasksSceneScript : MonoBehaviour
{
    // Объекты Image для отображения выбранных инструментов
    public Image selectedInstrument1;
    public Image selectedInstrument2;
    public Image selectedInstrument3;

    // Список спрайтов для инструментов
    public Sprite knifeSprite;
    public Sprite bucketSprite;
    public Sprite boxSprite;
    public Sprite shovelSprite;
    public Sprite bagSprite;
    public Sprite wheelbarrowSprite;

    // Словарь для связи названий инструментов со спрайтами
    private Dictionary<string, Sprite> toolSprites;

    // Список кнопок
    public Button taskButton1;
    public Button taskButton2;
    public Button taskButton3;
    public Button taskButton4;
    public Button taskButton5;

    // Ссылки на selected изображения
    public GameObject selectedTask1;
    public GameObject selectedTask2;
    public GameObject selectedTask3;
    public GameObject selectedTask4;
    public GameObject selectedTask5;

    // Персонаж, для которого выбираются задачи
    public string characterName = "Hero";

    private string currentTask;

    private List<string> temporarySelectedTools = new List<string>();

    void Start()
    {
        toolSprites = new Dictionary<string, Sprite>
    {
        { "Knife", knifeSprite },
        { "Bucket", bucketSprite },
        { "Box", boxSprite },
        { "Shovel", shovelSprite },
        { "Bag", bagSprite },
        { "Wheelbarrow", wheelbarrowSprite }
    };

        // Скрываем все изображения инструментов на старте
        ClearSelectedInstruments(); // Очистка инструментов при старте

        // Обновляем отображение инструментов
        DisplaySelectedInstruments();

        // Обработчики нажатий ко всем кнопкам
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

        Debug.Log("Выбрана задача: " + currentTask);
    }

    private void DisplaySelectedInstruments()
    {
        // Получаем выбранные инструменты из MainScene только при первом запуске
        if (temporarySelectedTools.Count == 0 && MainSceneScript.selectedTools.ContainsKey("Hero"))
        {
            List<string> heroTools = MainSceneScript.selectedTools["Hero"];
            Debug.Log("Отображаем выбранные инструменты для Hero: " + string.Join(", ", heroTools));

            // Сохраняем инструменты в временном хранилище
            temporarySelectedTools = new List<string>(heroTools);
        }

        // Перебираем выбранные инструменты и отображаем их
        for (int i = 0; i < temporarySelectedTools.Count && i < 3; i++)
        {
            string toolName = temporarySelectedTools[i];
            Sprite toolSprite;

            if (toolSprites.TryGetValue(toolName, out toolSprite))
            {
                // Устанавливаем изображение и показываем соответствующий объект
                switch (i)
                {
                    case 0:
                        selectedInstrument1.sprite = toolSprite;
                        selectedInstrument1.gameObject.SetActive(true);
                        break;
                    case 1:
                        selectedInstrument2.sprite = toolSprite;
                        selectedInstrument2.gameObject.SetActive(true);
                        break;
                    case 2:
                        selectedInstrument3.sprite = toolSprite;
                        selectedInstrument3.gameObject.SetActive(true);
                        break;
                }
            }
            else
            {
                Debug.LogWarning($"Спрайт для инструмента {toolName} не найден!");
            }
        }
    }

    private void ClearSelectedInstruments()
    {
        // Скрываем все изображения инструментов
        selectedInstrument1.gameObject.SetActive(false);
        selectedInstrument2.gameObject.SetActive(false);
        selectedInstrument3.gameObject.SetActive(false);

        // Очистим спрайты
        selectedInstrument1.sprite = null;
        selectedInstrument2.sprite = null;
        selectedInstrument3.sprite = null;

        // Очистим временные инструменты
        temporarySelectedTools.Clear();
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

    public void OpenInstrument()
    {
        SceneManager.LoadScene("InstrumentsScene");
    }

    public void BackClick()
    {
        if (!string.IsNullOrEmpty(currentTask))
            AddTaskToCharacter(currentTask);

        // Передаем выбранные инструменты в MainScene
        if (temporarySelectedTools.Count > 0)
        {
            MainSceneScript.selectedTools["Hero"] = new List<string>(temporarySelectedTools);
            Debug.Log("Выбранные инструменты переданы в MainScene.");
        }

        // Очистить инструменты перед переходом
        ClearSelectedInstruments(); // Очистка инструментов

        SceneManager.LoadScene("MainScene");
    }


}
