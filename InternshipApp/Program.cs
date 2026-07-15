using Dapper;
using Npgsql;
using KingMigrations.PostgreSql;
using KingMigrations.MigrationSources;
using KingMigrations.MigrationParsers;
using DotNetEnv;
using InternshipApp.Models;

const string EnvFilePath = "../.env";

// Подключение к БД
if (!File.Exists(EnvFilePath))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Ошибка: Файл .env не найден!");
    Console.ResetColor();
    Console.WriteLine("Пожалуйста, скопируйте файл .env_example в .env в корне проекта и заполните параметры подключения.");
    return;
}

Env.Load(EnvFilePath);

string connectionString = $"Host={Env.GetString("DB_HOST")};" +
                          $"Port={Env.GetString("DB_PORT")};" +
                          $"Database={Env.GetString("DB_NAME")};" +
                          $"Username={Env.GetString("DB_USER")};" +
                          $"Password={Env.GetString("DB_PASSWORD")}";

using var connection = new NpgsqlConnection(connectionString);

try
{
    connection.Open();
    Console.WriteLine("Подключение к базе данных установлено.");
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Ошибка подключения к БД: {ex.Message}");
    Console.ResetColor();
    return;
}

// Применение миграций (таблицы + тестовые данные)
try
{
    var migrationApplier = new PostgreSqlMigrationApplier();
    var migrationSource = new DirectoryMigrationSource("Migrations");
    migrationSource.AddParser(".sql", new SemicolonDelimitedMigrationParser());

    await migrationApplier.ApplyMigrationsAsync(connection, migrationSource);

    Console.WriteLine("Миграции успешно применены.\n");
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка при применении миграций: {ex.Message}\n");
}

// SQL-запросы
const string sql_task1 = """
    SELECT u.id, u.name AS user_name, ARRAY_AGG(r.name) AS role_names
    FROM Users u
    LEFT JOIN UserRoles ur ON u.id = ur.user_id
    LEFT JOIN Roles r ON ur.role_id = r.id
    GROUP BY u.id, u.name
    ORDER BY u.name
    """;

const string sql_task2 = """
    SELECT r.id, r.name, COUNT(ur.user_id) AS user_count
    FROM Roles r
    INNER JOIN UserRoles ur ON r.id = ur.role_id
    GROUP BY r.id, r.name
    ORDER BY user_count DESC
    """;

// Меню выбора задания
string? answer = null;

while (answer != "0")
{
    Console.WriteLine("Выберите действие:");
    Console.WriteLine("1 - Пользователи и их роли");
    Console.WriteLine("2 - Количество пользователей по ролям");
    Console.WriteLine("0 - Выход");
    Console.Write("\n> ");
    answer = Console.ReadLine();

    if (answer == "1")
    {
        Console.WriteLine("\n=== Задача 1: Пользователи и их роли ===");

        var usersWithRoles = connection.Query<UserWithRoles>(sql_task1).ToList();

        foreach (var user in usersWithRoles)
        {
            if (user.role_names.Length > 0)
            {
                Console.WriteLine($"{user.user_name,-15} — {string.Join(", ", user.role_names)}");
            }
            else
            {
                Console.WriteLine($"{user.user_name,-15} — Нет ролей");
            }
        }

        Console.WriteLine();
    }
    else if (answer == "2")
    {
        Console.WriteLine("\n=== Задача 2: Количество пользователей по ролям ===");

        var roleCount = connection.Query<RoleCount>(sql_task2).ToList();

        foreach (var item in roleCount)
        {
            Console.WriteLine($"{item.name,-15} - {item.user_count} пользователь(ей)");
        }

        Console.WriteLine();
    }
    else if (answer == "0")
    {
        Console.WriteLine("\nКонец программы.");
    }
    else
    {
        Console.WriteLine("Неверный ввод. Попробуйте снова.\n");
    }
}
