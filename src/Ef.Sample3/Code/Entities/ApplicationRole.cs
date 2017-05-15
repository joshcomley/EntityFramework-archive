using System;

namespace WebApplication2.Entities
{
    public class ApplicationRole// : IdentityRole<string>
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public ApplicationRole()
        {

        }

        public ApplicationRole(string roleName)
        {
            Name = roleName;
            Id = Guid.NewGuid().ToString();
        }
    }
}