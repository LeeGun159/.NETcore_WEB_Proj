using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Neoplus.NetCore.WorkLib.Models;

namespace Neoplus.NetCore.WorkLib.Controls
{
    public class Paging
    {
        public Paging() { }
        public static IHtmlContent RenderHtml(PageInfo pageInfo)
        {
            var firstNum = 1;

            // 2.이전 페이지
            var prevNum = Math.Max((((pageInfo.PageNum - 1) / pageInfo.PageListNum) * pageInfo.PageListNum) + 1 - pageInfo.PageListNum, 1);

            // 3. 페이지 Num
            var firstPageNum = ((pageInfo.PageNum - 1) / pageInfo.PageListNum) * pageInfo.PageListNum + 1;

            // 5.마지막 페이지
            var lastNum = (pageInfo.ItemCount / pageInfo.PageSize) + ((pageInfo.ItemCount % pageInfo.PageSize) > 0 ? 1 : 0);

            // 4.다음 페이지
            var nextNum = Math.Min((((pageInfo.PageNum - 1) / pageInfo.PageListNum) * pageInfo.PageListNum) + 1 + pageInfo.PageListNum
                          , lastNum);

            bool isPrev = pageInfo.PageNum > (firstNum * pageInfo.PageListNum);
            bool isNext = pageInfo.PageNum < (lastNum - (lastNum % pageInfo.PageListNum)) + 1;

            StringBuilder sb = new StringBuilder();
            sb.Append("<script>");
            sb.Append("function fn_PagingUrl(params) {");
            sb.Append("var url = location.origin + location.pathname + params;");
            sb.Append("location.href = url;");
            sb.Append("}");
            sb.Append("</script>");
            sb.Append("<div style='text-align:center;'>");
            sb.Append("<ul style='display:flex;justify-content:center;' class=\"pagination\">");

            if (isPrev)
            {
                sb.Append(RenderPagingItem(pageInfo, firstNum, "처음"));
                sb.Append(RenderPagingItem(pageInfo, prevNum, "이전"));
            }
            for (int n = firstPageNum; n < firstPageNum + pageInfo.PageListNum; n++)
            {
                if (lastNum < n) break;
                sb.Append(RenderPagingItem(pageInfo, n, n.ToString(), n == pageInfo.PageNum));
            }
            if (isNext)
            {
                sb.Append(RenderPagingItem(pageInfo, nextNum, "다음"));
                sb.Append(RenderPagingItem(pageInfo, lastNum, "마지막"));
            }

            sb.Append("</ul>");
            sb.Append("</div>");

            return new HtmlString(sb.ToString());
        }

        private static string RenderPagingItem(PageInfo pageInfo, int pageNum, string text, bool isCurrent = false)
        {
            string param = $"?pageNum={pageNum}&pageSize={pageInfo.PageSize}";

            if (!string.IsNullOrWhiteSpace(pageInfo.SearchType))
            {
                param += $"&searchType={pageInfo.SearchType}";
            }
            if (!string.IsNullOrWhiteSpace(pageInfo.SearchText))
            {
                param += $"&searchText={pageInfo.SearchText}";
            }
            if (!string.IsNullOrWhiteSpace(pageInfo.SortColumn))
            {
                param += $"&sortColumn={pageInfo.SortColumn}";
            }
            param += $"&sortType={pageInfo.SortType}";

            foreach (var item in pageInfo.Params)
            {
                param += $"&{item.Key}={item.Value}";
            }

            string pagingItem = string.Empty;

            if (isCurrent)
            {
                pagingItem = $"<li class=\"page-item\"><a class=\"page-link\"><b>{text}</b></a></li>";
            }
            else
            {
                pagingItem = $"<li class=\"page-item\"><a class=\"page-link\" href=\"javascript:fn_PagingUrl('{param}');\">{text}</a></li>";
            }

            return pagingItem;
        }
    }
}
