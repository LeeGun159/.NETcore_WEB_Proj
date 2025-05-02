using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neoplus.NetCore.WorkLib.Models
{
    public enum SortType
    {
        Asc,
        Desc
    }
    public class PageInfo
    {
        public PageInfo()
        {
            this.PageNum = 1;
            this.PageSize = 10;
            this.PageListNum = 10;
            Params = new Dictionary<string, string>();
        }

        /// <summary>
        /// 총 갯수
        /// </summary>
        public int ItemCount { get; set; }

        /// <summary>
        /// 페이지 번호
        /// </summary>
        public int PageNum { get; set; }

        /// <summary>
        /// 페이지 사이즈
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 페이징 칼럼
        /// </summary>
        public int PageListNum { get; set; }

        public string? SearchType { get; set; }
        public string? SearchText { get; set; }
        public string? SortColumn { get; set; }
        public SortType? SortType { get; set; }

        public Dictionary<string, string> Params { get; set; }
    }
}
