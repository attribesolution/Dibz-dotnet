using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Data;

namespace DIBZ.Logic
{    
     public class DibzChargesLogic : BaseLogic
        {
            public DibzChargesLogic(LogicContext context) : base(context)
            {

            }
            public async Task<List<DIBZ.Common.Model.DibzCharges>> GetAllDibzCharges()
            {
                var chargesData = await Db.Query<DIBZ.Common.Model.DibzCharges>(c => !c.IsDeleted).OrderByDescending(o => o.CreatedTime).ThenByDescending(o => o.UpdatedTime).QueryAsync();

                return chargesData.ToList();
            }

            public async Task<DIBZ.Common.Model.DibzCharges> GetFileById(int id)
            {
                return (await Db.Query<DIBZ.Common.Model.DibzCharges>(f => f.Id == id).QueryAsync()).FirstOrDefault();
            }
            public async Task<Common.Model.DibzCharges> GetDibzChargesById(int id)
            {
                return (await Db.Query<DIBZ.Common.Model.DibzCharges>(c => c.Id == id).QueryAsync()).FirstOrDefault();
            }
            public async Task<List<DIBZ.Common.Model.DibzCharges>> GetAllDibzChargesData()
            {
                var chargesData = await Db.Query<DIBZ.Common.Model.DibzCharges>(c => !c.IsDeleted).OrderByDescending(o => o.CreatedTime).ThenByDescending(o => o.UpdatedTime).QueryAsync();
                return chargesData.ToList();            
            }

        public async Task AddUpdate(Common.Model.DibzCharges request)
            {
                DIBZ.Common.Model.DibzCharges dibzcharges = null;
                if (request.Id > 0)
                {

                    dibzcharges = await GetDibzChargesById(request.Id);                    
                    if(!String.IsNullOrEmpty(request.Charges))
                    {
                    dibzcharges.Charges = request.Charges;                    
                    }
                    dibzcharges.UpdatedTime = DateTime.Now;
                    dibzcharges.IsActive = request.IsActive;


                }
                else
                {
                    dibzcharges = new DIBZ.Common.Model.DibzCharges
                    {
                        Charges = request.Charges,                       
                        CreatedTime = DateTime.Now,
                        IsActive = request.IsActive,

                    };

                    Db.Add(dibzcharges);

                }
                await Db.SaveAsync();

            }

            public async Task<int> UpdateStatus(int bannerId, bool statusCode)
            {
                var myBannerObj = await Db.GetObjectById<DIBZ.Common.Model.DibzCharges>(bannerId);
                myBannerObj.IsActive = statusCode;
                return await Db.SaveAsync();
            }

            public async Task Delete(int id)
            {
                DIBZ.Common.Model.DibzCharges banner = null;
                if (id > 0)
                {
                    banner = await GetDibzChargesById(id);

                }
                banner.IsDeleted = true;
                await Db.SaveAsync();

            }
        
        }
 
}
