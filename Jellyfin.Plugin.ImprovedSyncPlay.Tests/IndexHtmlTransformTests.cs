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

        var output = IndexHtmlTransform.Transform(input);

        var html = output["contents"]?.ToString();
        Assert.NotNull(html);
        Assert.Contains("improved-syncplay-share", html, StringComparison.Ordinal);
        Assert.Contains("<script type=\"text/javascript\">", html, StringComparison.Ordinal);
        Assert.EndsWith("</body></html>", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Transform_IsIdempotentWhenMarkerPresent()
    {
        var input = new JObject
        {
            ["contents"] = "<html><body>improved-syncplay-share</body></html>",
        };

        var output = IndexHtmlTransform.Transform(input);

        Assert.Same(input, output);
    }
}
