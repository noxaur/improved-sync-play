using Jellyfin.Plugin.ImprovedSyncPlay.Infrastructure;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using Microsoft.Extensions.Logging;
using Moq;

namespace Jellyfin.Plugin.ImprovedSyncPlay.Tests;

public class OptionalPluginGuardTests
{
    private const string PluginName = "Improved SyncPlay";
    private static readonly Guid PluginId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    [Fact]
    public void IsInstalled_ReturnsFalse_WhenPluginMissing()
    {
        var pluginManager = new Mock<IPluginManager>();
        pluginManager.Setup(m => m.GetPlugin(PluginId, null)).Returns((LocalPlugin?)null);

        var guard = CreateGuard(pluginManager.Object);

        Assert.False(guard.IsInstalled(PluginId));
        Assert.False(guard.TryGetPlugin(PluginId, out var plugin));
        Assert.Null(plugin);
    }

    [Fact]
    public void IsInstalled_ReturnsFalse_WhenPluginDisabledOrUnsupported()
    {
        var pluginManager = new Mock<IPluginManager>();
        pluginManager
            .Setup(m => m.GetPlugin(PluginId, null))
            .Returns(CreateLocalPlugin(isSupported: false, PluginStatus.Active));

        var guard = CreateGuard(pluginManager.Object);

        Assert.False(guard.IsInstalled(PluginId));
        Assert.False(guard.TryGetPlugin(PluginId, out _));
    }

    [Fact]
    public void IsInstalled_ReturnsTrue_WhenPluginEnabledAndSupported()
    {
        var localPlugin = CreateLocalPlugin(isSupported: true, PluginStatus.Active);
        var pluginManager = new Mock<IPluginManager>();
        pluginManager.Setup(m => m.GetPlugin(PluginId, null)).Returns(localPlugin);

        var guard = CreateGuard(pluginManager.Object);

        Assert.True(guard.IsInstalled(PluginId));
        Assert.True(guard.TryGetPlugin(PluginId, out var plugin));
        Assert.Same(localPlugin, plugin);
    }

    [Fact]
    public void RequirePlugin_Throws_WhenPluginMissing()
    {
        var pluginManager = new Mock<IPluginManager>();
        pluginManager.Setup(m => m.GetPlugin(PluginId, null)).Returns((LocalPlugin?)null);

        var logger = new ListLogger();
        var guard = new OptionalPluginGuard(pluginManager.Object, new PluginLogger(logger, PluginName));

        var exception = Assert.Throws<RequiredPluginMissingException>(
            () => guard.RequirePlugin(PluginId, "import library"));

        Assert.Equal(PluginId, exception.PluginId);
        Assert.Equal("import library", exception.Action);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Contains("import library", entry.Message, StringComparison.Ordinal);
    }

    private static OptionalPluginGuard CreateGuard(IPluginManager pluginManager)
    {
        var logger = new ListLogger();
        return new OptionalPluginGuard(pluginManager, new PluginLogger(logger, PluginName));
    }

    private static LocalPlugin CreateLocalPlugin(bool isSupported, PluginStatus status)
    {
        var manifest = new PluginManifest
        {
            Id = PluginId,
            Name = "Example Plugin",
            Version = "1.0.0.0",
            Status = status,
        };

        return new LocalPlugin("/plugins/example", isSupported, manifest);
    }
}
