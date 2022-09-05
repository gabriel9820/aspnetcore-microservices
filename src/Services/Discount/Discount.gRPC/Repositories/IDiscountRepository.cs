using Discount.gRPC.Entities;

namespace Discount.gRPC.Repositories
{
    public interface IDiscountRepository
    {
        Task<Coupon> GetDiscount(string productName);
        Task<bool> CreateDiscount(Coupon coupon);
        Task<bool> UpdateDiscount(int id, Coupon coupon);
        Task<bool> DeleteDiscount(int id);
    }
}
