using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic; 
using System.Data;
using System.IO;
using Mono.Data.Sqlite; // Подключение к SQLite



public class DatabaseManager : MonoBehaviour
{
    private string connectionString = "URI=file:" + Application.dataPath + "/MyDatabase.db";

    private void Start()
    {
        CreateTables(); // Создаем таблицы при старте
    }

    public void CreateTables()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            string createEquipmentTable = "CREATE TABLE IF NOT EXISTS Equipment (Id INTEGER PRIMARY KEY AUTOINCREMENT, SlotName TEXT)";
            string createSchedulesTable = "CREATE TABLE IF NOT EXISTS Schedules (Id INTEGER PRIMARY KEY AUTOINCREMENT, Day INTEGER, Character TEXT)";
            string createAssignmentsTable = "CREATE TABLE IF NOT EXISTS Assignments (Id INTEGER PRIMARY KEY AUTOINCREMENT, Character TEXT, Type TEXT, Task TEXT)";

            using (var command = new SqliteCommand(createEquipmentTable, connection))
            {
                command.ExecuteNonQuery();
            }
            using (var command = new SqliteCommand(createSchedulesTable, connection))
            {
                command.ExecuteNonQuery();
            }
            using (var command = new SqliteCommand(createAssignmentsTable, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    // Метод для добавления экипировки в таблицу
    public void InsertEquipment(string slotName)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            string insertQuery = "INSERT INTO Equipment (SlotName) VALUES (@SlotName)";
            using (var command = new SqliteCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@SlotName", slotName);
                command.ExecuteNonQuery();
            }
        }
    }

    // Метод для добавления расписания в таблицу
    public void InsertSchedule(int day, string character)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            string insertQuery = "INSERT INTO Schedules (Day, Character) VALUES (@Day, @Character)";
            using (var command = new SqliteCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@Day", day);
                command.Parameters.AddWithValue("@Character", character);
                command.ExecuteNonQuery();
            }
        }
    }

    // Метод для добавления задания в таблицу
    public void InsertAssignment(string character, string taskType)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            string insertQuery = "INSERT INTO Assignments (Character, Type, Task) VALUES (@Character, @Type, @Task)";
            using (var command = new SqliteCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@Character", character);
                command.Parameters.AddWithValue("@Type", taskType);
                command.Parameters.AddWithValue("@Task", ""); // Затычка для задачи
                command.ExecuteNonQuery();
            }
        }
    }
}
