using System;

namespace Jellyfin.Plugin.Template.Infrastructure;

/// <summary>
/// Thrown when a hard-required co-installed plugin is missing.
/// </summary>
public sealed class RequiredPluginMissingException : InvalidOperationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredPluginMissingException"/> class.
    /// </summary>
    public RequiredPluginMissingException()
        : base("A required plugin is missing.")
    {
        PluginId = Guid.Empty;
        Action = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredPluginMissingException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public RequiredPluginMissingException(string message)
        : base(message)
    {
        PluginId = Guid.Empty;
        Action = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredPluginMissingException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public RequiredPluginMissingException(string message, Exception innerException)
        : base(message, innerException)
    {
        PluginId = Guid.Empty;
        Action = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredPluginMissingException"/> class.
    /// </summary>
    /// <param name="pluginId">The required plugin identifier.</param>
    /// <param name="action">The action that cannot proceed.</param>
    public RequiredPluginMissingException(Guid pluginId, string action)
        : base($"Required plugin '{pluginId}' is not installed for action '{action}'.")
    {
        PluginId = pluginId;
        Action = action;
    }

    /// <summary>
    /// Gets the required plugin identifier.
    /// </summary>
    public Guid PluginId { get; }

    /// <summary>
    /// Gets the action that cannot proceed.
    /// </summary>
    public string Action { get; }
}
