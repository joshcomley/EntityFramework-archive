using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Entities
{
    public class ClientType : IDbObject<int>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}