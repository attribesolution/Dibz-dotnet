using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.Model
{
    public enum GameFormat
    {
        Nintendo_Switch = 1,
        Xbox1,
        PS4
    }
    public enum Gender
    {
        Male = 1,
        Female
    }
    public enum OfferStatus
    {
        Pending=1,
        Accept,
        NotAvailable,
        Cancel,
        Reject,
        PaymentNeeded
    }
    public enum SwapStatus
    {
        Accepted=1,
        Payment_Done_By_Offerer,
        Payment_Done_By_Swapper,
        Payment_Successful,
        Game1_NoShow,
        Game2_NoShow,
        All_NoShow,
        Game1_Received,
        Game2_Received,
        Testing,
        Test_Pass,
        Test_Fail,
        Dispatched,
        Returned,
        Swap_Cancel_By_Offerer,
        Swap_Cancel_By_Swapper
    }
    public enum NotificationStatus
    {
       Unseen = 1,
       seen,
       UnRead,
       Read
    }
    public enum NotificationType
    {
        Desktop = 1,
        Email
    }

    public enum NotificationBusinessType
    {
        CreateOffer = 1,
        CounterOffer,
        AcceptOffer,
        SwapAction,
        CRMAdminReply,
    }

    public enum FailCasses
    {
        DiscScratched = 1,
        CaseOrInstructionsInPoorCondition,
        GameFailedTesting,

    }


    public enum Channel
    {
        Web = 1,
        Andriod,
        Ios,

    }

    public enum QueryStatus
    {
        Open = 1,
        Close
    }
    public enum LocationType
    {
        Location1=1,
        Location2
    }

    public enum EmailContentType
    {
       Game_1_Recieved =1,
       AddInterest,
       AcceptOffer,
       DeclineOffer,
       CreateOffer,
       PaymentDone,
       Game_1_NoShow,
       Game_2_NoShow,
       All_NoShow,
       Game_2_Recieved,
       Testing,
       Test_Pass,
       Test_Fail,
       Dispatch,
       Return,

       PeriodictPaymentReminder,
       FinalPaymentReminder,
       PaymentTimeRanOut,
       ExpiryDayRule,
       ForgotPassword,
       SignUp,
       SwapCancel,
       NewsletterSubscribe,
       UserFirstGame
    }
    public enum Priority
    {
        High = 1,
        Medium,
        Low
    }
    public enum EmailType
    {
        Email=1,
    }
    public enum PageSize
    {
        AllGames = 4,
        SearchOffer = 4,
        FirstPage = 1,
        DefaultPageSize = 5
    }
}
