using Microsoft.OData.Client;
using System.Reflection;

namespace Test.Shop.Infrastructure.Shared.OData
{
    public static class Extensions
    {
        public static DataServiceQuery<T> WithSelect<T>(this DataServiceQuery<T> query)
        {
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(x => x.Name);

            var selectClause = string.Join(",", props);
            return query.AddQueryOption("$select", selectClause);
        }
    }
}
