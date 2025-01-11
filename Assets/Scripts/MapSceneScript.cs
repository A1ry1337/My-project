using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapSceneScript : MonoBehaviour
{
    public Button greenhouseButton; // Кнопка GreenhouseGarden
    public Sprite mapGreenhouse; // Обычное изображение
    public Sprite mapGreenhouseSelected; // Выбранное изображение

    public Button fieldButton;
    public Sprite mapField;
    public Sprite mapFieldSelected;

    public Button gardenButton;
    public Sprite mapGarden;
    public Sprite mapGardenSelected;

    private Button selectedButton; // Текущая выбранная кнопка
    private string currentLocation;

    void Start()
    {
        // Привязываем обработчики к кнопкам
        greenhouseButton.onClick.AddListener(() => OnButtonClicked(greenhouseButton, mapGreenhouse, mapGreenhouseSelected, "greenhouse"));
        fieldButton.onClick.AddListener(() => OnButtonClicked(fieldButton, mapField, mapFieldSelected, "field"));
        gardenButton.onClick.AddListener(() => OnButtonClicked(gardenButton, mapGarden, mapGardenSelected, "garden"));
    }

     void OnButtonClicked(Button button, Sprite normalSprite, Sprite selectedSprite, string locationName)
    {
        if (selectedButton == button)
        {
            // Если нажата уже выбранная кнопка, снимаем выделение
            button.GetComponent<Image>().sprite = normalSprite;
            selectedButton = null;
            currentLocation = null;
        }
        else
        {
            // Сбрасываем выделение с предыдущей кнопки
            if (selectedButton != null)
            {
                ResetButtonSprite(selectedButton);
            }

            // Устанавливаем выделение для текущей кнопки
            button.GetComponent<Image>().sprite = selectedSprite;
            selectedButton = button;
            currentLocation = locationName;
        }
    }

    void ResetButtonSprite(Button button)
    {
        if (button == greenhouseButton)
        {
            button.GetComponent<Image>().sprite = mapGreenhouse;
        }
        else if (button == fieldButton)
        {
            button.GetComponent<Image>().sprite = mapField;
        }
        else if (button == gardenButton)
        {
            button.GetComponent<Image>().sprite = mapGarden;
        }
    }


    public void BackClick() {
        if (!string.IsNullOrEmpty(currentLocation)) {
            MainSceneScript.currentLocation = currentLocation;
            MainSceneScript.UpdateLocationInDatabase(currentLocation);
            Debug.Log($"Текущая локация обновлена: {currentLocation}");
        }
        SceneManager.LoadScene("MainScene");
    }
}
