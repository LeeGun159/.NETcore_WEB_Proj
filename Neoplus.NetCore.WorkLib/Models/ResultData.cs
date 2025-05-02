using System;
using System.Collections;
using System.Collections.Generic;

namespace Neoplus.NetCore.WorkLib.Models
{
    public class ResultData<T> : IEnumerable<T> // IEnumerable<T> 구현     (컬렉션을 반복하는 기능을 제공)
    {
        public ResultData()
        {

        }

        public int ItemCount { get; set; }

        public string? SearchType { get; set; }
        public string? SearchText { get; set; }
        public string? SortColumn { get; set; }
        public SortType? SortType { get; set; }
        public int PageNum { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public List<T>? Result { get; set; }

        // IEnumerable<T> 구현
        public IEnumerator<T> GetEnumerator()
        {
            return Result?.GetEnumerator() ?? Enumerable.Empty<T>().GetEnumerator(); // Result가 null일 때는 빈 컬렉션을 반환
        }
        
        // IEnumerable 구현
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}