using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace DIBZ.Common.Model
{
    public class Scorecard: BaseModelObject
    {
       
        public int? ApplicationUserId { get; set; }

        /// <summary>
        /// Proposals : Number of times a user has agreed a swap 
        /// </summary>
        public int Proposals { get; set; }

        /// <summary>
        /// No shows : Number of times user has not sent in games within 5 working days
        /// </summary>
        public int NoShows { get; set; }
        /// <summary>
        /// Games sent : Number of games sent in 
        /// </summary>
        public int GamesSent { get; set; }
        /// <summary>
        /// Test Fails : Number of games that failed testing
        /// </summary>
        public int TestFails {get; set; }

        public int DiscScratched { get; set; }
        public int CaseOrInstructionsInPoorCondition { get; set; }
        public int GameFailedTesting { get; set; }



        /// <summary>
        /// Test Pass : Number of games that passed testing 
        /// </summary>
        public int TestPass { get; set; }
        /// <summary>
        /// DIBz : Number of successful swaps
        /// </summary>
        public int DIBz { get; set; }

        //public virtual ApplicationUser ApplicationUser { get; set; }
        //[ForeignKey("ApplicationUser")]
        //public int ApplicationUserId { get; set; }
    }
}
