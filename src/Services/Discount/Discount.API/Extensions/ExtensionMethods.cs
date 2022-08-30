using Npgsql;

namespace Discount.API.Extensions
{
    public static class DatabaseExtensionMethods
    {
        public static IServiceProvider MigrateDatabase<TContext>(this IServiceProvider services, int? retry = 0)
        {
            int retryForAvailability = retry.Value;

            var configuration = services.GetRequiredService<IConfiguration>();
            var logger = services.GetRequiredService<ILogger<TContext>>();

            try
            {
                logger.LogInformation("Migrating Database");

                using (var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand { Connection = connection })
                    {
                        command.CommandText = "DROP TABLE IF EXISTS Coupon";
                        command.ExecuteNonQuery();

                        command.CommandText = @"CREATE TABLE Coupon (
                                                    Id SERIAL PRIMARY KEY, 
                                                    ProductName VARCHAR(24) NOT NULL,
                                                    Description TEXT,
                                                    Amount INT
                                                    )";
                        command.ExecuteNonQuery();

                        command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount) VALUES ('IPhone X', 'IPhone Discount', 150);";
                        command.ExecuteNonQuery();

                        command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount) VALUES ('Samsung 10', 'Samsung Discount', 100);";
                        command.ExecuteNonQuery();
                    }
                }

                logger.LogInformation("Migrated Database");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error ocurred while migrating Database");

                if (retryForAvailability < 10)
                {
                    retryForAvailability++;
                    Thread.Sleep(2000);
                    MigrateDatabase<TContext>(services, retryForAvailability);
                }
            }

            return services;
        }
    }
}
