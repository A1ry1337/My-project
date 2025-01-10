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

            // Мы больше не устанавливаем начальный цвет, так как цвет будет определяться префабом
        }
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

            // Окрашиваем все кнопки в интервале, исключая саму первую и последнюю
            for (int i = startIndex + 1; i < endIndex; i++)
            {
                // Если кнопка еще не окрашена, то окрашиваем ее
                if (squares[i].GetComponent<Image>().color != selectedColor)
                {
                    squares[i].GetComponent<Image>().color = selectedColor;
                }
            }

            Debug.Log("Интервал между кнопками: " + (endIndex - startIndex));

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

    // Метод для загрузки сцены TasksScene
    void LoadTasksScene()
    {
        // Загружаем сцену TasksScene
        SceneManager.LoadScene("TasksScene");
    }
}
