﻿using IdentityServer.Application.Common.Exceptions;
using IdentityServer.Application.Common.Hashing;
using IdentityServer.Application.Interfaces;
using IdentityServer.Domain.IdentityUser;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Application.Requests.SA.Commands.CreateAdmin;

public class CreateAdminCommandHandler : IRequestHandler<CreateAdminCommand>
{
    private readonly IIdentityDbContext _context;

    public CreateAdminCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(CreateAdminCommand request, CancellationToken cancellationToken)
    {
        // Try to get user with given username or email
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(user =>
                    user.Username == request.Username && user.Email == request.Email,
                cancellationToken);

        // If it is already exists - ex
        if (existingUser is not null)
            throw new UserAlreadyExistsException(request.Username, request?.Email);

        // If its ok

        var adminRole = await _context.Roles
            .FirstOrDefaultAsync(role => role.Name == "admin", cancellationToken);

        _context.Users.Add(new IdentityUser
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = Sha256.Hash(request.Password),
            RoleId = adminRole.Id,
            Personal = new IdentityUserPersonal
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber
            }
        });

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}