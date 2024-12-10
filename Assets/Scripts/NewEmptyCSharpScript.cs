using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquareManager : MonoBehaviour
{
    public GameObject squarePrefab; // Префаб квадратика
    public Transform parentPanel;   // Панель, куда добавлять квадратики
    private List<Button> squares = new List<Button>(); // Список кнопок
    private Button firstSelected;   // Первая выбранная кнопка
    private Button secondSelected;  // Вторая выбранная кнопка

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
    }

    void OnSquareClicked(Button button, int index)
    {
        if (firstSelected == null)
        {
            firstSelected = button;
            Debug.Log("Первая кнопка выбрана: " + index);
        }
        else if (secondSelected == null && button != firstSelected)
        {
            secondSelected = button;
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
            int interval = Mathf.Abs(index2 - index1); // Расстояние между кнопками
            Debug.Log("Интервал между кнопками: " + interval);

            // Сброс выбора
            firstSelected = null;
            secondSelected = null;
        }
    }
}
