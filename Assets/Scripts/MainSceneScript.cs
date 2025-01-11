using System.Collections.Generic; // Для использования Dictionary и List
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mono.Data.Sqlite; // Необходимая библиотека для работы с SQLite
using System;
using System.Data;

public class MainSceneScript : MonoBehaviour
{
    public DatabaseManager databaseManager;  // Ссылка на DatabaseManager

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

    // Путь к базе данных SQLite
    static private string dbPath;

    void Awake()
    {
        dbPath = "URI=file:" + Application.persistentDataPath + "/schedule.db"; // Получение пути к базе данных
    }

    void Start()
    {
        // Инициализация базы данных
        InitializeDatabase();

        UpdateMapButtonImage();

        // Привязка событий кнопок
        slot1Button.onClick.AddListener(() => ToggleSlotSelection("Item1", selectedSlot1));
        slot2Button.onClick.AddListener(() => ToggleSlotSelection("Item2", selectedSlot2));
        slot3Button.onClick.AddListener(() => ToggleSlotSelection("Item3", selectedSlot3));

        buttonDay1.onClick.AddListener(() => SwitchDay(1));
        buttonDay2.onClick.AddListener(() => SwitchDay(2));
        buttonDay3.onClick.AddListener(() => SwitchDay(3));
        buttonDay4.onClick.AddListener(() => SwitchDay(4));

        dayButtons = new Button[] { buttonDay1, buttonDay2, buttonDay3, buttonDay4 };

        // Установка текущего дня
        currentDay = GetCurrentDay(); // Получаем текущий день
        LoadEquipmentForDay(currentDay); // Загружаем экипировку для текущего дня

        SwitchDay(currentDay); // Устанавливаем день
    }

    // Метод для инициализации базы данных SQLite
    void InitializeDatabase()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            // Проверка, существует ли таблица, если нет, создать её
            string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS Schedules (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                day INTEGER NOT NULL,
                location_id INTEGER,
                assignment_id INTEGER,
                FOREIGN KEY(location_id) REFERENCES Locations(id),
                FOREIGN KEY(assignment_id) REFERENCES Assignments(id)
            )";

            using (var command = new SqliteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
                Debug.Log("Таблица Schedules создана или уже существует.");
            }
            // Проверка, существует ли таблица Locations, если нет, создать её
            string createLocationsTableQuery = @"
            CREATE TABLE IF NOT EXISTS Locations (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                location_name TEXT NOT NULL
            )";

            using (var command = new SqliteCommand(createLocationsTableQuery, connection))
            {
                command.ExecuteNonQuery();
                Debug.Log("Таблица Locations создана или уже существует.");
            }

            // Проверка, существует ли таблица Assignments, если нет, создать её
            string createAssignmentsTableQuery = @"
            CREATE TABLE IF NOT EXISTS Assignments (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                character_id INTEGER NOT NULL,
                task_id INTEGER NOT NULL,
                time_interval_id INTEGER NOT NULL,
                FOREIGN KEY(character_id) REFERENCES Characters(id),
                FOREIGN KEY(task_id) REFERENCES Tasks(id),
                FOREIGN KEY(time_interval_id) REFERENCES TimeIntervals(id)
            )";
            using (var command = new SqliteCommand(createAssignmentsTableQuery, connection))
            {
                command.ExecuteNonQuery();
                Debug.Log("Таблица Assignments создана или уже существует.");
            }

            // Проверка, существует ли таблица AssignmentEquipments, если нет, создать её
            string createAssignmentEquipmentsTableQuery = @"
    CREATE TABLE IF NOT EXISTS AssignmentEquipments (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        assignment_id INTEGER NOT NULL,
        equipment_id INTEGER NOT NULL,
        FOREIGN KEY (assignment_id) REFERENCES Assignments(id),
        FOREIGN KEY (equipment_id) REFERENCES Equipments(id)
    )";

            using (var command = new SqliteCommand(createAssignmentEquipmentsTableQuery, connection))
            {
                command.ExecuteNonQuery();
                Debug.Log("Таблица AssignmentEquipments создана или уже существует.");
            }

