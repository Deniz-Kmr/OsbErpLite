using MediatR;
using OsbErpLite.Application.Common.Interfaces;

namespace OsbErpLite.Application.Identity.Commands;

public class RevokeRefreshTokenCommandHandler : IRequestHandler<RevokeRefreshTokenCommand, Unit>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public RevokeRefreshTokenCommandHandler(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<Unit> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken)
            ?? throw new KeyNotFoundException("Refresh token not found.");

        if (token.IsRevoked)
            return Unit.Value;

        token.IsRevoked = true;
        await _refreshTokenRepository.UpdateAsync(token, cancellationToken);

        return Unit.Value;
    }
}
