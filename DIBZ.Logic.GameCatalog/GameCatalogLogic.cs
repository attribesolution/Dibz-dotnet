using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Common.DTO;
using DIBZ.Common.Model;
using DIBZ.Data;
using DIBZ.Logic;

namespace DIBZ.Logic.GameCatalog
{
    public class GameData
    {
        public int AppUserId { get; set; }

        public int GameId { get; set; }
        public string GameName { get; set; }
        public int GameImageId { get; set; }
        public string GameFormat { get; set; }
        public string url { get; set; }
        public string imgpath { get; set; }
        public decimal sellprice { get; set; }
        public decimal cashprice { get; set; }
        public decimal voucherprice { get; set; }
        public DateTime GameAdded { get; set; }
        public bool IsValidForOffer { get; set; }
        public string UserName { get; set; }
        public string Category { get; set; }
        public string Format { get; set; }
    }

    public class GameCatalogLogic : BaseLogic
    {
        public GameCatalogLogic(LogicContext context) : base(context)
        {
        }

        public async Task<DIBZ.Common.Model.GameCatalog> AddUpdateGameCatalog(GameCatalogModel request)
        {
            DIBZ.Common.Model.GameCatalog gameCatalog = null;
            if (request.Id > 0)
            {
                gameCatalog = await GetGameCatalogById(request.Id);
            }
            else
            {
                gameCatalog = new DIBZ.Common.Model.GameCatalog();
            }

            gameCatalog.IsActive = true;
            gameCatalog.Description = request.Description;
            gameCatalog.FormatId = request.FormatId;
            await Db.SaveAsync();
            return gameCatalog;
        }

        public async Task<DIBZ.Common.Model.GameCatalog> AddUpdate(DIBZ.Common.Model.GameCatalog request, string fileName)
        {
            DIBZ.Common.Model.GameCatalog gameCatalog = null;
            if (request.Id > 0)
            {
                gameCatalog = await GetGameCatalogById(request.Id);


                gameCatalog.Name = request.Name;
                gameCatalog.Description = request.Description;
                gameCatalog.FormatId = request.FormatId;
                gameCatalog.IsFeatured = request.IsFeatured;
                gameCatalog.CategoryId = request.CategoryId;

                if (!string.IsNullOrEmpty(fileName))
                {
                    gameCatalog.GameImage = new UploadedFile { Filename = fileName };
                }
                else if (string.IsNullOrEmpty(fileName) && request.GameImageId > 0)
                {
                    gameCatalog.GameImageId = request.GameImageId;
                }

            }
            else
            {
                if (string.IsNullOrEmpty(fileName) && request.GameImageId > 0)
                {
                    gameCatalog = new DIBZ.Common.Model.GameCatalog
                    {
                        IsActive = true,
                        Name = request.Name,
                        Description = request.Description,
                        FormatId = request.FormatId,
                        CategoryId = request.CategoryId,
                        IsFeatured = request.IsFeatured,
                        GameImageId = request.GameImageId
                    };
                }
                else
                {
                    var imgObj = new UploadedFile { Filename = fileName };
                    gameCatalog = new DIBZ.Common.Model.GameCatalog
                    {
                        IsActive = true,
                        Name = request.Name,
                        Description = request.Description,
                        FormatId = request.FormatId,
                        CategoryId = request.CategoryId,
                        IsFeatured = request.IsFeatured,
                        GameImage = imgObj
                    };
                }

                Db.Add(gameCatalog);

            }
            await Db.SaveAsync();
            return gameCatalog;
        }

        public async Task<DIBZ.Common.Model.GameCatalog> GetGameCatalogById(int id)
        {
            return (await Db.Query<DIBZ.Common.Model.GameCatalog>(c => c.Id == id).QueryAsync()).FirstOrDefault();
        }

        public async Task<DIBZ.Common.Model.GameCatalog> GetGameCatalog(int id)
        {
            return (await Db.Query<DIBZ.Common.Model.GameCatalog>(c => c.Id == id).QueryAsync()).FirstOrDefault();
        }

        public async Task<DIBZ.Common.Model.GameCatalog> GetGameCatalogByFormat(int id)
        {
            return (await Db.Query<DIBZ.Common.Model.GameCatalog>(c => c.FormatId == id).QueryAsync()).FirstOrDefault();
        }

        public async Task<bool> Delete(int id)
        {
            DIBZ.Common.Model.GameCatalog gameCatalog = null;
            if (id > 0)
            {
                gameCatalog = await GetGameCatalogById(id);

            }
            gameCatalog.IsDeleted = true;
            await Db.SaveAsync();
            return true;
        }

