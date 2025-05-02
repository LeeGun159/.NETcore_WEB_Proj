using LoginMVC.Data;
using LoginMVC.Models;
using Microsoft.EntityFrameworkCore;

using Neoplus.NetCore.WorkLib.Models;

namespace LoginMVC.Services
{
    /// <summary>
    /// 주문 관련 비즈니스 로직을 처리하는 서비스
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _db;
        public OrderService(AppDbContext db) => _db = db; // DI로 주입된 DbContext를 사용

        public async Task<ResultData<Order>> GetPagedAsync(  // 페이징, 검색, 정렬이 적용된 주문 목록을 가져옴
            string searchType, string searchText,        
            string sortColumn, string sortOrder,
            int pageNum, int pageSize)
        {
            var q = _db.Orders.AsQueryable();   //  기본 쿼리 생성

            // 검색 조건 적용
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                q = searchType == "ShipAddress"
                    ? q.Where(o => o.ShipAddress.Contains(searchText))
                    : q.Where(o => o.ShipName.Contains(searchText));
            }

            // 정렬
            q = sortOrder == "desc"
                ? q.OrderByDescending(e => EF.Property<object>(e, sortColumn))
                : q.OrderBy(e => EF.Property<object>(e, sortColumn));

            //페이징 처리
            var items = await q
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var total = await q.CountAsync();  // 전체 아이템 수 계산 (페이징 전)


            return new ResultData<Order>    // ResultData 객체에 패키징하여 반환
            {
                Result = items,
                ItemCount = total,
                SortColumn = sortColumn,
                SortType = sortOrder == "asc"
                                ? SortType.Asc
                                : SortType.Desc,
                PageNum = pageNum,
                PageSize = pageSize,
                SearchType = searchType,
                SearchText = searchText
            };
        }


        public async Task<Order?> GetByIdAsync(int id) => // 단일 주문을 ID로 조회
            await _db.Orders.FindAsync(id);

        public async Task CreateAsync(Order order) // 새로운 주문을 생성하고 저장
        {
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order order) // 기존 주문을 수정하고 저장
        {
            _db.Orders.Update(order);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id) // ID 기반으로 주문을 삭제하고 저장
        {
            var o = await _db.Orders.FindAsync(id);
            if (o != null)
            {
                _db.Orders.Remove(o);
                await _db.SaveChangesAsync();
            }
        }
    }
}
