using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Лепестки_ветра.Models
{
    [Table("product")]
    public class Product : BaseModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string imageurl { get; set; }
        public decimal price { get; set; }
        public string delivery { get; set; }
        public bool is_hit { get; set; }
        public string description { get; set; }
        public string type { get; set; }
    }

}
