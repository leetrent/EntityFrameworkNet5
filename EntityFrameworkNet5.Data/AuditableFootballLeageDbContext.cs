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
        public DbSet<Audit> Audits { get; set; }

        public async Task<int> SaveChangesAsync(string username)
        {
            var auditEntries = OnBeforeSaveChanges(username);
            var saveResult = await base.SaveChangesAsync();
            if (auditEntries != null || auditEntries.Count > 0)
            {
                await OnAfterSaveChanges(auditEntries);
            }

            return saveResult;
        }

        private Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            foreach (var auditEntry in auditEntries)
            {
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }
                Audits.Add(auditEntry.ToAudit());
            }

            return SaveChangesAsync();
        }

        private List<AuditEntry> OnBeforeSaveChanges(string username)
        {
            IEnumerable<EntityEntry> entries = ChangeTracker.Entries().Where(q => q.State == EntityState.Added || q.State == EntityState.Modified || q.State == EntityState.Deleted);
            List<AuditEntry> auditEntries = new List<AuditEntry>();

            foreach (EntityEntry entry in entries)
            {
                BaseDomain auditableObject = (BaseDomain)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    auditableObject.CreatedDate = DateTime.Now;
                    auditableObject.CreatedBy = username;
                }

                if (entry.State == EntityState.Modified
                        || entry.State == EntityState.Deleted)
                {
                    auditableObject.ModifiedDate = DateTime.Now;
                    auditableObject.ModifiedBy = username;
                }

                AuditEntry auditEntry = new AuditEntry(entry);
                auditEntry.TableName = entry.Metadata.GetTableName();
                auditEntry.Action = entry.State.ToString();
                auditEntries.Add(auditEntry);

                foreach (PropertyEntry property in entry.Properties)
                {
                    if (property.IsTemporary)
                    {
                        auditEntry.TemporaryProperties.Add(property);
                        continue;
                    }

                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }

            foreach (var pendingAuditEntry in auditEntries.Where(q => q.HasTemporaryProperties == false))
            {
                Audits.Add(pendingAuditEntry.ToAudit());
            }

            return auditEntries.Where(q => q.HasTemporaryProperties).ToList();
        }
    }
}
