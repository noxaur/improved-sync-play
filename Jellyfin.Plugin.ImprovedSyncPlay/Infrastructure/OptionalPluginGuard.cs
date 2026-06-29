using System;
using MediaBrowser.Common.Plugins;

namespace Jellyfin.Plugin.ImprovedSyncPlay.Infrastructure;

/// <summary>
/// Detects co-installed plugins without assuming they are present.
/// </summary>
public sealed class OptionalPluginGuard
{
    private readonly IPluginManager _pluginManager;
    private readonly PluginLogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionalPluginGuard"/> class.
    /// </summary>
    /// <param name="pluginManager">The Jellyfin plugin manager.</param>
    /// <param name="logger">The plugin logger.</param>
    public OptionalPluginGuard(IPluginManager pluginManager, PluginLogger logger)
    {
        ArgumentNullException.ThrowIfNull(pluginManager);
        ArgumentNullException.ThrowIfNull(logger);

        _pluginManager = pluginManager;
        _logger = logger;
    }

    /// <summary>
    /// Returns whether a plugin is installed, enabled, and supported.
    /// </summary>
    /// <param name="pluginId">The plugin identifier.</param>
    /// <returns><c>true</c> when the plugin is available; otherwise <c>false</c>.</returns>
    public bool IsInstalled(Guid pluginId) => TryGetPlugin(pluginId, out _);

    /// <summary>
    /// Attempts to resolve an installed, enabled, and supported plugin.
    /// </summary>
    /// <param name="pluginId">The plugin identifier.</param>
    /// <param name="plugin">The resolved plugin, or <c>null</c> when absent.</param>
    /// <returns><c>true</c> when the plugin is available; otherwise <c>false</c>.</returns>
    public bool TryGetPlugin(Guid pluginId, out LocalPlugin? plugin)
    {
        plugin = _pluginManager.GetPlugin(pluginId);

        if (plugin is null || !plugin.IsEnabledAndSupported)
        {
            plugin = null;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Ensures a hard-required plugin is installed or throws after logging an error.
    /// </summary>
    /// <param name="pluginId">The required plugin identifier.</param>
    /// <param name="action">The action that cannot proceed without the plugin.</param>
    /// <exception cref="RequiredPluginMissingException">Thrown when the plugin is missing.</exception>
    public void RequirePlugin(Guid pluginId, string action)
    {
        if (IsInstalled(pluginId))
        {
            return;
        }

        _logger.Error($"Required plugin {pluginId} is not installed for action '{action}'.");
        throw new RequiredPluginMissingException(pluginId, action);
    }
}
