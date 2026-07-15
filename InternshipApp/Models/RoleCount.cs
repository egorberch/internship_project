namespace InternshipApp.Models;

// Роль и количество пользователей в ней (INNER JOIN + GROUP BY)
public class RoleCount
{
    public int id { get; set; }
    public string name { get; set; } = string.Empty;
    public int user_count { get; set; }
}
