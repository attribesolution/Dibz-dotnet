using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.DTO
{
    public class GameCatalogModel
    {
        public int Id { get; set; }
        public int FormatId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string UpdatedBy { get; set; }

        //-----------------------------------
        public Format Formats { get; set; }
        public string FormatName { get; set; }
        public string CategoryName { get; set; }
        public Category Categories { get; set; }

        public bool IsFeatured { get; set; }
        public int GameImageId { get; set; }

        public string FormatLongName { get; set; }
        
    }

    public class AdminGameCatalogModel
    {
        public string  Game { get; set; }
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public DateTime CreatedTime { get; set; }
        
        public string FormatName { get; set; }
        public string CategoryName { get; set; }
        
        public bool IsFeatured { get; set; }
        public int GameImageId { get; set; }

        public string FormatLongName { get; set; }
        public string Action { get; set; }
    }

    public class Format
    {
        public int FormatId { get; set; }

        public string Name { get; set; }

    }

    public class Category
    {
        public int CategoryId { get; set; }

        public string Name { get; set; }

    }
}
