using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Identity.Dtos;
using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.Identity.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken)
            ?? throw new KeyNotFoundException("Invalid username or password.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("User account is deactivated.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new KeyNotFoundException("Invalid username or password.");

        var accessToken = _jwtTokenService.GenerateToken(user);
        var expiry = _jwtTokenService.GetAccessTokenExpiry();

        var refreshToken = new RefreshToken
        {
            Token = _jwtTokenService.GenerateRefreshToken(),
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtTokenService.GetRefreshTokenExpiryDays()),
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        return new AuthResponseDto(accessToken, refreshToken.Token, user.Username, user.Role.ToString(), expiry);
    }
}
