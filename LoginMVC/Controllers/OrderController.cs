using LoginMVC.Models;
using LoginMVC.Services;
using Microsoft.AspNetCore.Mvc;


namespace LoginMVC.Controllers
{
    public class OrdersController : Controller
    {

        private readonly IOrderService _svc; // IOrderService 인터페이스를 통해 비즈니스 로직(주문 CRUD)을 수행하는 서비스
        public OrdersController(IOrderService svc)// DI로 컨트롤러 생성 시 서비스가 주입됨
        {
            _svc = svc;
        }

        /// <summary>
        /// 주문 리스트 페이지
        /// 검색, 정렬, 페이징 기능을 서비스에 위임
        /// </summary>
        /// <param name="searchType">검색 대상 필드 ("ShipName" 또는 "ShipAddress")</param>
        /// <param name="searchText">검색어</param>
        /// <param name="sortColumn">정렬 기준 컬럼</param>
        /// <param name="sortOrder">정렬 방향 ("asc" 또는 "desc")</param>
        /// <param name="pageNum">현재 페이지 번호</param>
        /// <param name="pageSize">페이지당 아이템 수</param>
        public async Task<IActionResult> Index(string searchType = "ShipName", string searchText = "",
                                     string sortColumn = "OrderID", string sortOrder = "asc", int pageNum = 1, int pageSize = 10)
        {
            // 서비스 호출로 모든 로직을 수행하고, 결과 모델을 받아서 뷰에 전달
            var model = await _svc.GetPagedAsync(searchType, searchText, sortColumn, sortOrder,
                                                             pageNum, pageSize);
            return View(model);
        }

        /// <summary>
        /// 주문 생성 폼 보여주기 (GET)
        /// </summary>
        public IActionResult Create()
        {
            
            return View();
        }

        /// <summary>
        /// 주문 생성 처리 (POST)
        /// </summary>
        /// <param name="order">폼에서 바인딩된 Order 객체</param>
        [HttpPost]
        public async Task<IActionResult> Create(Order order)
        {
            if (!ModelState.IsValid) return View(order); // 모델 유효성 검사에 실패시, 다시 폼 뷰로 돌아감
            await _svc.CreateAsync(order); // 서비스에 생성 로직 위임
            return RedirectToAction(nameof(Index)); // 생성 성공 후 리스트 페이지로 리다이렉트
        }

        /// <summary>
        /// 주문 수정 폼 보여주기 (GET)
        /// </summary>
        /// <param name="id">수정할 주문의 ID</param>
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _svc.GetByIdAsync(id); 
            if (order == null) return NotFound();
            return View(order);
        }

        /// <summary>
        /// 주문 수정 처리 (POST)
        /// </summary>
        /// <param name="id">폼 히든 값으로 넘어오는 주문 ID</param>
        /// <param name="order">수정된 주문 데이터</param>
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (id != order.OrderID) return BadRequest();
            if (!ModelState.IsValid) return View(order);

            await _svc.UpdateAsync(order);  // 서비스에 업데이트 로직 위임
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// 주문 삭제 처리 (POST)
        /// </summary>
        /// <param name="id">삭제할 주문의ID</param>
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _svc.DeleteAsync(id); // 서비스에 삭제 로직 위임
            return RedirectToAction(nameof(Index));
        }
    }
}
