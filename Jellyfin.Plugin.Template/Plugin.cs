using System;
using Jellyfin.Plugin.Template.Configuration;
using Jellyfin.Plugin.Template.Infrastructure;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Template;

/// <summary>
/// The main plugin.
/// </summary>
public class Plugin : BasePlugin<PluginConfiguration>
{
    // Replace with a real ecosystem plugin GUID from docs/catalog when integrating.
    private static readonly Guid ExampleOptionalPluginId = Guid.Parse("00000000-0000-0000-0000-000000000001");

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
        TryOptionalCoInstalledFeature();
    }

    /// <inheritdoc />
    public override string Name => "Template";

    /// <inheritdoc />
    public override Guid Id => Guid.Parse("eb5d7894-8eef-4b36-aa6f-5d124e828ce1");

    /// <summary>
    /// Gets the current plugin instance.
    /// </summary>
    public static Plugin? Instance { get; private set; }

    private void TryOptionalCoInstalledFeature()
    {
        if (!_guard.IsInstalled(ExampleOptionalPluginId))
        {
            _pluginLogger.LogOptionalPluginMissing(
                ExampleOptionalPluginId,
                "optional co-installed feature",
                "Optional enrichment disabled.");
            return;
        }

        // Optional integration would run here when the co-installed plugin is present.
    }
}
