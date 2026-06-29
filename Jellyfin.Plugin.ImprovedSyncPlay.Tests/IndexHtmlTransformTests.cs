using Jellyfin.Plugin.ImprovedSyncPlay.Web;
using Newtonsoft.Json.Linq;

namespace Jellyfin.Plugin.ImprovedSyncPlay.Tests;

public class IndexHtmlTransformTests
{
    [Fact]
    public void Transform_InjectsScriptBeforeClosingBody()
    {
        var input = new JObject
        {
            ["contents"] = "<html><body><div>app</div></body></html>",
        };

        var html = IndexHtmlTransform.Transform(input);

        Assert.NotNull(html);
        Assert.Contains("improved-syncplay-share-script", html, StringComparison.Ordinal);
        Assert.Contains("data-plugin=\"improved-syncplay-share-script\"", html, StringComparison.Ordinal);
        Assert.Contains("<script type=\"text/javascript\"", html, StringComparison.Ordinal);
        Assert.EndsWith("</body></html>", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Transform_IsIdempotentWhenMarkerPresent()
    {
        const string html = "<html><body><script data-plugin=\"improved-syncplay-share-script\"></script></body></html>";
        var input = new JObject
        {
            ["contents"] = html,
        };

        var output = IndexHtmlTransform.Transform(input);

        Assert.Equal(html, output);
    }
}
