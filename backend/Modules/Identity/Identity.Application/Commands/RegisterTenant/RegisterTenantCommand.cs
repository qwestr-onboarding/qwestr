using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.RegisterTenant;

public record RegisterTenantCommand(
    string Name,
    string? Subdomain,
    string OwnerEmail,
    string OwnerPassword,
    string OwnerFirstName,
    string OwnerLastName) : IRequest<ActionResult<TenantRegisteredResponse>>;
