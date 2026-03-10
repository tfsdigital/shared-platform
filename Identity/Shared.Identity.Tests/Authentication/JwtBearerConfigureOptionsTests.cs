using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Shared.Identity.Authentication;

namespace Shared.Identity.Tests.Authentication;

public class JwtBearerConfigureOptionsTests
{
    [Fact]
    public void Configure_WithoutName_ShouldBindConfigurationSection()
    {
        // Arrange
        var configuration = CreateMockConfiguration();
        var configureOptions = new JwtBearerConfigureOptions(configuration);
        var options = new JwtBearerOptions();

        // Act
        configureOptions.Configure(options);

        // Assert
        configuration.Received(1).GetSection("Authentication");
    }

    [Fact]
    public void Configure_WithName_ShouldBindConfigurationSection()
    {
        // Arrange
        var configuration = CreateMockConfiguration();
        var configureOptions = new JwtBearerConfigureOptions(configuration);
        var options = new JwtBearerOptions();
        const string schemeName = "Bearer";

        // Act
        configureOptions.Configure(schemeName, options);

        // Assert
        configuration.Received(1).GetSection("Authentication");
    }

    [Fact]
    public void Configure_WithNullName_ShouldBindConfigurationSection()
    {
        // Arrange
        var configuration = CreateMockConfiguration();
        var configureOptions = new JwtBearerConfigureOptions(configuration);
        var options = new JwtBearerOptions();

        // Act
        configureOptions.Configure(null, options);

        // Assert
        configuration.Received(1).GetSection("Authentication");
    }

    [Fact]
    public void Configure_WithNullConfiguration_ShouldThrowNullReferenceException()
    {
        // Arrange
        var configureOptions = new JwtBearerConfigureOptions(null!);
        var options = new JwtBearerOptions();

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => configureOptions.Configure(options));
    }

    private static IConfiguration CreateMockConfiguration()
    {
        var configuration = Substitute.For<IConfiguration>();
        var configurationSection = Substitute.For<IConfigurationSection>();

        configuration.GetSection("Authentication").Returns(configurationSection);

        return configuration;
    }
}
