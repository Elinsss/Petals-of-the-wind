using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Лепестки_ветра.Models;

[Table("wishlist")]
public class Wishlist : BaseModel
{
    internal Product? Product;

    [PrimaryKey]
    public int id { get; set; }
    public int id_product { get; set; }
    public int? id_buyer { get; set; }

}
