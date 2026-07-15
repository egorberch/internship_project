namespace InternshipApp.Models;

// Пользователь и список его ролей (LEFT JOIN + ARRAY_AGG)
public class UserWithRoles
{
    public int id { get; set; }
    public string user_name { get; set; } = string.Empty;
    public string[] role_names { get; set; } = [];
}
