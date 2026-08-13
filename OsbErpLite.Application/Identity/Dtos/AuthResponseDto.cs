namespace OsbErpLite.Application.Identity.Dtos;

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    string Username,
    string Role,
    DateTime ExpiresAt);
