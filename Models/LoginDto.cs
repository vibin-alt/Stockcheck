using System.ComponentModel.DataAnnotations.Schema;

public class LoginDto
{
    public string UserName { get; set; } = string.Empty;
    public string PassWord { get; set; } = string.Empty;
}

public class LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    [NotMapped]
    public string? Data { get; set; }        // This will contain the JSON string
}

public class UserData
{
    public string UserName { get; set; } = string.Empty;
    public DateTime LoginTime { get; set; }
    public string account { get; set; } = string.Empty;
    public string accdesc { get; set; } = string.Empty;
    public string trn { get; set; }= string.Empty;
}