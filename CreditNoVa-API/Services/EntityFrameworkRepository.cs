
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using System.Reflection;
using System.Threading.Tasks;
using EFund_API.Models.Interfaces;
using EFund_API.WebApp.Services.Interfaces;
using RestX.WebApp.Services;

namespace EFund_API.Services
{


    public class EntityFrameworkRepository<TContext> : EntityFrameworkReadOnlyRepository<TContext>, IRepository
    where TContext : DbContext
    {
        //private readonly ActiveTenant tenant;
        //private readonly IBackgroundJobClient jobClient;
        ////private readonly IRedisService cache;
        //private TContext RestXRestaurantManagementContext;
        protected readonly TContext context;
        //{
        //    get
        //    {
        //        return this.RestXRestaurantManagementContext ?? this.context as TContext;
        //    }
        //    set
        //    {
        //        this.RestXRestaurantManagementContext = value;
        //    }
        //}

        //public EntityFrameworkRepository(TContext context, IBackgroundJobClient jobClient, IRedisService cache, IEnumerable<ActiveTenant> tenant = null)
        //    : base(context)
        //{
        //    this.tenant = tenant?.FirstOrDefault();
        //    this.jobClient = jobClient;
        //    this.cache = cache;
        //}

        public EntityFrameworkRepository(TContext context) : base(context)
        {
            this.context = context;
        }

        public virtual object Create<TEntity>(TEntity entity, string createdBy = null)
            where TEntity : class, IEntity
        {
            entity.CreatedDate = DateTime.UtcNow;
            entity.CreatedBy = createdBy;
            //            entity.CustomProperties = this.ValidateCustomProperties(entity);
            var newEntity = this.context.Set<TEntity>().Add(entity);
            this.Save();
            //return newEntity.Entity.Id;
            return entity.Id;
        }

        public virtual async Task<object> CreateAsync<TEntity>(TEntity entity, string createdBy = null)
            where TEntity : class, IEntity
        {
            entity.CreatedDate = DateTime.UtcNow;
            entity.CreatedBy = createdBy ?? string.Empty;
            //            entity.CustomProperties = this.ValidateCustomProperties(entity);
            var newEntity = this.context.Set<TEntity>().Add(entity);
            await this.SaveAsync();
            //return newEntity.Entity.Id;
            return entity.Id;
        }

        public virtual void Update<TEntity>(TEntity entity, string modifiedBy = null)
            where TEntity : class, IEntity
        {
            entity.ModifiedDate = DateTime.UtcNow;
            entity.ModifiedBy = modifiedBy;
            //            entity.CustomProperties = this.ValidateCustomProperties(entity);
            this.context.Set<TEntity>().Attach(entity);
            this.context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete<TEntity>(object id)
            where TEntity : class, IEntity
        {
            TEntity entity = this.context.Set<TEntity>().Find(id);
            this.Delete(entity);
        }

        public virtual void Delete<TEntity>(TEntity entity)
            where TEntity : class, IEntity
        {
            var dbSet = this.context.Set<TEntity>();
            if (this.context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }

            dbSet.Remove(entity);
        }

        public virtual void Save()
        {
            var changes = this.context.ChangeTracker.Entries().Where(e => (e.State == EntityState.Modified || e.State == EntityState.Deleted) && MonitoredTriggerObjects.Contains(e.Entity.GetType().Name))
                .Select(c => new TriggerCheckData
                {
                    ObjectId = c.CurrentValues["Id"],
                    ObjectName = c.Entity.GetType().Name,
                }).ToList();

            // We have to get added items here so populate the Id's
            var newItems = this.context.ChangeTracker.Entries().Where(e => e.State == EntityState.Added && MonitoredTriggerObjects.Contains(e.Entity.GetType().Name)).ToList();

            this.context.SaveChanges();

            // Add newItems to changes as they will now have correct Id's
            changes.AddRange(newItems.Select(c => new TriggerCheckData
            {
                ObjectId = c.CurrentValues["Id"],
                ObjectName = c.Entity.GetType().Name,
            }));

            this.CheckForTriggers(changes);
        }

        public virtual async Task SaveAsync()
        {
            var changes = this.context.ChangeTracker.Entries().Where(e => (e.State == EntityState.Modified || e.State == EntityState.Deleted) && MonitoredTriggerObjects.Contains(e.Entity.GetType().Name))
                .Select(c => new TriggerCheckData
                {
                    ObjectId = c.CurrentValues["Id"],
                    ObjectName = c.Entity.GetType().Name,
                }).ToList();

            var added = this.context.ChangeTracker.Entries().Where(e => e.State == EntityState.Added).ToList();

            // We have to get added items here so populate the Id's
            var newItems = this.context.ChangeTracker.Entries().Where(e => e.State == EntityState.Added && MonitoredTriggerObjects.Contains(e.Entity.GetType().Name)).ToList();

            var result = await this.context.SaveChangesAsync();

            // Add newItems to changes as they will now have correct Id's
            changes.AddRange(newItems.Select(c => new TriggerCheckData
            {
                ObjectId = c.CurrentValues["Id"],
                ObjectName = c.Entity.GetType().Name,
            }));

            this.CheckForTriggers(changes);
        }

        public virtual async Task<T> ExecuteStoredProcedureAsync<T>(string storedProcedure, object[] parameters = null)
        {
            var conn = this.context.Database.GetDbConnection();
            using (var command = conn.CreateCommand())
            {
                command.CommandText = storedProcedure;
                command.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                if (conn.State == ConnectionState.Closed)
                {
                    await conn.OpenAsync();
                }

                var result = await command.ExecuteScalarAsync();
                if (result != null)
                {
                    return (T)result;
                }
            }
            return default(T);
        }

        public virtual async Task<T> ExecuteSqlCommandAsync<T>(string query, object[] parameters = null)
        {
            var conn = this.context.Database.GetDbConnection();
            using (var command = conn.CreateCommand())
            {
                command.CommandText = query;
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                if (conn.State == ConnectionState.Closed)
                {
                    await conn.OpenAsync();
                }

                var result = await command.ExecuteScalarAsync();
                if (result != null)
                {
                    return (T)result;
                }
            }
            return default(T);
        }

        public virtual async Task<List<T>> ExecuteSqlSelectAsync<T>(string query, object[] parameters = null, int commandTimeout = 600) where T : new()
        {
            var conn = this.context.Database.GetDbConnection();
            using (var command = conn.CreateCommand())
            {
                // Set the timeout to 6 minutes
                command.CommandTimeout = commandTimeout;

                if (query != null) command.CommandText = query;
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                if (conn.State == ConnectionState.Closed)
                {
                    await conn.OpenAsync();
                }

                using (var result = command.ExecuteReader())
                {
                    var results = new List<T>();
                    if (result != null)
                    {
                        var dataTable = result.GetSchemaTable();
                        while (result.Read())
                        {
                            var index = 0;
                            var t = new T();

                            if (dataTable != null)
                            {
                                foreach (DataRow row in dataTable.Rows)
                                {
                                    var value = result.GetValue(index);
                                    if (value == DBNull.Value)
                                    {
                                        t.GetType().GetProperty(row["BaseColumnName"].ToString())
                                            .SetValue(t, null, null);
                                    }
                                    else
                                    {
                                        t.GetType().GetProperty(row["BaseColumnName"].ToString())
                                            .SetValue(t, result.GetValue(index), null);
                                    }

                                    index++;
                                }
                            }

                            results.Add(t);
                        }

                        return results;
                    }
                }
            }

            return new List<T>();
        }

        public virtual async Task<int> ExecuteNonQueryAsync(string query, object[] parameters = null, int commandTimeout = 600)
        {
            var conn = this.context.Database.GetDbConnection();
            using (var command = conn.CreateCommand())
            {
                command.CommandTimeout = commandTimeout;
                command.CommandText = query;
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                if (conn.State == ConnectionState.Closed)
                {
                    await conn.OpenAsync();
                }

                return await command.ExecuteNonQueryAsync();

            }
        }

        /// <summary>
        /// This returns a list of objects that are monitored for triggers. Add the class name to trigger the monitor.
        /// You also need to add to the TriggerObjects table for the individual customer databases.
        /// </summary>
        private List<string> MonitoredTriggerObjects
        {
            get
            {
                if (this.context.GetType().Name != "AdminDbContext")
                {
                    return new List<string>()
                    {
                        "Enquiry",
                        "Quote",
                        "EnquiryHistory",
                        "EnquiryHistoryUnassigned",
                        "EnquiryTask",
                        "BookingPassenger",
                        "QuoteFeedback",
                        "Booking",
                        "BookingDocuments",
                        "PaymentTransaction",
                        "Notification"
                    };
                }

                return new List<string>();
            }

        }
        private void CheckForTriggers(List<TriggerCheckData> changes)
        {
            // Check if any objects changed are being monitored for triggers
            if (changes.Count == 0)
            {
                return;
            }

            if (this.context.GetType().Name != "AdminDbContext")
            {
                //jobClient.Enqueue<ITriggerService>(x => x.CheckForTriggers(this.tenant.Id, this.tenant.ActiveBrand.Id, changes));
            }
        }

        private class TriggerCheckData
        {
            public object ObjectId { get; set; }
            public string ObjectName { get; set; }
            public object Type { get; set; }
            public Dictionary<string, string> OriginalValues { get; set; }
            public Dictionary<string, string> CurrentValues { get; set; }
        }
    }
}

