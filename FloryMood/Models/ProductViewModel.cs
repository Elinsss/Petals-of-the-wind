using System.ComponentModel.DataAnnotations.Schema;
using Supabase.Postgrest.Models;

[Table("product")]
public class ProductViewModel : BaseModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
    public bool IsHit { get; set; }
    public string DeliveryInfo { get; set; }
}
