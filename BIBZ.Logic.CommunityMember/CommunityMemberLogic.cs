using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Common.DTO;
using DIBZ.Common.Model;
using DIBZ.Data;
using DIBZ.Logic;

namespace BIBZ.Logic.CommunityMember
{
    public class CommunityMemberLogic : BaseLogic
    {
        public CommunityMemberLogic(LogicContext context) : base(context)
        {
        }

        public async Task<DIBZ.Common.Model.CommunityMember> AddUpdateGameCatalog(DIBZ.Common.Model.CommunityMember request)
        {
            DIBZ.Common.Model.CommunityMember CommunityMember = null;
            if (request.Id > 0)
            {
                CommunityMember = await GetCommunityMemberById(request.Id);
            }
            else
            {
                CommunityMember = new DIBZ.Common.Model.CommunityMember();
            }

            CommunityMember.IsActive = true;
            CommunityMember.IsArchive = true;
            CommunityMember.Address= request.Address;
            CommunityMember.CellNo = request.CellNo;
            CommunityMember.Email = request.Email;
            CommunityMember.Name = request.Name;
            CommunityMember.Password = request.Password;
            await Db.SaveAsync();
            return CommunityMember;
        }

        public async Task<DIBZ.Common.Model.CommunityMember> GetCommunityMemberById(int Id)
        {
            return (await Db.Query<DIBZ.Common.Model.CommunityMember>(c => c.Id == Id).QueryAsync()).FirstOrDefault();
        }

        public async Task<bool> Delete(int Id)
        {
            DIBZ.Common.Model.CommunityMember CommunityMember = null;
            if (Id > 0)
            {
                CommunityMember = await GetCommunityMemberById(Id);

            }
            CommunityMember.IsDeleted = true;
            //We can delete both Hard/soft delete
            //Db.Remove(GameCatalog);
            await Db.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<DIBZ.Common.Model.CommunityMember>> GetAllGameCatalog()
        {
            return await Db.Query<DIBZ.Common.Model.CommunityMember>(c => !c.IsDeleted).QueryAsync();
        }
    }
}
