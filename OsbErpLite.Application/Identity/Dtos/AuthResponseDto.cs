namespace OsbErpLite.Application.Identity.Dtos;

public record AuthResponseDto(
    string Token,
    string Username,
    string Role,
    DateTime ExpiresAt);
