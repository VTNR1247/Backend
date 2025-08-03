using Backend.Models.Domains;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class AuthDBContext : DbContext
    {
        public AuthDBContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<AuthenticationClass> Users {  get; set; }
    }
}
