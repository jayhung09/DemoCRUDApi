using System.ComponentModel.DataAnnotations.Schema;

namespace DemoCRUDApi.Model
{
    public class CrudDemo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string? DemoName { get; set; }
    }
}
