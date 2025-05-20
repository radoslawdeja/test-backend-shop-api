using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Test.Shop.Infrastructure.DAL.Utilities
{
    public static class DbContextExtensions
    {
        public const string IgnoreKey = "IGNORE";

        public static ReferenceReferenceBuilder IgnoreConstraint(this ReferenceReferenceBuilder builder)
        {
            builder.HasConstraintName($"{IgnoreKey}{Guid.NewGuid()}");
            return builder;
        }
    }
}
