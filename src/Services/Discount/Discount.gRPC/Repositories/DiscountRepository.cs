using Dapper;
using Discount.gRPC.Entities;
using Npgsql;

namespace Discount.gRPC.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;

        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            using (var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
            {
                var coupon = await connection
                    .QueryFirstOrDefaultAsync<Coupon>("SELECT * FROM Coupon WHERE ProductName = @ProductName", new { ProductName = productName });

                if (coupon == null)
                {
                    return new Coupon { ProductName = "No Discount", Description = "No Description", Amount = 0 };
                }

                return coupon;
            }
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using (var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
            {
                var affected = await connection
                    .ExecuteAsync("INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
                        new
                        {
                            ProductName = coupon.ProductName,
                            Description = coupon.Description,
                            Amount = coupon.Amount
                        });

                return affected > 0;
            }
        }

        public async Task<bool> UpdateDiscount(int id, Coupon coupon)
        {
            using (var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
            {
                var affected = await connection
                    .ExecuteAsync("UPDATE Coupon SET ProductName = @ProductName, Description = @Description, Amount = @Amount WHERE Id = @Id",
                        new
                        {
                            ProductName = coupon.ProductName,
                            Description = coupon.Description,
                            Amount = coupon.Amount,
                            Id = id,
                        });

                return affected > 0;
            }
        }

        public async Task<bool> DeleteDiscount(int id)
        {
            using (var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
            {
                var affected = await connection
                    .ExecuteAsync("DELETE FROM Coupon WHERE Id = @Id", new { Id = id });

                return affected > 0;
            }
        }
    }
}
