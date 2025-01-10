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


    public static string currentLocation;
    public static List<string> selectedEquipment = new List<string>();

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

    void Start()
    {
        UpdateMapButtonImage();
        // Привязка событий кнопок
        slot1Button.onClick.AddListener(() => ToggleSlotSelection("Slot1", selectedSlot1));
        slot2Button.onClick.AddListener(() => ToggleSlotSelection("Slot2", selectedSlot2));
        slot3Button.onClick.AddListener(() => ToggleSlotSelection("Slot3", selectedSlot3));
    }

    // Метод для изменения текущей локации
    public void SetCurrentLocation(string location)
    {
        currentLocation = location;
        UpdateMapButtonImage(); // Обновляем изображение кнопки Map
        Debug.Log($"Текущая локация обновлена: {currentLocation}");
    }

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
        SceneManager.LoadScene("MapScene");
    }
}
