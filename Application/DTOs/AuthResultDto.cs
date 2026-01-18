namespace Application.DTOs;

public class AuthResultDto
{
    public bool IsSuccess { get; set; }
    public string? Token { get; set; }
    public string? Error { get; set; }

    public static AuthResultDto Fail(string e) => new AuthResultDto { IsSuccess = false, Error = e };
    public static AuthResultDto Success(string token) => new AuthResultDto { IsSuccess = true, Token = token };
}
