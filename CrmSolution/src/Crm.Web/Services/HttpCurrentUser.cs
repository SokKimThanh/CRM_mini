using System;
using System.Security.Claims;
using Crm.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

namespace Crm.Web.Services;

public sealed class HttpCurrentUser : ICurrentUser
{
    private readonly AuthenticationStateProvider _auth;

    public HttpCurrentUser(AuthenticationStateProvider auth) => _auth = auth;

    private ClaimsPrincipal User =>
        _auth.GetAuthenticationStateAsync().GetAwaiter().GetResult().User;

    public Guid UserId =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var g) ? g : Guid.Empty;

    public Guid? TeamId =>
        Guid.TryParse(User.FindFirstValue("team_id"), out var g) ? g : null;

    public string Role => User.FindFirstValue(ClaimTypes.Role) ?? "SALES";
}
