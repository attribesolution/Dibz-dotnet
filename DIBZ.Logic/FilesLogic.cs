using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Common.Model;
using DIBZ.Data;

namespace DIBZ.Logic
{
    public class FilesLogic : BaseLogic
    {
        public FilesLogic(LogicContext context) : base(context)
        {
        }

        public async Task<UploadedFile> GetFileById(int id)
        {
            return (await Db.Query<UploadedFile>(f => f.Id == id).QueryAsync()).FirstOrDefault();
        }
    }
}
