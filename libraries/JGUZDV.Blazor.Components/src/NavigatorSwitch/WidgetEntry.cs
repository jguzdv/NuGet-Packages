namespace JGUZDV.Blazor.Components.NavigatorSwitch
{
    public class WidgetEntry
    {
        public WidgetEntry(Guid id,
            string? logoAsDataString,
            string shortName,
            WidgetLink serviceLink)
        {
            Id = id;
            LogoAsDataString = logoAsDataString;
            ShortName = shortName;
            ServiceLink = serviceLink;
        }

        public Guid Id { get; }

        public string? LogoAsDataString { get; }

        public string ShortName { get; }
        public WidgetLink ServiceLink { get; }
    }

    public enum WidgetLinkType
    {
        Main,
        Help,
        Documenation,
        Additional
    }

    public class WidgetLink
    {
        public WidgetLink(Guid id, WidgetLinkType type, string anchorText_de, string anchorText_en, string url)
        {
            Id = id;
            Type = type;
            AnchorText_de = anchorText_de;
            AnchorText_en = anchorText_en;
            Url = url;
        }

        public Guid Id { get; set; }
        public WidgetLinkType Type { get; set; }

        public string AnchorText_de { get; set; }
        public string AnchorText_en { get; set; }

        public string Url { get; set; }
    }
}