        public async Task RemoveGameFromCollectionOnDispatch(int applicationUserId, int gameId)
        {
            var gameData = await Db.GetObjectById<DIBZ.Common.Model.GameCatalog>(gameId);
            var userData = await Db.GetObjectById<ApplicationUser>(applicationUserId);

            userData.GameCatalogs.Remove(gameData);
            await Db.SaveAsync();

        }
        public async Task RemoveGameFromCollection(int applicationUserId, int gameId)
        {
            var gameData = await Db.GetObjectById<DIBZ.Common.Model.GameCatalog>(gameId);
            var userData = await Db.GetObjectById<ApplicationUser>(applicationUserId);

            userData.GameCatalogs.Remove(gameData);
            await Db.SaveAsync();

        }
        public async Task AddGameIntoCollection(int applicationUserId, int gameId)
        {
            var gameData = await Db.GetObjectById<DIBZ.Common.Model.GameCatalog>(gameId);
            var userData = await Db.GetObjectById<ApplicationUser>(applicationUserId);

            userData.GameCatalogs.Add(gameData);
            await Db.SaveAsync();

        }
        
        public async Task<List<GameCatalogModel>> GetAllGameCatalog()
        {
            var gameData = await Db.Query<DIBZ.Common.Model.GameCatalog>(c => !c.IsDeleted && c.IsActive).QueryAsync();
            var searchedData = gameData.Select(t => new GameCatalogModel
            {
                Name = t.Name.Trim(),
                FormatName = t.Format.ShortName,
                CategoryName = t.Category.Name,
                Id = t.Id,
                IsFeatured = t.IsFeatured,
                GameImageId = t.GameImageId,
                CreatedTime = t.CreatedTime,
                FormatLongName = t.Format.Name,
                url = t.url,
                imgpath = t.imgpath,
                sellprice = t.sellprice,
                cashprice = t.cashprice,
                voucherprice = t.voucherprice,

            });
            return searchedData.OrderBy(o => o.Name).ToList();
        }

        public List<AdminGameCatalogModel> GetAllGameCatalog(string searchBy, int take, int skip, string sortBy, bool sortDir, out int filteredResultsCount, out int totalResultsCount, string dic)
        {
            if (String.IsNullOrEmpty(searchBy))
            {
                // if we have an empty search then just order the results by Id ascending

                sortDir = true;

            }

            var gameData = Db.Query<DIBZ.Common.Model.GameCatalog>(c => !c.IsDeleted && c.IsActive).ToList();
            if (searchBy != null)
            {
                gameData = gameData.Where(o => o.Name.ToLower().Contains(searchBy.ToLower())).ToList(); //Db.Query<DIBZ.Common.Model.GameCatalog>(o => !o.IsDeleted && o.IsActive && o.Name.ToLower().StartsWith(searchBy.ToLower())).ToList();
            }

            var filteredData = new List<DIBZ.Common.Model.GameCatalog>();
            if (gameData != null)
            {
                filteredData = gameData.OrderBy(o => o.Id).Skip(skip)
                   .Take(take)
                   .ToList();
            }

            int total = gameData.Count();
            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            filteredResultsCount = total;
            totalResultsCount = filteredData.Count();

            var sortedData = sortBy != null && sortBy == "Name" ? dic == "asc" ? filteredData.OrderBy(o => o.Name).ToList() : filteredData.OrderByDescending(o => o.Name).ToList()
            : sortBy == "FormatLongName" ? dic == "asc" ? filteredData.OrderBy(o => o.Format.Name).ToList() : filteredData.OrderByDescending(o => o.Format.Name).ToList()
            : sortBy == "CategoryName" ? dic == "asc" ? filteredData.OrderBy(o => o.Category.Name).ToList() : filteredData.OrderByDescending(o => o.Category.Name).ToList()
            : sortBy == "IsFeatured" ? dic == "asc" ? filteredData.OrderBy(o => o.IsFeatured).ToList() : filteredData.OrderByDescending(o => o.IsFeatured).ToList()
            : filteredData.OrderBy(o => o.Id).ToList();

            var searchedData = sortedData.Select(t => new AdminGameCatalogModel
            {
                Game = string.Format("<img src='/Admin/GameCatalog/GetGameImage?fileId={0}' class='img-responsive rounded img-thumbnail' width='100' height='100'>", t.GameImageId),
                Name = t.Name,
                FormatLongName = t.Format.Name,
                CategoryName = t.Category.Name,
                IsFeatured = t.IsFeatured,
                url = t.url,
                imgpath = t.imgpath,
                sellprice = t.sellprice,
                cashprice = t.cashprice,
                voucherprice =t.voucherprice,
                Action = string.Concat(
               string.Format(" <a class='label btn-info' href='/Admin/GameCatalog/AddUpdate/{0}' title='Edit'>Edit</a>", t.Id),
               string.Format(" <a class='label btn-danger' href='/Admin/GameCatalog/Delete/{0}' onclick=\"return confirm('Are you sure want to delete this game?')\" title='Delete'>Delete</a>", t.Id)
               )


            }).ToList();

            return searchedData;

        }
        public async Task<IEnumerable<DIBZ.Common.Model.GameCatalog>> GetAllFeaturedGames()
        {
            return await Db.Query<DIBZ.Common.Model.GameCatalog>(c => !c.IsDeleted && c.IsActive && c.IsFeatured).QueryAsync();
        }

