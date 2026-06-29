using Jellyfin.Plugin.ImprovedSyncPlay.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.ImprovedSyncPlay.Tests;

public class PluginLoggerTests
{
    [Theory]
    [InlineData(nameof(PluginLogger.Verbose), LogLevel.Trace)]
    [InlineData(nameof(PluginLogger.Debug), LogLevel.Debug)]
    [InlineData(nameof(PluginLogger.Info), LogLevel.Information)]
    [InlineData(nameof(PluginLogger.Warning), LogLevel.Warning)]
    [InlineData(nameof(PluginLogger.Error), LogLevel.Error)]
    public void TierMethods_EmitExpectedLogLevel(string methodName, LogLevel expectedLevel)
    {
        var logger = new ListLogger();
        var pluginLogger = new PluginLogger(logger, "Template");

        Invoke(pluginLogger, methodName, "hello");

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(expectedLevel, entry.Level);
        Assert.Contains("[Template]", entry.Message, StringComparison.Ordinal);
        Assert.Contains("hello", entry.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void LogOptionalPluginMissing_EmitsWarningWithContext()
    {
        var logger = new ListLogger();
        var pluginLogger = new PluginLogger(logger, "Template");
        var pluginId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        pluginLogger.LogOptionalPluginMissing(pluginId, "sync metadata", "Feature disabled.");

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Warning, entry.Level);
        Assert.Contains(pluginId.ToString(), entry.Message, StringComparison.Ordinal);
        Assert.Contains("sync metadata", entry.Message, StringComparison.Ordinal);
        Assert.Contains("Feature disabled.", entry.Message, StringComparison.Ordinal);
    }

    private static void Invoke(PluginLogger pluginLogger, string methodName, string message)
    {
        switch (methodName)
        {
            case nameof(PluginLogger.Verbose):
                pluginLogger.Verbose(message);
                break;
            case nameof(PluginLogger.Debug):
                pluginLogger.Debug(message);
                break;
            case nameof(PluginLogger.Info):
                pluginLogger.Info(message);
                break;
            case nameof(PluginLogger.Warning):
                pluginLogger.Warning(message);
                break;
            case nameof(PluginLogger.Error):
                pluginLogger.Error(message);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(methodName), methodName, null);
        }
    }
}
