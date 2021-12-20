using BalarinaAPI.Authentication;
using BalarinaAPI.Core.Model;
using BalarinaAPI.Core.ViewModel;
using BalarinaAPI.Core.ViewModel.Category;
using BalarinaAPI.Core.ViewModel.Season;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelAPI.Core;
using TravelAPI.Core.Helper;

namespace BalarinaAPI.Controllers.Season 
{
    [Route("api/[controller]")]
    [ApiController]
    public class seasonController : ControllerBase
    {
        #region variables
        private readonly IUnitOfWork unitOfWork;
        private readonly Helper helper;
        private readonly TrendingDuration trendingDuration = new TrendingDuration();

        #endregion

        #region Constructor
        public seasonController(IUnitOfWork _unitOfWork, IOptions<Helper> _helper)
        {
            unitOfWork = _unitOfWork;
            this.helper = _helper.Value;
        }
        #endregion

        #region CRUD OPERATION

        #region Get All Season API Key Authentication
        [ApiAuthentication]
        [HttpGet]
        [Route("getallseasonsapikey")]
        public async Task<ActionResult<List<SeasonModel>>> getallseasonsapikeyAsync()
        {
            try
            {
                List<SeasonModel> seasons = new List<SeasonModel>();
                //Get All Programs 
                var ResultSeasons = await unitOfWork.Season.GetObjects(); ResultSeasons.ToList();
                var ResultPrograms = await unitOfWork.Program.GetObjects(); ResultPrograms.ToList();

                var result = (from program in ResultPrograms 
                              join season in ResultSeasons
                                on program.ProgramId equals season.ProgramId
                                 select new
                                 {
                                     season.SessionId,
                                     season.SessionTitle,
                                     season.CreationDate,
                                     season.SeasonViews,
                                     program.ProgramId,
                                     program.ProgramName  ,
                                     season.SeasonIndex
                                 }).ToList();
                foreach (var item in result)
                {
                    var EpisodesRelated = await unitOfWork.Episode.GetObjects(x => x.SessionId == item.SessionId);

                    SeasonModel model = new SeasonModel()
                    {
                        SessionId = item.SessionId,
                        ProgramId = item.ProgramId,
                        CreationDate = item.CreationDate,
                        SeasonViews =item.SeasonViews,
                        SessionTitle = item.SessionTitle,
                        ProgramName = item.ProgramName,
                        EpisodesCount = EpisodesRelated.Count() ,
                        SeasonIndex = item.SeasonIndex
                    };
                    seasons.Add(model);
                }
                return seasons;
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
            }
        }
        #endregion

        #region Get All Season => Dashboard
        [ApiAuthentication]
        [HttpGet]
        [Route("getallseasons")] 
        public async Task<ActionResult<List<SeasonModel>>> getallseasonsAsync()
        {
            try
            {
                List<SeasonModel> seasons = new List<SeasonModel>();
                //Get All Programs 
                var ResultSeasons = await unitOfWork.Season.GetObjects(); ResultSeasons.ToList();
                var ResultPrograms = await unitOfWork.Program.GetObjects(); ResultPrograms.ToList();

                var result = (from program in ResultPrograms
                              join season in ResultSeasons
                                on program.ProgramId equals season.ProgramId
                              select new
                              {
                                  season.SessionId,
                                  season.SessionTitle,
                                  season.CreationDate,
                                  season.SeasonViews,
                                  program.ProgramId,
                                  program.ProgramName
                              }).ToList();
                foreach (var item in result)
                {
                    var EpisodesRelated = await unitOfWork.Episode.GetObjects(x => x.SessionId == item.SessionId);
                    int SeasonViews = EpisodesRelated.Sum(x => x.EpisodeViews);

                    SeasonModel model = new SeasonModel()
                    {
                        SessionId = item.SessionId,
                        ProgramId = item.ProgramId,
                        CreationDate = item.CreationDate,
                        SeasonViews = SeasonViews,
                        SessionTitle = item.SessionTitle,
                        ProgramName = item.ProgramName,
                        EpisodesCount = EpisodesRelated.Count()
                    };
                    seasons.Add(model);
                }
                return seasons;
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
            }
        }
        #endregion

