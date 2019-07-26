using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Data;

namespace DIBZ.Logic.Banner
{    
     public class BannerLogic : BaseLogic
        {
            public BannerLogic(LogicContext context) : base(context)
            {

            }
            public async Task<List<DIBZ.Common.Model.Banners>> GetAllBanners()
            {
                var bannerData = await Db.Query<DIBZ.Common.Model.Banners>(c => !c.IsDeleted).OrderByDescending(o => o.CreatedTime).ThenByDescending(o => o.UpdatedTime).QueryAsync();

                return bannerData.ToList();
            }

            public async Task<DIBZ.Common.Model.Banners> GetFileById(int id)
            {
                return (await Db.Query<DIBZ.Common.Model.Banners>(f => f.Id == id).QueryAsync()).FirstOrDefault();
            }
            public async Task<Common.Model.Banners> GetBannerById(int id)
            {
                return (await Db.Query<DIBZ.Common.Model.Banners>(c => c.Id == id).QueryAsync()).FirstOrDefault();
            }
            public async Task<IEnumerable<Common.Model.Banners>> GetAllBannerImage()            
            {
                 return await Db.Query<DIBZ.Common.Model.Banners>(c => c.IsActive == true && c.IsDeleted == false).QueryAsync();
            }

        public async Task AddUpdate(Common.Model.Banners request)
            {
                DIBZ.Common.Model.Banners banner = null;
                if (request.Id > 0)
                {

                    banner = await GetBannerById(request.Id);
                    banner.Name = request.Name;
                    banner.Title = request.Title;
                    if(!String.IsNullOrEmpty(request.FileNewName))
                    {
                        banner.FileOrignalName = request.FileOrignalName;
                        banner.FileNewName = request.FileNewName;
                    }                  
                    banner.UpdatedTime = DateTime.Now;
                    banner.IsActive = request.IsActive;


                }
                else
                {
                    banner = new DIBZ.Common.Model.Banners
                    {                                                
                        Name = request.Name,
                        Title = request.Title,
                        FileOrignalName = request.FileOrignalName,
                        FileNewName = request.FileNewName,
                        CreatedTime = DateTime.Now,
                        IsActive = request.IsActive,

                    };

                    Db.Add(banner);

                }
                await Db.SaveAsync();

            }

            public async Task<int> UpdateStatus(int bannerId, bool statusCode)
            {
                var myBannerObj = await Db.GetObjectById<DIBZ.Common.Model.Banners>(bannerId);
                myBannerObj.IsActive = statusCode;
                return await Db.SaveAsync();
            }

            public async Task Delete(int id)
            {
                DIBZ.Common.Model.Banners banner = null;
                if (id > 0)
                {
                    banner = await GetBannerById(id);

                }
                banner.IsDeleted = true;
                await Db.SaveAsync();

            }
        

        }
 
}
