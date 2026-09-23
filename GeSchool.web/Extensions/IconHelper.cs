using Microsoft.AspNetCore.Html;

namespace GeSchool.web.Extensions;

public static class IconHelper
{
    private const string Prefix = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"1em\" height=\"1em\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\" aria-hidden=\"true\" focusable=\"false\">";
    private const string Suffix = "</svg>";

    private static readonly Dictionary<string, string> Paths = new()
    {
        ["dashboard"] = "<rect x='3' y='3' width='7' height='9'/><rect x='14' y='3' width='7' height='5'/><rect x='14' y='12' width='7' height='9'/><rect x='3' y='16' width='7' height='5'/>",
        ["building"] = "<rect x='4' y='2' width='16' height='20' rx='2'/><path d='M9 22v-4h6v4'/><path d='M8 6h.01'/><path d='M16 6h.01'/><path d='M8 10h.01'/><path d='M16 10h.01'/><path d='M8 14h.01'/><path d='M16 14h.01'/>",
        ["users"] = "<path d='M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2'/><circle cx='9' cy='7' r='4'/><path d='M22 21v-2a4 4 0 0 0-3-3.87'/><path d='M16 3.13a4 4 0 0 1 0 7.75'/>",
        ["graduation-cap"] = "<path d='M22 10 12 5 2 10l10 5 10-5Z'/><path d='M6 12v5c0 1.66 2.69 3 6 3s6-1.34 6-3v-5'/><path d='M22 10v6'/>",
        ["book-open"] = "<path d='M2 3h6a4 4 0 0 1 4 4v14a3 3 0 0 0-3-3H2Z'/><path d='M22 3h-6a4 4 0 0 0-4 4v14a3 3 0 0 1 3-3h7Z'/>",
        ["clipboard-list"] = "<rect x='8' y='2' width='8' height='4' rx='1'/><path d='M16 4h2a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2h2'/><path d='M9 12h6'/><path d='M9 16h6'/><path d='M9 8h1'/>",
        ["file-text"] = "<path d='M14.5 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V7.5Z'/><path d='M14 2v6h6'/><path d='M9 13h6'/><path d='M9 17h6'/>",
        ["award"] = "<circle cx='12' cy='8' r='6'/><path d='M15.477 12.89 17 22l-5-3-5 3 1.523-9.11'/>",
        ["menu"] = "<path d='M4 6h16'/><path d='M4 12h16'/><path d='M4 18h16'/>",
        ["search"] = "<circle cx='11' cy='11' r='8'/><path d='m21 21-4.3-4.3'/>",
        ["plus"] = "<path d='M12 5v14'/><path d='M5 12h14'/>",
        ["pencil"] = "<path d='M17 3a2.85 2.83 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5Z'/><path d='m15 5 4 4'/>",
        ["trash"] = "<path d='M3 6h18'/><path d='M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6'/><path d='M8 6V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2'/>",
        ["eye"] = "<path d='M2 12s3.5-7 10-7 10 7 10 7-3.5 7-10 7-10-7-10-7Z'/><circle cx='12' cy='12' r='3'/>",
        ["check-circle"] = "<circle cx='12' cy='12' r='10'/><path d='m9 12 2 2 4-4'/>",
        ["alert-triangle"] = "<path d='m21.73 18-8-14a2 2 0 0 0-3.48 0l-8 14A2 2 0 0 0 4 21h16a2 2 0 0 0 1.73-3Z'/><path d='M12 9v4'/><path d='M12 17h.01'/>",
        ["info"] = "<circle cx='12' cy='12' r='10'/><path d='M12 16v-4'/><path d='M12 8h.01'/>",
        ["x-circle"] = "<circle cx='12' cy='12' r='10'/><path d='m15 9-6 6'/><path d='m9 9 6 6'/>",
        ["sun"] = "<circle cx='12' cy='12' r='4'/><path d='M12 2v2'/><path d='M12 20v2'/><path d='m4.93 4.93 1.41 1.41'/><path d='m17.66 17.66 1.41 1.41'/><path d='M2 12h2'/><path d='M20 12h2'/><path d='m6.34 17.66-1.41 1.41'/><path d='m19.07 4.93-1.41 1.41'/>",
        ["moon"] = "<path d='M12 3a6 6 0 0 0 9 9 9 9 0 1 1-9-9Z'/>",
        ["monitor"] = "<rect x='2' y='3' width='20' height='14' rx='2'/><path d='M8 21h8'/><path d='M12 17v4'/>",
        ["chevron-down"] = "<path d='m6 9 6 6 6-6'/>",
        ["chevron-up"] = "<path d='m18 15-6-6-6 6'/>",
        ["chevron-left"] = "<path d='m15 18-6-6 6-6'/>",
        ["chevron-right"] = "<path d='m9 18 6-6-6-6'/>",
        ["log-out"] = "<path d='M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4'/><path d='M16 17l5-5-5-5'/><path d='M21 12H9'/>",
        ["bell"] = "<path d='M6 8a6 6 0 0 1 12 0c0 7 3 9 3 9H3s3-2 3-9'/><path d='M10.3 21a1.94 1.94 0 0 0 3.4 0'/>",
        ["calendar"] = "<rect x='3' y='4' width='18' height='18' rx='2'/><path d='M16 2v4'/><path d='M8 2v4'/><path d='M3 10h18'/>",
        ["x"] = "<path d='M18 6 6 18'/><path d='m6 6 12 12'/>",
        ["printer"] = "<polyline points='6 9 6 2 18 2 18 9'/><path d='M6 18H4a2 2 0 0 1-2-2v-5a2 2 0 0 1 2-2h16a2 2 0 0 1 2 2v5a2 2 0 0 1-2 2h-2'/><rect x='6' y='14' width='12' height='8'/>",
        ["rotate-ccw"] = "<path d='M3 12a9 9 0 1 0 9-9 9.75 9.75 0 0 0-6.74 2.74L3 8'/><path d='M3 3v5h5'/>",
        ["download"] = "<path d='M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4'/><polyline points='7 10 12 15 17 10'/><line x1='12' y1='15' x2='12' y2='3'/>",
    };

    public static IHtmlContent Render(string name, string? cssClass = null)
    {
        if (!Paths.TryGetValue(name, out var path))
        {
            return HtmlString.Empty;
        }

        var classAttr = string.IsNullOrEmpty(cssClass) ? string.Empty : $" class=\"{cssClass}\"";
        var svg = Prefix.Replace("<svg ", $"<svg{classAttr} ") + path + Suffix;
        return new HtmlString(svg);
    }
}
