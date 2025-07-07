namespace portal_agile.Dtos.MenuItem
{
    public class MenuItemDto
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public required int MenuLevel { get; set; }
        public string? RequiredPermission { get; set; }
        public string Tooltip { get; set; } = string.Empty;
        public string ModuleName { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 0;
        public List<MenuItemDto> Children { get; set; } = new List<MenuItemDto>();
    }
}
