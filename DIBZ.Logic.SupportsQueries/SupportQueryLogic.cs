using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Common.DTO;
using DIBZ.Common.Model;
using DIBZ.Data;
using DIBZ.Common;
using DIBZ.Logic;
//using DIBZ.Base;


namespace DIBZ.Logic.SupportsQueries
{

    public class SupportQueryLogic : BaseLogic
    {
        public SupportQueryLogic(LogicContext context) : base(context)
        {

        }
        //save Contact us form data for Application User
        public int SaveMessages(string adminEmailAddress, string name, string phone, string subject, string message,int? appuserid)
        {
            DIBZ.Common.Model.MyQueries myQuery = new DIBZ.Common.Model.MyQueries();
            myQuery.AppUserId = appuserid;
            myQuery.Email = adminEmailAddress;
            myQuery.Name = name;
            myQuery.PhoneNo = phone;
            myQuery.Subject = subject;
            myQuery.Message = message;
            myQuery.QueryStatus = QueryStatus.Open;
            myQuery.IsActive = true;
            DIBZ.Common.Model.MyQueryDetails myQueryDetails = new DIBZ.Common.Model.MyQueryDetails();
            myQueryDetails.Message = message;
            myQueryDetails.myquery = myQuery;
            Db.Add(myQueryDetails);
            return Db.Save();
            
        }

        //public int SaveMessagesForNotAppUser(string adminEmailAddress, string name, string phone, string subject, string message)
        //{
        //    DIBZ.Common.Model.MyQueries myQuery = new DIBZ.Common.Model.MyQueries();
        //    myQuery.Email = adminEmailAddress;
        //    myQuery.Name = name;
        //    myQuery.PhoneNo = phone;
        //    myQuery.Subject = subject;
        //    myQuery.Message = message;
        //    myQuery.QueryStatus = QueryStatus.Open;
        //    myQuery.IsActive = true;
        //    DIBZ.Common.Model.MyQueryDetails myQueryDetails = new DIBZ.Common.Model.MyQueryDetails();
        //    myQueryDetails.Message = message;
        //    myQueryDetails.myquery = myQuery;
        //    Db.Add(myQueryDetails);
        //    return Db.Save();

        //}




        //save conversation message via Admin or ApplicationUser
        public int SaveConversationForAppUser( int queryId, string Message)
        {
            DIBZ.Common.Model.MyQueryDetails queryLogs = new DIBZ.Common.Model.MyQueryDetails();
            queryLogs.MyQueryId = queryId;
            queryLogs.Message = Message;
            queryLogs.IsActive = true;
            Db.Add(queryLogs);
            return Db.Save(); 
        }

        //public DIBZ.Common.Model.MyQueryDetails SaveConversationAdmin(int adminId, int queryId, string Message)
        //{
        //    DIBZ.Common.Model.MyQueryDetails queryLogs = new DIBZ.Common.Model.MyQueryDetails();
        //    queryLogs.AdminId = adminId;
        //    queryLogs.MyQueryId = queryId;
        //    queryLogs.Message = Message;
        //    queryLogs.IsActive = true;
        //    Db.Add(queryLogs);
        //    Db.Save();
        //    return queryLogs;
        //}


        public int SaveConversationAdmin(int adminId, int queryId, string Message)
        {
            DIBZ.Common.Model.MyQueryDetails queryLogs = new DIBZ.Common.Model.MyQueryDetails();
            queryLogs.AdminId = adminId;
            queryLogs.MyQueryId = queryId;
            queryLogs.Message = Message;
            queryLogs.IsActive = true;
            Db.Add(queryLogs);
            return Db.Save();   
        }


        //  Select Data For Application User From Contact Us Form
        public List<DIBZ.Common.DTO.MyQueriesModel> GetMessagesByApplicationUserEmail(string appUserEmail)
        {
           var response = Db.Query<DIBZ.Common.Model.MyQueries>(o => !o.IsDeletedByAppUser && o.Email == appUserEmail).OrderBy(e=> e.CreatedTime).ToList();
            var myData = response.Select(e => new DIBZ.Common.DTO.MyQueriesModel
            {
                Id = e.Id,
                CreatedTime = e.CreatedTime,
                Message = e.Message,
                Subject = e.Subject,
                Name = e.Name,
                LastUpdateBy = (e.querylog.OrderByDescending(o => o.Id).FirstOrDefault().AdminId == null) ? e.Name : e.querylog.OrderByDescending(o => o.Id).FirstOrDefault().admin.FirstName,
                QueryStatus = e.QueryStatus,
            }).OrderByDescending(o=> o.CreatedTime).ToList();
            return myData;
        }
        
