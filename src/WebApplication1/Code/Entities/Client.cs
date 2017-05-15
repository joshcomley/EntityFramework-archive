using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Entities
{
    public class Client : DbObject
    {
        [Required(ErrorMessage = "Please enter a name")]
        public string Name { get; set; }
        public List<Video> Videos { get; set; }
        public string Description { get; set; }
        public List<ApplicationUser> Users { get; set; }
        [Required(ErrorMessage = "Please select a management type")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select a management type")]
        public int TypeId { get; set; }
        public ClientType Type { get; set; }
    }
}