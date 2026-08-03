using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Identity.Dtos;

namespace OsbErpLite.Application.Identity.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(IUserRepository userRepository, IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken)
            ?? throw new KeyNotFoundException("Invalid username or password.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("User account is deactivated.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new KeyNotFoundException("Invalid username or password.");

        var token = _jwtTokenService.GenerateToken(user);
        var expiry = _jwtTokenService.GetExpiry();

        return new AuthResponseDto(token, user.Username, user.Role.ToString(), expiry);
    }
}