      //  Select Data For Admin Against All Queryies From Contact Us Form
        public List<DIBZ.Common.DTO.MyQueriesModel> GetMyQueries()
        {
            var response = Db.Query<MyQueries>(o => !o.IsDeletedByAdmin).ToList();
            var myData = response.Select(e => new DIBZ.Common.DTO.MyQueriesModel
            {
                Id = e.Id,
                CreatedTime = e.CreatedTime,
                Name = e.Name,
                Email = e.Email,
                PhoneNo = e.PhoneNo,
                Subject = e.Subject,
                Message = e.Message,
                LastUpdateBy = (e.querylog.OrderByDescending(o => o.Id).FirstOrDefault().AdminId == null) ? e.Name : e.querylog.OrderByDescending(o => o.Id).FirstOrDefault().admin.FirstName,
                QueryStatus = e.QueryStatus,
            }).ToList();
            return myData;
        }

        public List<DIBZ.Common.DTO.MyQueriesModel> GetQueryDetailById(int queryId)
        {

            //var response = Db.Query<MyQueryDetails>(o => !o.IsDeleted && o.MyQueryId == queryId);
            //var myData = response.Select(e => new DIBZ.Common.DTO.MyQueriesModel
            //{
            //    Id = e.MyQueryId,
            //    Name = (e.AdminId.HasValue) ? e.admin.FirstName : e.myquery.Name,
            //    UserImageId = (e.AdminId.HasValue) ? 0 : e.myquery.appuser.FirstOrDefault().ProfileImageId.Value,
            //    CreatedTime = e.CreatedTime,
            //    Message = e.Message,
            //    QueryStatus=e.myquery.QueryStatus,
            //    QueryStatusValue  =(int)e.myquery.QueryStatus
            //}).ToList();
            //return myData;

            var response = Db.Query<MyQueryDetails>(o => !o.IsDeleted && o.MyQueryId == queryId).ToList();
            int userId = response.Select(s => Convert.ToInt32(s.myquery.AppUserId)).FirstOrDefault();
            var userImageProfileId = Db.Query<ApplicationUser>(s => s.Id == userId).Select(d => d.ProfileImageId).FirstOrDefault();
            var myData = response.Select(e => new DIBZ.Common.DTO.MyQueriesModel
            {
                Id = e.MyQueryId,
                Name = (e.AdminId.HasValue) ? e.admin.FirstName : e.myquery.Name,
                UserImageId = (e.AdminId.HasValue) ? 0 : userImageProfileId,
                CreatedTime = e.CreatedTime,
                Message = e.Message,
                QueryStatus = e.myquery.QueryStatus,
                QueryStatusValue = (int)e.myquery.QueryStatus
            }).ToList();
            return myData;
        }

        public List<DIBZ.Common.Model.MyQueryDetails> GetMessagesByMyQueryId(int myQueryId)
        {
            return Db.Query<DIBZ.Common.Model.MyQueryDetails>(o => !o.IsDeleted && o.MyQueryId == myQueryId).ToList();
        }


        public DIBZ.Common.Model.MyQueries GetMyQueryInfo(int id)
        {
            return Db.Query<DIBZ.Common.Model.MyQueries>(o =>  id == o.Id).FirstOrDefault();
        }

         public async Task<int> UpdateQueryStatus(int myQueryId,int statusCode)
         {
            var myQueryObj = await Db.GetObjectById<DIBZ.Common.Model.MyQueries>(myQueryId);
            myQueryObj.QueryStatus =(QueryStatus) statusCode;
            return await Db.SaveAsync();
         }

        public async Task<MyQueries> GetQueriesById(int id)
        {
            var queryData = await Db.GetObjectById<MyQueries>(id);
            return queryData;
        }
        
      //  delete message Conversation via Admin or ApplicationUser
        public async Task DeleteForAppUser(int id)
        {
            DIBZ.Common.Model.MyQueries queryLog = null;
            queryLog = await GetQueriesById(id);
            queryLog.IsDeletedByAppUser = true;
            await Db.SaveAsync();
        }


        public async Task DeleteForAdmin(int id)
        {
            DIBZ.Common.Model.MyQueries queryLog = null;
            queryLog = await GetQueriesById(id);
            queryLog.IsDeletedByAdmin = true;
            await Db.SaveAsync();
        }
    }
}
