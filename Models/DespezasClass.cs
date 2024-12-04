using Gest.Emus;

namespace Gest.Models
{
    public class DespezasClass
    {
        public int Id { get; set; }
        public string? Desc { get; set; }
        public decimal? Price { get; set; }
        public int? Finalidade { get; set; }
        public DateTime? Data { get; set; }
    }
}
