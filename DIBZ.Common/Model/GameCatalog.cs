using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.Model
{
    public class GameCatalog :BaseModelObject
    {
        public virtual Format Format { get; set; }
        public int FormatId { get; set; }

        public virtual Category Category { get; set; }
        public int CategoryId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string url { get; set; }
        public string imgpath { get; set; }
        public decimal sellprice { get; set; }
        public decimal cashprice { get; set; }
        public decimal voucherprice { get; set; }
        public int? CreatedBy { get; set; }
        public virtual UploadedFile GameImage { get; set; }
        public int GameImageId { get; set; }
        public bool IsFeatured { get; set; }
        public virtual ICollection<Offer> Offers { get; set; }

        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
        public GameCatalog()
        {

            Offers = new Collection<Offer>();
            ApplicationUsers = new Collection<ApplicationUser>();
        }
    }
}
