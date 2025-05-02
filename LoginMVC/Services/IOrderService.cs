using LoginMVC.Models;
using Neoplus.NetCore.WorkLib.Models;

namespace LoginMVC.Services
{
    public interface IOrderService
    {
        Task<ResultData<Order>> GetPagedAsync(
            string searchType, string searchText,
            string sortColumn, string sortOrder,
            int pageNum, int pageSize);

        Task<Order?> GetByIdAsync(int id);
        Task CreateAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(int id);
    }
}