        public async Task<IEnumerable<GameData>> GetAllGamesOfApplicationUser(int applicationUserId)
        {
            var userData = await Db.GetObjectById<ApplicationUser>(applicationUserId);
            var gameData = userData.GameCatalogs.Select(g => new GameData
            {
                AppUserId = userData.Id,
                GameId = g.Id,
                GameName = g.Name.Trim(),
                GameFormat = g.Format.ShortName,
                GameImageId = g.GameImageId,
                GameAdded = g.CreatedTime,        
                url = g.url,
                imgpath = g.imgpath,
                sellprice =g.sellprice,
                cashprice =g.cashprice,
                voucherprice = g.voucherprice,        
                IsValidForOffer = !g.Offers.Any(o => o.OfferStatus == OfferStatus.Accept && o.ApplicationUserId == applicationUserId)//g.Offers.Any(o =>o.OfferStatus==OfferStatus.Accept && o.Swaps.Any(s => s.OfferId == o.Id))

            }).OrderBy(o => o.GameName).ToList();
            return gameData;
        }

        public IEnumerable<GameData> GetUsersFirstGame()
        {
            DIBZDbContext context = new DIBZDbContext();
            var usersFirstGameData = (from ApplicationUser in context.ApplicationUsers
                                      select new GameData
                                      {
                                          UserName = ApplicationUser.FirstName,
                                          GameName = ApplicationUser.GameCatalogs.FirstOrDefault().Name,
                                          Category = ApplicationUser.GameCatalogs.FirstOrDefault().Category.Name,
                                          Format = ApplicationUser.GameCatalogs.FirstOrDefault().Format.Name
                                      }).ToList();
            return usersFirstGameData;
        }

        public async Task<List<GameCatalogModel>> SearchGameCatalog(int? applicationUserId, SearchOffer Data, int currentPage = 1, bool isCount = false)
        {
            int pageSize = Convert.ToInt16(PageSize.AllGames);
            int skipRecords = (currentPage - 1) * pageSize;
            IEnumerable<DIBZ.Common.Model.GameCatalog> gameCatalogs = null;

            var userData = await Db.GetObjectById<ApplicationUser>((int)applicationUserId);
            var userExistingGameIds = userData.GameCatalogs.Select(g => g.Id).ToList();

            gameCatalogs = await Db.Query<DIBZ.Common.Model.GameCatalog>(o => !userExistingGameIds.Contains(o.Id) && o.Name.ToLower().Contains(Data.GameName.ToLower().Trim()) && o.IsActive && !o.IsDeleted
            && o.CategoryId == (Data.CategoryId == 0 ? o.CategoryId : Data.CategoryId)
            && o.FormatId == (Data.FormatId == 0 ? o.FormatId : Data.FormatId)).OrderByDescending(g => g.CreatedTime).QueryAsync();

            var searchedData = Enumerable.Empty<GameCatalogModel>().ToList();

            if (isCount == true)
            {
                searchedData = gameCatalogs.Select(t => new GameCatalogModel
                {
                    Name = t.Name,
                    FormatName = t.Format.Name,
                    CategoryName = t.Category.Name,
                    Id = t.Id,
                    IsFeatured = t.IsFeatured,
                    GameImageId = t.GameImageId,
                    imgpath = t.imgpath,
                    sellprice = t.sellprice,
                    cashprice = t.cashprice,
                    voucherprice = t.voucherprice,

                }).ToList();
            }
            else
            {
                searchedData = gameCatalogs.Select(t => new GameCatalogModel
                {
                    Name = t.Name,
                    FormatName = t.Format.Name,
                    CategoryName = t.Category.Name,
                    Id = t.Id,
                    IsFeatured = t.IsFeatured,
                    GameImageId = t.GameImageId,
                    imgpath = t.imgpath,
                    sellprice = t.sellprice,
                    cashprice = t.cashprice,
                    voucherprice = t.voucherprice,
                }).Skip(skipRecords).Take(pageSize).ToList();
            }
            return searchedData;
        }

        public async Task DeleteMyGameById(int applicationUserId, int gameId)
        {
            var gameData = await Db.GetObjectById<DIBZ.Common.Model.GameCatalog>(gameId);
            var userData = await Db.GetObjectById<ApplicationUser>(applicationUserId);
            userData.GameCatalogs.Remove(gameData);

            var pendingOffers = await GetAllPendingOffersByGameId(applicationUserId, gameId);
            foreach (var item in pendingOffers)
            {
                item.OfferStatus = OfferStatus.NotAvailable;
                item.IsDeleted = true;
            }
            await Db.SaveAsync();
        }
        public async Task<List<Offer>> GetAllPendingOffersByGameId(int applicationUserId, int gameId)
        {
            return (await Db.Query<DIBZ.Common.Model.Offer>(c => c.GameCatalog.Id == gameId && c.OfferStatus == OfferStatus.Pending && c.ApplicationUserId == applicationUserId).QueryAsync()).ToList();
        }
    }
}