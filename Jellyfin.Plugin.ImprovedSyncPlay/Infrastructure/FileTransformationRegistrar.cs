using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using Jellyfin.Plugin.ImprovedSyncPlay.Web;
using Newtonsoft.Json.Linq;

namespace Jellyfin.Plugin.ImprovedSyncPlay.Infrastructure;

/// <summary>
/// Registers index.html transformations with the File Transformation plugin via reflection.
/// </summary>
public static class FileTransformationRegistrar
{
    /// <summary>
    /// Published GUID for File Transformation when available in the plugin catalog.
    /// </summary>
    public static readonly Guid FileTransformationPluginId = Guid.Parse("5e87cc92-571a-4d8d-8d98-d2d4147f9f90");

    private const string TransformationInterfaceTypeName = "Jellyfin.Plugin.FileTransformation.PluginInterface";
    private const string RegisterTransformationMethodName = "RegisterTransformation";
    private static int _registered;

    /// <summary>
    /// Attempts to register the index.html script injection with File Transformation.
    /// </summary>
    /// <param name="guard">Optional plugin guard for catalog GUID detection.</param>
    /// <param name="logger">Plugin logger.</param>
    /// <param name="pluginId">This plugin's identifier used as the transformation id.</param>
    public static void TryRegister(OptionalPluginGuard guard, PluginLogger logger, Guid pluginId)
    {
        ArgumentNullException.ThrowIfNull(guard);
        ArgumentNullException.ThrowIfNull(logger);

        if (Interlocked.Exchange(ref _registered, 1) == 1)
        {
            return;
        }

        if (!IsFileTransformationAvailable(guard))
        {
            logger.LogOptionalPluginMissing(
                FileTransformationPluginId,
                "SyncPlay share button web injection",
                "Share button UI disabled.");
            Interlocked.Exchange(ref _registered, 0);
            return;
        }

        Assembly? fileTransformationAssembly = FindFileTransformationAssembly();
        if (fileTransformationAssembly is null)
        {
            logger.Info("File Transformation assembly not loaded; skipping web injection.");
            Interlocked.Exchange(ref _registered, 0);
            return;
        }

        Type? pluginInterfaceType = fileTransformationAssembly.GetType(TransformationInterfaceTypeName);
        MethodInfo? registerMethod = pluginInterfaceType?.GetMethod(
            RegisterTransformationMethodName,
            BindingFlags.Public | BindingFlags.Static);

        if (registerMethod is null)
        {
            logger.Warning("File Transformation RegisterTransformation method not found; skipping web injection.");
            Interlocked.Exchange(ref _registered, 0);
            return;
        }

        Assembly pluginAssembly = typeof(IndexHtmlTransform).Assembly;
        var payload = new JObject
        {
            ["id"] = pluginId.ToString(),
            ["fileNamePattern"] = @"index\.html$",
            ["callbackAssembly"] = pluginAssembly.FullName,
            ["callbackClass"] = typeof(IndexHtmlTransform).FullName,
            ["callbackMethod"] = nameof(IndexHtmlTransform.Transform),
        };

        try
        {
            registerMethod.Invoke(null, new object?[] { payload });
            logger.Info("Registered SyncPlay share button index.html transformation.");
        }
        catch (Exception ex) when (ex is TargetInvocationException or MemberAccessException or ArgumentException or InvalidOperationException)
        {
            Interlocked.Exchange(ref _registered, 0);
            var message = ex is TargetInvocationException tie ? tie.InnerException?.Message ?? tie.Message : ex.Message;
            logger.Warning($"Failed to register File Transformation callback: {message}");
        }
    }

    private static bool IsFileTransformationAvailable(OptionalPluginGuard guard)
    {
        if (guard.IsInstalled(FileTransformationPluginId))
        {
            return true;
        }

        return FindFileTransformationAssembly() is not null;
    }

    private static Assembly? FindFileTransformationAssembly()
    {
        return AssemblyLoadContext.All
            .SelectMany(context => context.Assemblies)
            .FirstOrDefault(assembly =>
                assembly.FullName?.Contains(".FileTransformation", StringComparison.Ordinal) == true);
    }
}
