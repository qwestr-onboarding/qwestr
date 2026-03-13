using MediatR;
using Qwestr.Modules.Identity.Application.DTOs;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<ActionResult<TokenResponse>>;
