using Identity.Application.DTOs;
using MediatR;
using Shared.Kernel.Entities;

namespace Identity.Application.Commands.RegisterTenant;

public record RegisterTenantCommand(
    string Name,
    string? Subdomain,
    string OwnerEmail,
    string OwnerPassword,
    string OwnerFirstName,
    string OwnerLastName) : IRequest<ActionResult<TenantRegisteredResponse>>;
