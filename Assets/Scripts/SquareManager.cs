using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SquareManager : MonoBehaviour
{
    public GameObject squarePrefab; // Префаб квадратика
    public Transform parentPanel;   // Панель, куда добавлять квадратики
    private List<Button> squares = new List<Button>(); // Список кнопок
    private Button firstSelected;   // Первая выбранная кнопка
    private Button secondSelected;  // Вторая выбранная кнопка

    // Цвета для выделения
    public Color selectedColor = Color.green; // Цвет для выбранных квадратов

    private const string SelectedButtonsKey = "SelectedButtons";

    void Start()
    {
        // Создаём 12 квадратиков
        for (int i = 0; i < 12; i++)
        {
            GameObject newSquare = Instantiate(squarePrefab, parentPanel);
            Button button = newSquare.GetComponent<Button>();
            int index = i; // Локальная копия индекса для лямбды
            button.onClick.AddListener(() => OnSquareClicked(button, index));
            squares.Add(button);
        }

        // Восстанавливаем закрашенные кнопки
        RestoreSelectedButtons();

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    void OnSquareClicked(Button button, int index)
    {
        // Если кнопка уже закрашена, ничего не делаем
        if (button.GetComponent<Image>().color == selectedColor) return;

        if (firstSelected == null)
        {
            firstSelected = button;
            // Окрашиваем первую выбранную кнопку
            firstSelected.GetComponent<Image>().color = selectedColor;
            Debug.Log("Первая кнопка выбрана: " + index);
        }
        else if (secondSelected == null && button != firstSelected)
        {
            secondSelected = button;
            // Окрашиваем вторую выбранную кнопку
            secondSelected.GetComponent<Image>().color = selectedColor;
            Debug.Log("Вторая кнопка выбрана: " + index);
            CalculateInterval();
        }
    }

    void CalculateInterval()
    {
        if (firstSelected != null && secondSelected != null)
        {
            int index1 = squares.IndexOf(firstSelected);
            int index2 = squares.IndexOf(secondSelected);

            // Находим минимальный и максимальный индекс для интервала
            int startIndex = Mathf.Min(index1, index2);
            int endIndex = Mathf.Max(index1, index2);

            // Создаем строку интервала (например, "1-5")
            string interval = $"{startIndex}-{endIndex}";

            // Сохраняем интервал в MainSceneScript
            MainSceneScript.SaveTimeInterval("Hero", interval);

            Debug.Log($"Интервал {interval} сохранен для персонажа Hero на день {MainSceneScript.currentDay}.");

            // Окрашиваем кнопки в интервале
            for (int i = startIndex; i <= endIndex; i++)
            {
                squares[i].GetComponent<Image>().color = selectedColor;
            }

            // Сохраняем состояние закрашенных кнопок
            SaveSelectedButtons();

            // Сброс выбора
            ResetSelections();

            // Загрузка сцены TasksScene
            LoadTasksScene();
        }
    }

    void ResetSelections()
    {
        // Сбрасываем выбор, но не сбрасываем уже окрашенные ячейки
        firstSelected = null;
        secondSelected = null;
    }

    // Метод для сохранения состояния закрашенных кнопок
    void SaveSelectedButtons()
    {
        List<int> selectedButtonIndices = new List<int>();

        // Собираем индексы закрашенных кнопок
        for (int i = 0; i < squares.Count; i++)
        {
            if (squares[i].GetComponent<Image>().color == selectedColor)
            {
                selectedButtonIndices.Add(i);
            }
        }

        // Сохраняем индексы в PlayerPrefs
        PlayerPrefs.SetString(SelectedButtonsKey, string.Join(",", selectedButtonIndices));
        PlayerPrefs.Save();
    }

    // Метод для восстановления состояния закрашенных кнопок
    void RestoreSelectedButtons()
    {
        if (PlayerPrefs.HasKey(SelectedButtonsKey))
        {
            string savedIndices = PlayerPrefs.GetString(SelectedButtonsKey);
            string[] indices = savedIndices.Split(',');

            foreach (string indexStr in indices)
            {
                if (int.TryParse(indexStr, out int index) && index >= 0 && index < squares.Count)
                {
                    squares[index].GetComponent<Image>().color = selectedColor; // Восстанавливаем цвет
                }
            }
        }
    }

    // // Восстанавливаем интервалы из MainSceneScript
    // void RestoreIntervals()
    // {
    //     // Проверяем, есть ли сохраненные интервалы для текущего дня
    //     if (MainSceneScript.selectedTimeIntervals.TryGetValue("Hero", out List<string> intervals))
    //     {
    //         foreach (string interval in intervals)
    //         {
    //             // Парсим строку интервала, например, "1-5"
    //             string[] parts = interval.Split('-');
    //             if (parts.Length == 2 && int.TryParse(parts[0], out int startIndex) && int.TryParse(parts[1], out int endIndex))
    //             {
    //                 // Окрашиваем все кнопки в интервале
    //                 for (int i = startIndex; i <= endIndex; i++)
    //                 {
    //                     squares[i].GetComponent<Image>().color = selectedColor;
    //                 }
                    
    //             }
    //         }
    //     }
    // }

    void LoadTasksScene()
    {
        SceneManager.LoadScene("TasksScene");
    }
}
