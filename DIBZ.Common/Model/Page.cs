using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.Model
{
    public class Page
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }

        public string Text { get; set; }
        public Page(int currentPage, int pageSize, int totalRecords, string text)
        {
            this.CurrentPage = currentPage;
            this.PageSize = pageSize;
            this.TotalRecords = totalRecords;
            this.Text = text;
        }
    }
}
