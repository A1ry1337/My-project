using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MainSceneScript : MonoBehaviour
{
    // Ссылки на изображения для карты
    public Sprite mapFarmerGardenSelected;
    public Sprite mapFarmerFieldSelected;
    public Sprite mapFarmerGreenhouseSelected;
    public Sprite mapFarmerNoSelected;

    public Button mapButton; // Ссылка на саму кнопку Map

    // Ссылки на кнопки слотов
    public Button slot1Button;
    public Button slot2Button;
    public Button slot3Button;

    // Ссылки на объекты, которые должны быть видимы/невидимы
    public GameObject selectedSlot1;
    public GameObject selectedSlot2;
    public GameObject selectedSlot3;

    // Ссылки на объекты PlaningArea для каждого дня
    public GameObject planingDay1;
    public GameObject planingDay2;
    public GameObject planingDay3;
    public GameObject planingDay4;

    // Ссылки на кнопки переключения дней
    public Button buttonDay1;
    public Button buttonDay2;
    public Button buttonDay3;
    public Button buttonDay4;

    private Button[] dayButtons;

    public Sprite activeButtonDay;
    public Sprite inactiveButtonDay;


    public static string currentLocation;
    public static List<string> selectedEquipment = new List<string>();

    public static int currentDay; // Текущий выбранный день


    // Хранилище данных для персонажей (главный герой + 3 персонажа)
    public static Dictionary<string, List<string>> selectedTasks = new Dictionary<string, List<string>>()
    {
        { "Hero", new List<string>() },
        { "Character1", new List<string>() },
        { "Character2", new List<string>() },
        { "Character3", new List<string>() }
    };

    public static Dictionary<string, List<string>> selectedTools = new Dictionary<string, List<string>>()
    {
        { "Hero", new List<string>() },
        { "Character1", new List<string>() },
        { "Character2", new List<string>() },
        { "Character3", new List<string>() }
    };

    public static Dictionary<string, List<string>> selectedTimeIntervals = new Dictionary<string, List<string>>()
    {
        { "Hero", new List<string>() },
        { "Character1", new List<string>() },
        { "Character2", new List<string>() },
        { "Character3", new List<string>() }
    };

    // ВСПОМОГАТЕЛЬНЫЙ Словарь для хранения выбранных интервалов для каждого дня
    public static Dictionary<int, List<string>> selectedIntervalsPerDay = new Dictionary<int, List<string>>()
    {
        { 1, new List<string>() },
        { 2, new List<string>() },
        { 3, new List<string>() },
        { 4, new List<string>() }
    };


    void Start()
    {
        UpdateMapButtonImage();
        // Привязка событий кнопок
        slot1Button.onClick.AddListener(() => ToggleSlotSelection("Slot1", selectedSlot1));
        slot2Button.onClick.AddListener(() => ToggleSlotSelection("Slot2", selectedSlot2));
        slot3Button.onClick.AddListener(() => ToggleSlotSelection("Slot3", selectedSlot3));

        buttonDay1.onClick.AddListener(() => SwitchDay(1));
        buttonDay2.onClick.AddListener(() => SwitchDay(2));
        buttonDay3.onClick.AddListener(() => SwitchDay(3));
        buttonDay4.onClick.AddListener(() => SwitchDay(4));

        dayButtons = new Button[] { buttonDay1, buttonDay2, buttonDay3, buttonDay4 };

        // Оценка состояния кнопок и установка currentDay
        currentDay = GetCurrentDay(); // Получаем текущий день

        // ПРИ ВЫЗОВЕ КАРТА слетает
        SwitchDay(currentDay); // Устанавливаем день, который был активен до этого
    }

    // Метод для изменения текущей локации
    // public void SetCurrentLocation(string location)
    // {
    //     currentLocation = location;
    //     UpdateMapButtonImage(); // Обновляем изображение кнопки Map
    //     Debug.Log($"Текущая локация обновлена: {currentLocation}");
    // }

    private void UpdateMapButtonImage()
    {
        // Получаем компонент Image у кнопки Map
        Image mapImage = mapButton.GetComponent<Image>();

        if (string.IsNullOrEmpty(currentLocation))
        {
            mapImage.sprite = mapFarmerNoSelected;
        }
        else if (currentLocation == "garden")
        {
            mapImage.sprite = mapFarmerGardenSelected;
        }
        else if (currentLocation == "field")
        {
            mapImage.sprite = mapFarmerFieldSelected;
        }
        else if (currentLocation == "greenhouse")
        {
            mapImage.sprite = mapFarmerGreenhouseSelected;
        }
        Debug.Log(mapImage.sprite.name);
    }

    // Метод для обработки выбора слота
    void ToggleSlotSelection(string slotName, GameObject selectedSlot)
    {
        // Если слот уже выбран, удаляем из списка, если нет - добавляем
        if (selectedEquipment.Contains(slotName))
        {
            selectedEquipment.Remove(slotName);
            selectedSlot.SetActive(false); // Скрываем обводку, если слот отменен
            Debug.Log($"Слот {slotName} удален из выбранных.");
        }
        else
        {
            selectedEquipment.Add(slotName);
            selectedSlot.SetActive(true); // Показываем обводку, если слот выбран
            Debug.Log($"Слот {slotName} добавлен в выбранные.");
        }
        Debug.Log("Обновленное состояние selectedEquipment: " + string.Join(", ", selectedEquipment));
    }

    public void SelectEquipment(string equipment)
    {
        if (!selectedEquipment.Contains(equipment))
        {
            selectedEquipment.Add(equipment);
            Debug.Log("Добавлена экипировка: " + equipment);
        }
        else
        {
            Debug.Log("Экипировка уже выбрана: " + equipment);
        }
    }

    int GetCurrentDay() {
        // Восстановление текущего дня
        return PlayerPrefs.GetInt("CurrentDay", 1); // Получаем день из PlayerPrefs

        // // Проверяем, какой день был активен по изображению кнопки
        // for (int i = 1; i <= dayButtons.Length; i++) {
        //     Button button = dayButtons[i-1];
        //     if (button.GetComponent<Image>().sprite.name == "buttonActive_0") {
        //         return i;
        //     }
        // }
        // return 1; // Если день не был активен, возвращаем первый день по умолчанию
    }

    // Метод для переключения дня
    public void SwitchDay(int day)
    {
        currentDay = day;

        // Сбрасываем карту 
        currentLocation = null;
        UpdateMapButtonImage();

        // Скрыть все панели дней
        planingDay1.SetActive(false);
        planingDay2.SetActive(false);
        planingDay3.SetActive(false);
        planingDay4.SetActive(false);

        // На каждую кнопку поставить изображение неактивной кнопки
        buttonDay1.GetComponent<Image>().sprite = inactiveButtonDay;
        buttonDay2.GetComponent<Image>().sprite = inactiveButtonDay;
        buttonDay3.GetComponent<Image>().sprite = inactiveButtonDay;
        buttonDay4.GetComponent<Image>().sprite = inactiveButtonDay;

        // Показать панель текущего дня
        switch (day)
        {
            case 1:
                planingDay1.SetActive(true);
                buttonDay1.GetComponent<Image>().sprite = activeButtonDay;
                DisplayIntervalsForDay(day); // Показываем интервалы для выбранного дня
                break;
            case 2:
                planingDay2.SetActive(true);
                buttonDay2.GetComponent<Image>().sprite = activeButtonDay;
                DisplayIntervalsForDay(day);
                break;
            case 3:
                planingDay3.SetActive(true);
                buttonDay3.GetComponent<Image>().sprite = activeButtonDay;
                DisplayIntervalsForDay(day);
                break;
            case 4:
                planingDay4.SetActive(true);
                buttonDay4.GetComponent<Image>().sprite = activeButtonDay;
                DisplayIntervalsForDay(day);
                break;
        }

        Debug.Log($"Переключено на день {day}");
    }

    // Метод для отображения выбранных интервалов для дня
    private void DisplayIntervalsForDay(int day)
    {
        if (selectedIntervalsPerDay.ContainsKey(day))
        {
            List<string> intervals = selectedIntervalsPerDay[day];
            foreach (var interval in intervals)
            {
                Debug.Log($"Отображение интервала {interval} для дня {day}");
                // Тут добавьте логику отображения интервалов на панели соответствующего дня
            }
        }
    }

    // Сохранение временного интервала
    public static void SaveTimeInterval(string character, string interval)
    {
        if (selectedTimeIntervals.ContainsKey(character))
        {
            string intervalWithDay = $"Day {currentDay}: {interval}";
            // Сохраняем день и интервал
            selectedTimeIntervals[character].Add(intervalWithDay);
            // Сохраняем этот интервал для текущего дня
            if (!selectedIntervalsPerDay[currentDay].Contains(interval))
            {
                selectedIntervalsPerDay[currentDay].Add(interval);
            }
            Debug.Log($"Сохранен интервал {intervalWithDay} для {character}.");
        }
        else
        {
            Debug.LogError($"Персонаж {character} не найден в словаре.");
        }
    }

    public void SelectTimeInterval(string character, string timeInterval)
    {
        if (!selectedTimeIntervals[character].Contains(timeInterval))
        {
            selectedTimeIntervals[character].Add(timeInterval);
        }
        Debug.Log($"{character} selected time interval: {timeInterval}");
        SceneManager.LoadScene("TasksScene");
    }

    public void OpenMapScene() {
        PlayerPrefs.SetInt("CurrentDay", currentDay);
        SceneManager.LoadScene("MapScene");
    }
}
