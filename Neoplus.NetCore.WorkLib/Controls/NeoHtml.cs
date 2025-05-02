using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Neoplus.NetCore.WorkLib.Models;

namespace Neoplus.NetCore.WorkLib.Controls
{
    public static class NeoHtml
    {
        public static IHtmlContent Pager(PageInfo pageInfo)
        {
            return Paging.RenderHtml(pageInfo);
        }
    }
}
