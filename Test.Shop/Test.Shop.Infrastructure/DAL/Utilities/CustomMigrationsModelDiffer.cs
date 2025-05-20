using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Pomelo.EntityFrameworkCore.MySql.Migrations.Internal;

namespace Test.Shop.Infrastructure.DAL.Utilities
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001: Internal EF Core API usage", Justification = "<Pending>")]
    public class CustomMigrationsModelDiffer : MySqlMigrationsModelDiffer
    {
        public CustomMigrationsModelDiffer(IRelationalTypeMappingSource typeMappingSource, IMigrationsAnnotationProvider migrationsAnnotationProvider, IRowIdentityMapFactory rowIdentityMapFactory, CommandBatchPreparerDependencies commandBatchPreparerDependencies) : base(typeMappingSource, migrationsAnnotationProvider, rowIdentityMapFactory, commandBatchPreparerDependencies)
        {

        }

        protected override IEnumerable<MigrationOperation> Add(IForeignKeyConstraint target, DiffContext diffContext)
        {
            if (target.Name.StartsWith(DbContextExtensions.IgnoreKey))
            {
                return [];
            }

            return base.Add(target, diffContext);
        }

        protected override IEnumerable<MigrationOperation> Remove(IForeignKeyConstraint source, DiffContext diffContext)
        {
            if (source.Name.StartsWith(DbContextExtensions.IgnoreKey))
            {
                return [];
            }

            return base.Remove(source, diffContext);
        }
    }
}
