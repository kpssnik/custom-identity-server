﻿using AutoMapper;
using IdentityServer.Api.Models.User;
using IdentityServer.Application.Requests.User.Commands.RegisterUser;
using IdentityServer.Application.Requests.User.Queries.GetUserJwt;
using IdentityServer.Application.Requests.User.Queries.LoginUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Api.Controllers;

// Default controller
[ApiController]
public class UserController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public UserController(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper;
        _mediator = mediator;
    }

    // Register user and return his jwt
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserModel model)
    {
        // register user and return his id for getting jwt
        var registerUserCommand = _mapper.Map<RegisterUserCommand>(model);
        var registeredUserId = await _mediator.Send(registerUserCommand);

        // get user jwt token
        var getUserJwtQuery = new GetUserJwtQuery() { UserId = registeredUserId };
        var userToken = await _mediator.Send(getUserJwtQuery);

        // return user jwt
        return Ok(userToken);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserModel model)
    {
        var loginUserQuery = _mapper.Map<LoginUserQuery>(model);
        var userId = await _mediator.Send(loginUserQuery);

        var getUserJwtQuery = new GetUserJwtQuery { UserId = userId };
        var userToken = await _mediator.Send(getUserJwtQuery);

        return Ok(userToken);
    }
}