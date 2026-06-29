using System;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Jellyfin.Plugin.ImprovedSyncPlay.Web;

/// <summary>
/// Injects SyncPlay share client script into index.html via File Transformation.
/// </summary>
public static class IndexHtmlTransform
{
    private const string ScriptResourceName = "Jellyfin.Plugin.ImprovedSyncPlay.Web.syncplay-share.js";
    private const string InjectionMarker = "improved-syncplay-share-script";
    private static readonly Lazy<string> ScriptContent = new(LoadScript);

    /// <summary>
    /// Transforms index.html by injecting the SyncPlay share script before the closing body tag.
    /// </summary>
    /// <param name="context">File Transformation payload with a <c>contents</c> property.</param>
    /// <returns>Transformed HTML contents.</returns>
    public static string Transform(JObject context)
    {
        ArgumentNullException.ThrowIfNull(context);

        string html = context["contents"]?.ToString() ?? string.Empty;
        if (string.IsNullOrEmpty(html) || html.Contains(InjectionMarker, StringComparison.Ordinal))
        {
            return html;
        }

        int bodyClose = html.LastIndexOf("</body>", StringComparison.OrdinalIgnoreCase);
        if (bodyClose < 0)
        {
            return html;
        }

        string injection = $"<script type=\"text/javascript\" data-plugin=\"{InjectionMarker}\">{ScriptContent.Value}</script>\n";
        var result = new StringBuilder(html.Length + injection.Length);
        result.Append(html.AsSpan(0, bodyClose));
        result.Append(injection);
        result.Append(html.AsSpan(bodyClose));

        return result.ToString();
    }

    private static string LoadScript()
    {
        Assembly assembly = typeof(IndexHtmlTransform).Assembly;
        using Stream? stream = assembly.GetManifestResourceStream(ScriptResourceName);
        if (stream is null)
        {
            throw new InvalidOperationException($"Embedded resource not found: {ScriptResourceName}");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
