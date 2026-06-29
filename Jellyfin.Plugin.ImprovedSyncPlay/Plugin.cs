using System;
using Jellyfin.Plugin.ImprovedSyncPlay.Configuration;
using Jellyfin.Plugin.ImprovedSyncPlay.Infrastructure;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.ImprovedSyncPlay;

/// <summary>
/// The main plugin.
/// </summary>
public class Plugin : BasePlugin<PluginConfiguration>
{
    /// <summary>
    /// This plugin's identifier.
    /// </summary>
    public static readonly Guid PluginId = Guid.Parse("a3f8c2e1-4b5d-6e7f-8a9b-0c1d2e3f4a5b");

    private readonly OptionalPluginGuard _guard;
    private readonly PluginLogger _pluginLogger;

    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin"/> class.
    /// </summary>
    /// <param name="applicationPaths">Instance of the <see cref="IApplicationPaths"/> interface.</param>
    /// <param name="xmlSerializer">Instance of the <see cref="IXmlSerializer"/> interface.</param>
    /// <param name="pluginManager">Instance of the <see cref="IPluginManager"/> interface.</param>
    /// <param name="logger">Instance of the <see cref="ILogger{Plugin}"/> interface.</param>
    public Plugin(
        IApplicationPaths applicationPaths,
        IXmlSerializer xmlSerializer,
        IPluginManager pluginManager,
        ILogger<Plugin> logger)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
        _pluginLogger = new PluginLogger(logger, Name);
        _guard = new OptionalPluginGuard(pluginManager, _pluginLogger);
        FileTransformationRegistrar.TryRegister(_guard, _pluginLogger, Id);
    }

    /// <inheritdoc />
    public override string Name => "Improved SyncPlay";

    /// <inheritdoc />
    public override Guid Id => PluginId;

    /// <summary>
    /// Gets the current plugin instance.
    /// </summary>
    public static Plugin? Instance { get; private set; }

}
