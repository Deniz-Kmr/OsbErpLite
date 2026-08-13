using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Identity.Dtos;
using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.Identity.Commands;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var existingToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid refresh token.");

        if (existingToken.IsRevoked)
            throw new UnauthorizedAccessException("Refresh token has been revoked.");

        if (existingToken.ExpiresAt <= DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token has expired.");

        var user = await _userRepository.GetByIdAsync(existingToken.UserId, cancellationToken)
            ?? throw new UnauthorizedAccessException("User not found.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("User account is deactivated.");

        // rotation: eski token'i revoke et
        existingToken.IsRevoked = true;
        await _refreshTokenRepository.UpdateAsync(existingToken, cancellationToken);

        var accessToken = _jwtTokenService.GenerateToken(user);
        var expiry = _jwtTokenService.GetAccessTokenExpiry();

        var newRefreshToken = new RefreshToken
        {
            Token = _jwtTokenService.GenerateRefreshToken(),
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtTokenService.GetRefreshTokenExpiryDays()),
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

        return new AuthResponseDto(accessToken, newRefreshToken.Token, user.Username, user.Role.ToString(), expiry);
    }
}
