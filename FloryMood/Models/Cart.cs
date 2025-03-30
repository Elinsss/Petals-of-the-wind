using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Лепестки_ветра.Models;

[Table("cart")]
public class cart : BaseModel
{
    internal Product? Product;

    [PrimaryKey]
    public int id { get; set; }
    public int? user_id { get; set; }
    public int product_id { get; set; }
    public int quantity { get; set; }
    public int quantity_flowers { get; set; }

}
