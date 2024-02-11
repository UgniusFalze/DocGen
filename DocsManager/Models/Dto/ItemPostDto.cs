namespace DocsManager.Models.Dto;

public class ItemPostDto
{
    
    public string Name { get; set; }
    public string UnitOfMeasurement { get; set; }
    public int Units { get; set; }
    public decimal PriceOfUnit { get; set; }
}