            // Таблица Equipments
            string createEquipmentsTableQuery = @"
    CREATE TABLE IF NOT EXISTS Equipments (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        name VARCHAR NOT NULL
    )";

            using (var command = new SqliteCommand(createEquipmentsTableQuery, connection))
            {
                command.ExecuteNonQuery();
                Debug.Log("Таблица Equipments создана или уже существует.");
            }

            // Таблица Characters
            string createCharactersTableQuery = @"
        CREATE TABLE IF NOT EXISTS Characters (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name VARCHAR NOT NULL,
            type VARCHAR NOT NULL
        )";

            using (var command = new SqliteCommand(createCharactersTableQuery, connection))
            {
                command.ExecuteNonQuery();
                Debug.Log("Таблица Characters создана или уже существует.");
            }
            // Таблица HeroEquipment
            string createHeroEquipmentTableQuery = @"
    CREATE TABLE IF NOT EXISTS HeroEquipment (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Day INTEGER NOT NULL CHECK(Day >= 1 AND Day <= 4),  -- День (1, 2, 3 или 4)
        Item1 TEXT,  -- Название первого предмета, если выбран
        Item2 TEXT,  -- Название второго предмета, если выбран
        Item3 TEXT   -- Название третьего предмета, если выбран
    )";

            using (var command = new SqliteCommand(createHeroEquipmentTableQuery, connection))
            {
                command.ExecuteNonQuery();
                Debug.Log("Таблица HeroEquipment создана или уже существует.");
            }

            // Автоматическое заполнение днями 1, 2, 3, 4
            string insertDaysQuery = @"
    INSERT INTO HeroEquipment (Day)
    SELECT Day FROM (
        SELECT 1 AS Day UNION ALL
        SELECT 2 UNION ALL
        SELECT 3 UNION ALL
        SELECT 4
    )
    WHERE NOT EXISTS (
        SELECT 1 FROM HeroEquipment WHERE Day IN (1, 2, 3, 4)
    )";

            using (var command = new SqliteCommand(insertDaysQuery, connection))
            {
                int rowsInserted = command.ExecuteNonQuery();
                Debug.Log($"{rowsInserted} дней добавлено в таблицу HeroEquipment (если их не было).");
            }



            // Создаем персонажа Hero, если его нет в базе данных
            string insertHeroQuery = @"
        INSERT INTO Characters (name, type)
        SELECT @name, @type
        WHERE NOT EXISTS (SELECT 1 FROM Characters WHERE name = @name)";

