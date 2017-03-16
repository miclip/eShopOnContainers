namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Queries
{
    using Dapper;
    using Microsoft.Extensions.Configuration;
    using System.Data.SqlClient;
    using Npgsql;
    using System.Threading.Tasks;
    using System;
    using System.Dynamic;
    using System.Collections.Generic;

    public class OrderQueries
        :IOrderQueries
    {
        private NpgsqlConnection _connection = null;

        public OrderQueries(NpgsqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }


        public async Task<dynamic> GetOrder(int id)
        {
            using (_connection)
            {
                _connection.Open();

                var result = await _connection.QueryAsync<dynamic>(
                   @"select o.""Id"" as ordernumber, o.""OrderDate"" as date, os.""Name"" as status, 
                        oi.""ProductName"" as productname, oi.""Units"" as units, oi.""UnitPrice"" as unitprice, oi.""PictureUrl"" as pictureurl,
                        a.""Street"" as street, a.""City"" as city, a.""Country"" as country, a.""State"" as state, a.""ZipCode"" as zipcode
                        FROM ""ordering"".""orders"" o
                        INNER JOIN ""ordering"".""address"" a ON a.""Id"" = o.""AddressId""
                        LEFT JOIN ""ordering"".""orderItems"" oi ON o.""Id"" = oi.""OrderId""
                        LEFT JOIN ""ordering"".""orderstatus"" os on o.""OrderStatusId"" = os.""Id""
                        WHERE o.""Id""=@id"
                        , new { id }
                    );

                if (result.AsList().Count == 0)
                    throw new KeyNotFoundException();

                return MapOrderItems(result);
            }
        }

        public async Task<dynamic> GetOrders()
        {
            using (_connection)
            {
                _connection.Open();

                return await _connection.QueryAsync<dynamic>(@"SELECT o.""Id"" as ordernumber,o.""OrderDate"" as date, os.""Name"" as status, SUM(oi.""Units"" * oi.""UnitPrice"") as total
                     FROM ""ordering"".""orders"" o
                     LEFT JOIN ""ordering"".""orderItems"" oi ON oi.""OrderId"" = o.""Id""
                     LEFT JOIN ""ordering"".""orderstatus"" os on o.""OrderStatusId"" = os.""Id""
                     GROUP BY o.""Id"", o.""OrderDate"", os.""Name""");
            }
        }

        public async Task<dynamic> GetCardTypes()
        {
            using (_connection)
            {
                _connection.Open();

                return await _connection.QueryAsync<dynamic>(@"SELECT * FROM ""ordering"".""cardtypes""");
            }
        }

        private dynamic MapOrderItems(dynamic result)
        {
            dynamic order = new ExpandoObject();

            order.ordernumber = result[0].ordernumber;
            order.date = result[0].date;
            order.status = result[0].status;
            order.street = result[0].street;
            order.city = result[0].city;
            order.zipcode = result[0].zipcode;
            order.country = result[0].country;

            order.orderitems = new List<dynamic>();
            order.total = 0;

            foreach (dynamic item in result)
            {
                dynamic orderitem = new ExpandoObject();
                orderitem.productname = item.productname;
                orderitem.units = item.units;
                orderitem.unitprice = item.unitprice;
                orderitem.pictureurl = item.pictureurl;

                order.total += item.units * item.unitprice;
                order.orderitems.Add(orderitem);
            }

            return order;
        }
    }
}
