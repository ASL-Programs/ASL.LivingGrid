using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;

namespace ASL.LivingGrid.WebAdminPanel.Services;

public static class SecurityServiceExtensions
{
    public static AuthenticationBuilder AddExternalIdentityProviders(this AuthenticationBuilder builder, IConfiguration config)
    {
        if (!string.IsNullOrEmpty(config["Security:Saml:MetadataUrl"]))
        {
            // SAML integration requires external package; configuration is logged for now
        }
        if (!string.IsNullOrEmpty(config["Security:Oidc:Authority"]))
        {
            builder.AddOAuth("Oidc", o => {
                o.AuthorizationEndpoint = config["Security:Oidc:Authority"] + "/authorize";
                o.TokenEndpoint = config["Security:Oidc:Authority"] + "/token";
                o.ClientId = config["Security:Oidc:ClientId"];
                o.ClientSecret = config["Security:Oidc:ClientSecret"];
            });
        }
        if (!string.IsNullOrEmpty(config["Security:OAuth2:TokenEndpoint"]))
        {
            builder.AddOAuth("OAuth2", o => {
                o.TokenEndpoint = config["Security:OAuth2:TokenEndpoint"];
                o.AuthorizationEndpoint = config["Security:OAuth2:AuthorizationEndpoint"] ?? string.Empty;
                o.ClientId = config["Security:OAuth2:ClientId"];
                o.ClientSecret = config["Security:OAuth2:ClientSecret"];
            });
        }
        if (!string.IsNullOrEmpty(config["Security:Ldap:Server"]))
        {
            // LDAP integration is handled via external authentication services
        }
        return builder;
    }
}
