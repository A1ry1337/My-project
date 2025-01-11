using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class InstrumentsSceneScript : MonoBehaviour
{
    // Кнопки для инструментов
    public Button instrument1Button; // Нож
    public Button instrument2Button; // Ведро
    public Button instrument3Button; // Ящик
    public Button instrument4Button; // Лопата
    public Button instrument5Button; // Мешок
    public Button instrument6Button; // Тачка

    // Объекты, отображающие выбранные инструменты
    public GameObject selectedInstrument1; // Для ножа
    public GameObject selectedInstrument2; // Для ведра
    public GameObject selectedInstrument3; // Для ящика
    public GameObject selectedInstrument4; // Для лопаты
    public GameObject selectedInstrument5; // Для мешка
    public GameObject selectedInstrument6; // Для тачки

    // Словарь выбранных инструментов из MainSceneScript
    private Dictionary<string, List<string>> selectedTools;

    // Словарь для связи инструментов с кнопками и объектами
    private Dictionary<string, (Button button, GameObject selectedObject)> toolMappings;

    private void Start()
    {
        // Инициализируем ссылку на словарь
        selectedTools = MainSceneScript.selectedTools;

        // Создаем связи между инструментами, кнопками и объектами
        toolMappings = new Dictionary<string, (Button, GameObject)>
        {
            { "Knife", (instrument1Button, selectedInstrument1) },
            { "Bucket", (instrument2Button, selectedInstrument2) },
            { "Box", (instrument3Button, selectedInstrument3) },
            { "Shovel", (instrument4Button, selectedInstrument4) },
            { "Bag", (instrument5Button, selectedInstrument5) },
            { "Wheelbarrow", (instrument6Button, selectedInstrument6) }
        };

        // Устанавливаем обработчики для кнопок
        foreach (var tool in toolMappings)
        {
            string toolName = tool.Key;
            tool.Value.button.onClick.AddListener(() => SelectTool(toolName));
        }

        // Скрываем все объекты выбора инструментов при старте
        foreach (var tool in toolMappings)
        {
            tool.Value.selectedObject.SetActive(false);
        }
    }

    private void SelectTool(string tool)
    {
        // Проверяем, есть ли уже запись для персонажа Hero
        if (!selectedTools.ContainsKey("Hero"))
        {
            selectedTools["Hero"] = new List<string>();
        }

        var heroTools = selectedTools["Hero"];

        // Проверяем, выбран ли инструмент
        if (heroTools.Contains(tool))
        {
            // Убираем инструмент из списка
            heroTools.Remove(tool);
            Debug.Log($"Инструмент {tool} убран из списка.");

            // Скрываем объект выбора
            toolMappings[tool].selectedObject.SetActive(false);
        }
        else
        {
            // Проверяем ограничения
            if (tool == "Shovel" || tool == "Knife")
            {
                // Проверяем, заняты ли две руки
                if (heroTools.Contains("Shovel") && heroTools.Contains("Knife"))
                {
                    Debug.Log("Нельзя выбрать больше двух ручных инструментов.");
                    return;
                }
            }
            else
            {
                // Проверяем, занят ли слот для карго
                if (heroTools.Exists(t => t == "Bucket" || t == "Box" || t == "Bag" || t == "Wheelbarrow"))
                {
                    Debug.Log("Нельзя выбрать больше одного предмета карго.");
                    return;
                }
            }

            // Добавляем инструмент в список
            heroTools.Add(tool);
            Debug.Log($"Инструмент {tool} добавлен в список.");

            // Отображаем объект выбора
            toolMappings[tool].selectedObject.SetActive(true);
        }

        // Выводим текущий список выбранных инструментов для персонажа Hero
        Debug.Log($"Текущие инструменты для Hero: {string.Join(", ", heroTools)}");
    }


    public void BackClick() {
        SceneManager.LoadScene("TasksScene");
    }
}