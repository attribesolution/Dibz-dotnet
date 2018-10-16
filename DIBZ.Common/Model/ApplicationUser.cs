using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.Model
{
    public class ApplicationUser:User
    {
        public string AboutMe { get; set; }
        public string NickName { get; set; }
        public string CellNo { get; set; }
        public string YearOfBirth { get; set; }
        public int ProfileViewedCounter { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public virtual UploadedFile ProfileImage { get; set; }
        [ForeignKey("ProfileImage")]
        public int? ProfileImageId { get; set; }
        public virtual Scorecard Scorecard { get; set; }
        [ForeignKey("Scorecard")]
        public int? ScorecardId { get; set; }
        public virtual ICollection<Offer> Offers { get; set; }
        public virtual ICollection<GameCatalog> GameCatalogs { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public ApplicationUser()
        {
            
            Offers = new Collection<Offer>();
            Transactions = new Collection<Transaction>();
            
        }
    }
}
