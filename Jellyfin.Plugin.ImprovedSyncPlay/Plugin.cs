using System;
using Jellyfin.Plugin.ImprovedSyncPlay.Configuration;
using Jellyfin.Plugin.ImprovedSyncPlay.Infrastructure;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.ImprovedSyncPlay;

public class Plugin : BasePlugin<PluginConfiguration>
{
    private static readonly Guid ExampleOptionalPluginId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly OptionalPluginGuard _guard;
    private readonly PluginLogger _pluginLogger;

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

    public override string Name => "Improved SyncPlay";
    public override Guid Id => Guid.Parse("a3f8c2e1-4b5d-6e7f-8a9b-0c1d2e3f4a5b");
    public static Plugin? Instance { get; private set; }

    private void TryOptionalCoInstalledFeature()
    {
        if (!_guard.IsInstalled(ExampleOptionalPluginId))
        {
            _pluginLogger.LogOptionalPluginMissing(
                ExampleOptionalPluginId,
                "optional co-installed feature",
                "Optional enrichment disabled.");
        }
    }
}
