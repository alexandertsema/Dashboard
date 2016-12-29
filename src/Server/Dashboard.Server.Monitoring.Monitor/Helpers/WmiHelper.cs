using System;
using System.Management;
using System.Text;
using Microsoft.Management.Infrastructure;

namespace Dashboard.Server.Monitoring.Monitor.Helpers
{
    public class WmiHelper
    {
        public static string GetServerName() => $@"\\{Environment.MachineName}\root\CIMV2";

        public static SelectQuery BuildQuery<T>(T model) //todo: cover with test
        {
            var query = new SelectQuery();

            var queryString = new StringBuilder().Append("Select ");

            var properties = model.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (!property.PropertyType.IsEnum)
                {
                    queryString.Append($"{property.Name},");
                }
                else
                {
                    queryString.Remove(queryString.Length - 1, 1);
                    queryString.Append($" from {property.GetValue(model)}");
                }
            }

            query.QueryString = queryString.ToString();

            return query;
        }
    }
}