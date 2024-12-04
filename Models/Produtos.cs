using Gest.Emus;

namespace Gest.Models
{
    public class Produtos
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public Finalidade Mine { get; set; }
        public Finalidade? MineDes { get; set; }
        public string ProductLink { get; set; }
        public string? Image { get; set; }

        //public int? State { get; set; }
        public decimal? BuingPrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public int? Quantity { get; set; }

    }
}
