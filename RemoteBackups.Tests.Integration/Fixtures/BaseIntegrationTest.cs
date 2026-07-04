using Microsoft.EntityFrameworkCore;
using RemoteBackups.Api.Persistance;
using Respawn;

namespace RemoteBackups.Tests.Integration.Fixtures
{
    public abstract class BaseIntegrationTest : IClassFixture<TestDatabaseContainer>, IAsyncLifetime
    {
        protected readonly ApplicationDbContext DbContext;
        private Respawner _respawner = default!;

        protected BaseIntegrationTest(TestDatabaseContainer container)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(container.ConnectionString)
                .Options;

            DbContext = new ApplicationDbContext(options);
        }

        public async Task InitializeAsync()
        {
            await DbContext.Database.EnsureCreatedAsync();

            var connection = DbContext.Database.GetDbConnection();
            await connection.OpenAsync();

            _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
            {
                TablesToIgnore = new Respawn.Graph.Table[]
                {
                    new Respawn.Graph.Table("__EFMigrationsHistory")
                }
            });
        }

        public async Task DisposeAsync()
        {
            await _respawner.ResetAsync(DbContext.Database.GetDbConnection());
        }
    }
}
