using Serilog;

namespace OsbErpLite.Api.Extensions;

public static class LoggingServiceExtensions
{
    public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration));

        return builder;
    }
}
