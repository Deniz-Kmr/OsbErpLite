namespace OsbErpLite.Application.Identity.Dtos;

public record AuthResponseDto(
    string Token,
    string RefreshToken,
    string Username,
    string Role,
    DateTime ExpiresAt);
