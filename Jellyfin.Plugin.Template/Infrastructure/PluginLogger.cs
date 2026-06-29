using System;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Template.Infrastructure;

/// <summary>
/// Tiered logger with a consistent plugin name prefix.
/// </summary>
public sealed partial class PluginLogger
{
    private readonly ILogger _logger;
    private readonly string _pluginName;

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginLogger"/> class.
    /// </summary>
    /// <param name="logger">The underlying logger.</param>
    /// <param name="pluginName">The plugin display name used in log prefixes.</param>
    public PluginLogger(ILogger logger, string pluginName)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentException.ThrowIfNullOrWhiteSpace(pluginName);

        _logger = logger;
        _pluginName = pluginName;
    }

    /// <summary>
    /// Logs a verbose message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Verbose(string message) => LogVerbose(_logger, _pluginName, message);

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Debug(string message) => LogDebug(_logger, _pluginName, message);

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Info(string message) => LogInformation(_logger, _pluginName, message);

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Warning(string message) => LogWarning(_logger, _pluginName, message);

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Error(string message) => LogError(_logger, _pluginName, message);

    /// <summary>
    /// Logs context when an optional co-installed plugin is missing.
    /// </summary>
    /// <param name="pluginId">The missing plugin identifier.</param>
    /// <param name="action">The action that was skipped or degraded.</param>
    /// <param name="userMessage">A short user-facing explanation.</param>
    public void LogOptionalPluginMissing(Guid pluginId, string action, string userMessage) =>
        LogOptionalPluginMissingWarning(_logger, _pluginName, pluginId, action, userMessage);

    [LoggerMessage(Level = LogLevel.Trace, Message = "[{PluginName}] {Message}")]
    private static partial void LogVerbose(ILogger logger, string pluginName, string message);

    [LoggerMessage(Level = LogLevel.Debug, Message = "[{PluginName}] {Message}")]
    private static partial void LogDebug(ILogger logger, string pluginName, string message);

    [LoggerMessage(Level = LogLevel.Information, Message = "[{PluginName}] {Message}")]
    private static partial void LogInformation(ILogger logger, string pluginName, string message);

    [LoggerMessage(Level = LogLevel.Warning, Message = "[{PluginName}] {Message}")]
    private static partial void LogWarning(ILogger logger, string pluginName, string message);

    [LoggerMessage(Level = LogLevel.Error, Message = "[{PluginName}] {Message}")]
    private static partial void LogError(ILogger logger, string pluginName, string message);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "[{PluginName}] Optional plugin {PluginId} is not installed for action '{Action}': {UserMessage}")]
    private static partial void LogOptionalPluginMissingWarning(
        ILogger logger,
        string pluginName,
        Guid pluginId,
        string action,
        string userMessage);
}
