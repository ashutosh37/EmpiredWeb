using Empired.Data.Configurations;
using Empired.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empired.Data
{
    public class RWRSPContext : DbContext
    {
        public RWRSPContext()
            : base("RWRSP")
        {
            Database.SetInitializer<RWRSPContext>(null);
        }

        #region Entity Sets
        public IDbSet<User> UserSet { get; set; }
        public IDbSet<Role> RoleSet { get; set; }
        public IDbSet<UserRole> UserRoleSet { get; set; }
        public IDbSet<Patient> PatientSet { get; set; }
        //public IDbSet<Movie> MovieSet { get; set; }
        //public IDbSet<Genre> GenreSet { get; set; }
        //public IDbSet<Stock> StockSet { get; set; }
        //public IDbSet<Rental> RentalSet { get; set; }
        public IDbSet<Error> ErrorSet { get; set; }
        #endregion

        public virtual void Commit()
        {
            base.SaveChanges();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

           modelBuilder.Configurations.Add(new UserConfiguration());
           modelBuilder.Configurations.Add(new UserRoleConfiguration());
            modelBuilder.Configurations.Add(new RoleConfiguration());
            modelBuilder.Configurations.Add(new PatientConfiguration());

        }
    }
}
