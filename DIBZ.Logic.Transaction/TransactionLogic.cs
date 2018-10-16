using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Common;
using DIBZ.Common.Model;
using DIBZ.Data;


namespace DIBZ.Logic.Transaction
{
    public class TransactionLogic:BaseLogic
    {
        public TransactionLogic(LogicContext context) : base(context)
        {
        }
        public async Task<int> AddTransaction(int offerId, decimal amount,int userId,int swapId)
        {
            DIBZ.Common.Model.Transaction trans = new DIBZ.Common.Model.Transaction
            {
                IsActive = true,
                Aomunt = amount,
                OfferId = offerId,
                IsPaid = true,
                ApplicationUserId=userId
            };

            Db.Add(trans);

            var swapObj= await Db.GetObjectById<DIBZ.Common.Model.Swap>(swapId);
            DIBZ.Common.Model.Swap swap = new DIBZ.Common.Model.Swap();
            swap = swapObj;
            if (swapObj.GameSwapPsersonId == userId)
            {
                swapObj.SwapStatus = SwapStatus.Payment_Done_By_Swapper;
            }
            else
            {
                swapObj.SwapStatus = SwapStatus.Payment_Done_By_Offerer;
            }
            Db.Add(swap);
            return  await Db.SaveAsync();
        }
    }
}
