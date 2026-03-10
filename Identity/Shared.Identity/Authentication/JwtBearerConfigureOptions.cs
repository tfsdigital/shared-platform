using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Shared.Identity.Authentication;

public class JwtBearerConfigureOptions(IConfiguration configuration) : IConfigureNamedOptions<JwtBearerOptions>
{
    private const string ConfigurationSection = "Authentication";

    public void Configure(string? name, JwtBearerOptions options) =>
        Configure(options);

    public void Configure(JwtBearerOptions options) =>
        configuration.GetSection(ConfigurationSection).Bind(options);
}
