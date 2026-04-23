namespace TubaToolbox
{
    public record ToolItem(string Name, string? RelativePath = null, bool IsImage = false, bool IsInfoOnly = false);
}
