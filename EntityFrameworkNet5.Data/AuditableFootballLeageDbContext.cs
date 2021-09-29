using EntityFrameworkNet5.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkNet5.Data
{
    public abstract class AuditableFootballLeageDbContext : DbContext
    {
        public async Task<int> SaveChangesAsync(string userName)
        {
            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            // NEW ENTITIES TO BE ADDED
            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            IEnumerable<EntityEntry> addEntries = ChangeTracker.Entries().Where(y => y.State == EntityState.Added);
            foreach (EntityEntry addEntry in addEntries)
            {
                BaseDomain baseDomainObj = (BaseDomain)addEntry.Entity;
                baseDomainObj.CreatedDate = DateTime.Now;
                baseDomainObj.CreatedBy = userName;
            }

            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            // EXISTING ENTITIES TO BE MODIFIED
            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            IEnumerable<EntityEntry> modifiedEntries = ChangeTracker.Entries().Where(y => y.State == EntityState.Modified);
            foreach (EntityEntry modifiedEntry in modifiedEntries)
            {
                BaseDomain baseDomainObj = (BaseDomain)modifiedEntry.Entity;
                baseDomainObj.ModifiedDate = DateTime.Now;
                baseDomainObj.ModifiedBy = userName;
            }

            return await base.SaveChangesAsync();
        }
    }
}
