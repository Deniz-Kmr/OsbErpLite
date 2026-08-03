using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Identity.Dtos;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.Identity.Commands;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    public UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper, ILogger<UpdateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"User with Id '{request.Id}' was not found.");

        if (request.CurrentUserId == request.Id && (!request.IsActive || request.Role != UserRole.Admin))
            throw new InvalidOperationException("Kendi hesabınızı devre dışı bırakamaz veya Admin rolünüzü değiştiremezsiniz.");

        var isDemotingOrDeactivatingAdmin = user.Role == UserRole.Admin && user.IsActive
            && (request.Role != UserRole.Admin || !request.IsActive);

        if (isDemotingOrDeactivatingAdmin)
        {
            var activeAdminCount = await _userRepository.GetActiveAdminCountAsync(cancellationToken);
            if (activeAdminCount <= 1)
                throw new InvalidOperationException("Sistemde en az bir aktif Admin bulunmalıdır.");
        }

        user.Role = request.Role;
        user.IsActive = request.IsActive;
        user.EmployeeId = request.EmployeeId;

        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("Admin {AdminId} updated user {Username} (Id: {UserId}): Role={Role}, IsActive={IsActive}",
            request.CurrentUserId, user.Username, user.Id, user.Role, user.IsActive);

        return _mapper.Map<UserDto>(user);
    }
}