            using (var insertCommand = new SqliteCommand(insertHeroQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@name", "Hero");
                insertCommand.Parameters.AddWithValue("@type", "Hero");
                insertCommand.ExecuteNonQuery();
                Debug.Log("Персонаж Hero добавлен в таблицу Characters.");
            }

            // Таблица Tasks
            string createTasksTableQuery = @"
    CREATE TABLE IF NOT EXISTS Tasks (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        name VARCHAR NOT NULL,
        description TEXT
    )";

            using (var command = new SqliteCommand(createTasksTableQuery, connection))
            {
                command.ExecuteNonQuery();
                Debug.Log("Таблица Tasks создана или уже существует.");
            }

            // Таблица TimeIntervals
            string createTimeIntervalsTableQuery = @"
    CREATE TABLE IF NOT EXISTS TimeIntervals (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        start_time TIME NOT NULL,
        end_time TIME NOT NULL
    )";

            using (var command = new SqliteCommand(createTimeIntervalsTableQuery, connection))
            {
                command.ExecuteNonQuery();
                Debug.Log("Таблица TimeIntervals создана или уже существует.");
            }




            // Проверка наличия записей для дней с 1 по 4, чтобы не добавлять их заново
            for (int day = 1; day <= 4; day++)
            {
                // Запрос для проверки наличия записи для дня
                string checkQuery = $"SELECT COUNT(*) FROM Schedules WHERE day = {day}";
                using (var checkCommand = new SqliteCommand(checkQuery, connection))
                {
                    long count = (long)checkCommand.ExecuteScalar();
                    if (count == 0)  // Если записи нет, вставляем новую
                    {
                        string insertQuery = $"INSERT INTO Schedules (day) VALUES ({day})";
                        using (var insertCommand = new SqliteCommand(insertQuery, connection))
                        {
                            insertCommand.ExecuteNonQuery();
                            Debug.Log($"Запись для дня {day} добавлена.");
                        }
                    }
                    else
                    {
                        Debug.Log($"Запись для дня {day} уже существует.");
                    }
                }
            }

            connection.Close();
        }
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
        Debug.Log(mapImage.sprite.name);
    }

    void ToggleSlotSelection(string slotName, GameObject selectedSlot)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            if (selectedEquipment.Contains(slotName))  // Если предмет уже выбран
            {
                // Удаляем предмет из базы данных
                string updateQuery = $"UPDATE HeroEquipment SET {slotName} = NULL WHERE Day = @Day";
                using (var command = new SqliteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Day", currentDay);
                    command.ExecuteNonQuery();
                }

                // Удаляем предмет из списка
                selectedEquipment.Remove(slotName);
                selectedSlot.SetActive(false);
                Debug.Log($"Слот {slotName} удален из выбранных.");
            }
            else  // Если предмет не выбран
            {
                // Добавляем предмет в базу данных
                string updateQuery = $"UPDATE HeroEquipment SET {slotName} = @Item WHERE Day = @Day";
                using (var command = new SqliteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Item", slotName);
                    command.Parameters.AddWithValue("@Day", currentDay);
                    command.ExecuteNonQuery();
                }

                // Добавляем предмет в список
                selectedEquipment.Add(slotName);
                selectedSlot.SetActive(true);
                Debug.Log($"Слот {slotName} добавлен в выбранные.");
            }

            Debug.Log("Обновленное состояние selectedEquipment: " + string.Join(", ", selectedEquipment));
        }
    }

    void LoadEquipmentForDay(int day)
    {
        selectedEquipment.Clear(); // Очищаем текущий список

        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            string selectQuery = "SELECT Item1, Item2, Item3 FROM HeroEquipment WHERE Day = @Day";
            using (var command = new SqliteCommand(selectQuery, connection))
            {
                command.Parameters.AddWithValue("@Day", day);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        for (int i = 1; i <= 3; i++)  // Проверяем все три предмета
                        {
                            string item = reader[$"Item{i}"] as string;
                            if (!string.IsNullOrEmpty(item))
                            {
                                selectedEquipment.Add(item); // Добавляем предмет в список
                            }
                        }
                    }
                }
            }
        }

        // Устанавливаем состояние кнопок
        selectedSlot1.SetActive(selectedEquipment.Contains("Item1"));
        selectedSlot2.SetActive(selectedEquipment.Contains("Item2"));
        selectedSlot3.SetActive(selectedEquipment.Contains("Item3"));

        Debug.Log($"Загружено оборудование для дня {day}: " + string.Join(", ", selectedEquipment));
    }




    int GetCurrentDay()
    {
        return PlayerPrefs.GetInt("CurrentDay", 1); // Получаем день из PlayerPrefs
    }

    public void SwitchDay(int day)
    {
        currentDay = day;

        // Установим текущую локацию для дня, если она есть в базе данных
        UpdateLocationForDay(day);

        // Обновляем изображение карты
        UpdateMapButtonImage();

        LoadEquipmentForDay(day);

        // Скрыть все панели дней
        planingDay1.SetActive(false);
        planingDay2.SetActive(false);
        planingDay3.SetActive(false);
        planingDay4.SetActive(false);

        // На каждую кнопку поставить изображение неактивной кнопки
        foreach (Button button in dayButtons)
        {
            button.GetComponent<Image>().sprite = inactiveButtonDay;
        }

        // Показать панель текущего дня
        switch (day)
        {
            case 1:
                planingDay1.SetActive(true);
                buttonDay1.GetComponent<Image>().sprite = activeButtonDay;
                DisplayIntervalsForDay(day);
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

    private void UpdateLocationForDay(int day)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            // Получаем id локации для текущего дня
            string getLocationQuery = "SELECT location_id FROM Schedules WHERE day = @day";
            int locationId = 0;

            using (var getLocationCommand = new SqliteCommand(getLocationQuery, connection))
            {
                getLocationCommand.Parameters.AddWithValue("@day", day);
                var result = getLocationCommand.ExecuteScalar();

                if (result != DBNull.Value && result != null)
                {
                    locationId = Convert.ToInt32(result);
                    Debug.Log($"Для дня {day} найдена локация с id {locationId}.");
                }
                else
                {
                    Debug.LogWarning($"Локация для дня {day} не найдена.");
                    currentLocation = null; // Сбрасываем текущую локацию
                    return; // Прерываем выполнение, если локация не найдена
                }
            }

            // Получаем название локации по id
            string getLocationNameQuery = "SELECT location_name FROM Locations WHERE id = @locationId";
            string locationName = "";

            using (var getLocationNameCommand = new SqliteCommand(getLocationNameQuery, connection))
            {
                getLocationNameCommand.Parameters.AddWithValue("@locationId", locationId);
                var result = getLocationNameCommand.ExecuteScalar();

                if (result != DBNull.Value && result != null)
                {
                    locationName = result.ToString();
                    currentLocation = locationName; // Обновляем текущую локацию
                    Debug.Log($"Текущая локация: {currentLocation}");
                }
                else
                {
                    Debug.LogWarning($"Не удалось найти локацию с id {locationId}.");
                    currentLocation = null; // Сбрасываем текущую локацию
                }
            }

            connection.Close();
        }
    }


    private void DisplayIntervalsForDay(int day)
    {
        // Отобразить интервалы для выбранного дня
        var intervals = selectedIntervalsPerDay[day];
        Debug.Log($"Интервалы для дня {day}: " + string.Join(", ", intervals));
    }

    public static void SaveTimeInterval(string character, string interval)
    {
        if (selectedTimeIntervals.ContainsKey(character))
        {
            string intervalWithDay = $"Day {currentDay}: {interval}";
            selectedTimeIntervals[character].Add(intervalWithDay);
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

    public void OpenMapScene()
    {
        PlayerPrefs.SetInt("CurrentDay", currentDay);
        SceneManager.LoadScene("MapScene");
    }
    static public void UpdateLocationInDatabase(string locationName)
    {
        // Открываем соединение с базой данных
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            // Проверяем, существует ли локация в таблице Locations
            string checkLocationQuery = "SELECT id FROM Locations WHERE location_name = @locationName";
            int locationId = 0;

            using (var checkCommand = new SqliteCommand(checkLocationQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@locationName", locationName);
                var result = checkCommand.ExecuteScalar();
                if (result != null)
                {
                    locationId = Convert.ToInt32(result);
                    Debug.Log($"Локация {locationName} уже существует в базе данных с id {locationId}.");
                }
                else
                {
                    // Локация не найдена, добавляем её
                    string insertLocationQuery = "INSERT INTO Locations (location_name) VALUES (@locationName)";
                    using (var insertCommand = new SqliteCommand(insertLocationQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@locationName", locationName);
                        insertCommand.ExecuteNonQuery();
                        // Получаем id только что добавленной локации
                        string getLastInsertIdQuery = "SELECT last_insert_rowid()";
                        using (var command = new SqliteCommand(getLastInsertIdQuery, connection))
                        {
                            locationId = Convert.ToInt32(command.ExecuteScalar());
                        }
                        Debug.Log($"Добавлена новая локация {locationName} с id {locationId}.");
                    }
                }
            }

            // Теперь обновляем таблицу Schedules для текущего дня
            string updateScheduleQuery = @"
            UPDATE Schedules
            SET location_id = @locationId
            WHERE day = @currentDay";

            using (var updateCommand = new SqliteCommand(updateScheduleQuery, connection))
            {
                updateCommand.Parameters.AddWithValue("@locationId", locationId);
                updateCommand.Parameters.AddWithValue("@currentDay", currentDay);
                updateCommand.ExecuteNonQuery();
                Debug.Log($"Обновлено местоположение для дня {currentDay} с id локации {locationId}.");
            }

            connection.Close();
        }
    }

}
