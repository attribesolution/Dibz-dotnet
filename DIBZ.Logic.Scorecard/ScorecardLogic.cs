using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Common.DTO;
using DIBZ.Common.Model;
using DIBZ.Data;

namespace DIBZ.Logic.Scorecard
{
    public class ScorecardLogic:  BaseLogic
    {
        public ScorecardLogic(LogicContext context) : base(context)
        {
        }
        public async Task<DIBZ.Common.Model.Scorecard> GetApplicationUserScorecard(int applicationUserId)
        {
            var appUser= this.GetUserById(applicationUserId);
            return (await Db.Query<DIBZ.Common.Model.Scorecard>(o => o.ApplicationUserId == applicationUserId && o.IsActive && !o.IsDeleted).QueryAsync()).FirstOrDefault();
        }
        public ApplicationUser GetUserById(int id)
        {
            return Db.Query<ApplicationUser>(c => c.Id == id).FirstOrDefault();
        }
        public async Task<DIBZ.Common.Model.Scorecard> GetScoreCardByAppUserId(int id)
        {
            return (await Db.Query<DIBZ.Common.Model.Scorecard> (c => c.ApplicationUserId == id).QueryAsync()).FirstOrDefault();
        }
        
        public async Task<bool> UpdateScoreCardByAppUserId(int id, int swapStatus, int failReasonIntValue, bool isGameSentCase)
        {
            var status = (DIBZ.Common.Model.SwapStatus)swapStatus;
            DIBZ.Common.Model.Scorecard scoreCard = new DIBZ.Common.Model.Scorecard();
            scoreCard = await GetScoreCardByAppUserId(id);
            if (scoreCard != null)
            {
                if (isGameSentCase == true)
                {
                    if (status == DIBZ.Common.Model.SwapStatus.Game1_Received || status == DIBZ.Common.Model.SwapStatus.Game2_Received)
                    {
                        var count = scoreCard.GamesSent;
                        scoreCard.GamesSent = count + 1;
                    }
                }
                else if (status == DIBZ.Common.Model.SwapStatus.Game1_NoShow || status == DIBZ.Common.Model.SwapStatus.Game2_NoShow)
                {
                    var count = scoreCard.NoShows;
                    scoreCard.NoShows = count + 1;
                }
                
                else if (status == DIBZ.Common.Model.SwapStatus.Test_Pass)
                {
                    var count = scoreCard.TestPass;
                    scoreCard.TestPass = count + 1;
                }
                else if (status == DIBZ.Common.Model.SwapStatus.Test_Fail && failReasonIntValue > 0)
                {
                    int failCaseCount = 0;
                    var count = scoreCard.TestFails;
                    scoreCard.TestFails = count + 1;
                    if (failReasonIntValue == (int)DIBZ.Common.Model.FailCasses.CaseOrInstructionsInPoorCondition)
                    {
                        failCaseCount = scoreCard.CaseOrInstructionsInPoorCondition;
                        scoreCard.CaseOrInstructionsInPoorCondition = failCaseCount + 1;
                    }
                    else if (failReasonIntValue == (int)DIBZ.Common.Model.FailCasses.DiscScratched)
                    {
                        failCaseCount = scoreCard.DiscScratched;
                        scoreCard.DiscScratched = failCaseCount + 1;
                    }
                    else if (failReasonIntValue == (int)DIBZ.Common.Model.FailCasses.GameFailedTesting)
                    {
                        failCaseCount = scoreCard.GameFailedTesting;
                        scoreCard.GameFailedTesting = failCaseCount + 1;
                    }

                }
                else if (status == DIBZ.Common.Model.SwapStatus.Dispatched)
                {
                    var count = scoreCard.DIBz;
                    scoreCard.DIBz = count + 1;
                }
                await Db.SaveAsync();
            }
            return true;
        }

        public async Task<int> UpdateScoreCardOfApplicationUserWithNoShow(int applicationUserId)
        {
            DIBZ.Common.Model.Scorecard scoreCard = (await Db.Query<DIBZ.Common.Model.Scorecard>(c => c.ApplicationUserId == applicationUserId).QueryAsync()).FirstOrDefault();
            if (scoreCard == null)
            {
                scoreCard = new DIBZ.Common.Model.Scorecard();
                scoreCard.ApplicationUserId = applicationUserId;
                scoreCard.Proposals += 1;
                scoreCard.NoShows += 1;
                Db.Add(scoreCard);
            }
            else
            {
                scoreCard.NoShows++;
            }
            
           return await Db.SaveAsync();

        }
    }
}
