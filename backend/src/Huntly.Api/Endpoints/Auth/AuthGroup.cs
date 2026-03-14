using FastEndpoints;

namespace Huntly.Api.Endpoints.Auth;

public sealed class AuthGroup : Group
{
    public AuthGroup()
    {
        Configure("auth", ep => { });
    }
}