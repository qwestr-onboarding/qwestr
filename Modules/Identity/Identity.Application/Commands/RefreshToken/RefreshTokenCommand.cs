using Identity.Application.DTOs;
using MediatR;
using Shared.Kernel.Entities;

namespace Identity.Application.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<ActionResult<TokenResponse>>;