        #region Get All season Name , ID 
        /// <summary>
        /// Get All season Name , ID 
        /// </summary>
        /// <returns>
        /// List of Object that Contains 
        /// Id , Name
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("getseason_id_name")]
        public async Task<ActionResult<List<ListOfNameID<Object_ID_Name>>>> GetSeason_ID_Name()
        {
            try
            {
                //check If Category ID If Exist
                var _SeasonObject = await unitOfWork.Season.GetObjects(); _SeasonObject.ToList();
                if (_SeasonObject == null)
                    return BadRequest("Season list is empty ");
                //Get All Programs 

                List<ListOfNameID<Object_ID_Name>> Collection = new List<ListOfNameID<Object_ID_Name>>();
                foreach (var item in _SeasonObject)
                {
                    ListOfNameID<Object_ID_Name> obj = new ListOfNameID<Object_ID_Name>() { ID = item.SessionId, Name = item.SessionTitle };
                    Collection.Add(obj);
                }
                return Collection;
            }
            catch (Exception ex)
            {
                // Log error in db
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Get All Season Name , ID  related with Program ID
        /// <summary>
        /// Get All Season Name , ID 
        /// </summary>
        /// <returns>
        /// List of Object that Contains 
        /// Id , Name
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("getseasonidname_withprogramid")] 
        public async Task<ActionResult<List<ListOfNameID<Object_ID_Name>>>> getseasonidname_withprogramid(int ID)
        {
            try
            {
                //check If Category ID If Exist
                var _SeasonObject = await unitOfWork.Season.GetObjects(x=>x.ProgramId == ID); _SeasonObject.ToList();
                //Get All Programs 

                List<ListOfNameID<Object_ID_Name>> Collection = new List<ListOfNameID<Object_ID_Name>>();
                foreach (var item in _SeasonObject)
                {
                    ListOfNameID<Object_ID_Name> obj = new ListOfNameID<Object_ID_Name>() { ID = item.SessionId, Name = item.SessionTitle };
                    Collection.Add(obj);
                }
                return Collection;
            }
            catch (Exception ex)
            {
                // Log error in db
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Get All Season Related With ProgramID API Key Authentication
        [ApiAuthentication]
        [HttpGet]
        [Route("getallseasonsbyprogramid")]
        public async Task<ActionResult<IEnumerable<Seasons>>> getallseasonsbyprogramid(int ID)
        {
            try
            {
                #region Check Program ID Exist or Not
                var programObj = await unitOfWork.Program.FindObjectAsync(ID);
                if (programObj == null)
                    return BadRequest("Program ID Not Found ");
                #endregion
                //Get All Programs 
                var ResultSeasons = await unitOfWork.Season.GetObjects(x=>x.ProgramId == ID);

                #region Increase Program Views 
                programObj.ProgramViews += 1;
                #endregion

                #region update operation
                bool result = unitOfWork.Program.Update(programObj);
                await unitOfWork.Complete();

                #endregion

                #region check operation is updated successed
                if (!result)
                    return BadRequest("Create Operation Failed");
                #endregion

                return ResultSeasons.ToList();
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
            }
        }
        #endregion

        #region Get First Season , First Episode Related With ProgramID 
        [ApiAuthentication]
        [HttpGet]
        [Route("getfirstseasonsbyprogramid")]
        public async Task<ActionResult<FirstSeasonInProgram>> getfirstseasonsbyprogramid(int ID)
        { 
            try
            {   
                #region Check Program ID Exist or Not
                var programObj = await unitOfWork.Program.FindObjectAsync(ID);
                if (programObj == null)
                    return BadRequest("Program ID Not Found ");
                #endregion
                //Get All Programs 
                var ResultSeasons = await unitOfWork.Season.GetObjects(x => x.ProgramId == ID);
                FirstSeasonInProgram firstSeason = new FirstSeasonInProgram()
                {
                    FirstSeasonID = ResultSeasons.
                }




                return ResultSeasons.ToList();
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
            }
        }
        #endregion

        #region Insert New Season 
        [ApiAuthentication]
        [HttpPost]
        [Route("createseason")]
        public async Task<ActionResult<SeasonInput>> createseasonAsync([FromQuery] SeasonInput  model )
        {
            try
            {
                var programID = await unitOfWork.Program.FindObjectAsync(model.ProgramId);
                if (programID == null)
                    return BadRequest("program ID Not Found ");

                if (string.IsNullOrEmpty(model.SessionTitle))
                    return BadRequest(" Season title cannot be null or empty");

                Seasons _season = new Seasons()
                {
                     SessionTitle = model.SessionTitle,
                     ProgramId = model.ProgramId,
                     CreationDate = DateTime.Now,
                     SeasonViews = model.SeasonViews
                };

                var Seasons = await unitOfWork.Season.GetObjects();
                if (Seasons.Where(x => x.ProgramId == model.ProgramId).Count() == 0)
                    _season.SeasonIndex = Seasons.Where(x => x.ProgramId == model.ProgramId).Count();
                else  _season.SeasonIndex = Seasons.Where(x => x.ProgramId == model.ProgramId).Count() - 1 ;

                bool result = await unitOfWork.Season.Create(_season);

                if (!result)
                    return BadRequest("Create Operation Failed");

                await unitOfWork.Complete();

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Edit Season
        [ApiAuthentication]
        [HttpPut]
        [Route("putseason")]
        public async Task<ActionResult<SeasonToUpdate>> putseason([FromQuery] SeasonToUpdate model)
        {
            try
            {
                var SeasonObj =await  unitOfWork.Season.FindObjectAsync(model.SessionId);
                if (SeasonObj == null)
                    return BadRequest("Season ID Not Found ");

                if(model.SessionTitle == null)
                    model.SessionTitle = SeasonObj.SessionTitle;

                if (model.SeasonViews == null)
                    model.SeasonViews = SeasonObj.SeasonViews;

                if (model.ProgramId == null)
                    model.ProgramId = SeasonObj.ProgramId;
                else
                {
                    var programObj = await unitOfWork.Program.GetObjects(x => x.ProgramId == model.ProgramId);
                    if (programObj == null)
                        return BadRequest("Program ID Not Found ");
                }

                Seasons _season = new Seasons()
                {
                  SessionId= model.SessionId,
                  ProgramId =(int) model.ProgramId,
                  SessionTitle = model.SessionTitle,
                  SeasonViews = (int)model.SeasonViews
                };

                bool result = unitOfWork.Season.Update(_season);

                if (!result)
                    return BadRequest("UPDATE OPERATION FAILED ");

                await unitOfWork.Complete();

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                string strError1 = ex.Message;
                string strError2 = ex.StackTrace;
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Delete season
        //[ApiAuthentication]
        [HttpDelete("{ID}")]
        public async Task<ActionResult<Seasons>> deleteseasonAsync(int ID)
        {
            try
            {
                bool result = await unitOfWork.Season.DeleteObject(ID);
                if (!result)
                    return BadRequest("season Not Exist");
                await unitOfWork.Complete();

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #endregion
    }
}
 