using BalarinaAPI.Authentication;
using BalarinaAPI.Core.Model;
using BalarinaAPI.Core.ViewModel;
using BalarinaAPI.Core.ViewModel.Category;
using BalarinaAPI.Core.ViewModel.DetailsAPI;
using BalarinaAPI.Core.ViewModel.Episode;
using BalarinaAPI.Hub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TravelAPI.Core;
using TravelAPI.Core.Helper;

namespace BalarinaAPI.Controllers.Episodes
{
    [Route("api/[controller]")]
    [ApiController]
    public class episodesController : ControllerBase
    {
        #region variables
        private readonly IUnitOfWork unitOfWork;
        private readonly Helper helper;
        private readonly TrendingDuration trendingDuration;

        #endregion

        #region Constructor
        public episodesController(IUnitOfWork _unitOfWork, IOptions<Helper> _helper, IOptions<TrendingDuration> _trendingDuration)
        {
            unitOfWork = _unitOfWork;
            this.helper = _helper.Value;
            this.trendingDuration = _trendingDuration.Value;
        }
        #endregion

        #region CRUD Operation 

        #region Get All Episodes
        /// <summary>
        /// get all episodes with Bearer toacken
        /// </summary>
        /// <returns>
        /// list of episode that cantain EpisodeId ,EpisodeTitle,EpisodeDescription,YoutubeUrl,EpisodeVisible,CreationDate,LikeRate,DislikeRate
        /// EpisodeViews,seasonID,EpisodePublishDate and EpisodeIamgePath + LivePathImages
        /// </returns>
        [Authorize]
        [HttpGet]
        [Route("getallepisodes")]
        public async Task<ActionResult<RetrieveData<Episode>>> getallepisodes()
        {
            try
            {
                var Collection = new RetrieveData<Episode>();
                var ResultEpisodes = await unitOfWork.Episode.GetObjects();
                Collection.Url = helper.LivePathImages;
                Collection.DataList = ResultEpisodes.ToList();
                return Ok(Collection);
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
            }
        }
        #endregion

        #region Get Episodes => Dashboard
        /// <summary>
        /// this function get All Episodes Ordered By Publish Date    
        /// </summary>
        /// <returns>
        ///  return list of Model that contains
        ///  EpisodeId,EpisodeTitle,EpisodeViews,EpisodePublishDate,ProgramId
        ///  ProgramName,ProgramImg,CategoryId,CategoryTitle,ProgramTypeId,ProgramTypeTitle
        /// </returns>

        [ApiAuthentication]
        [HttpGet]
        [Route("episodes")]
        public async Task<ActionResult<RetrieveData<EpisodeModel>>> GetEpisodes()
        {
            try
            {
                #region declare list to fetch output 
                List<EpisodeModel> EpisodeModel = new List<EpisodeModel>();
                List<EpisodeModel> EpisodeModelOrdered = new List<EpisodeModel>();
                #endregion

                #region get Categories , Programs , ProgramTypes , Episodes
                var Interviewers = await unitOfWork.Interviewer.GetObjects(); Interviewers.ToList();
                var Categories = await unitOfWork.category.GetObjects(); Categories.ToList();
                var Programs = await unitOfWork.Program.GetObjects(); Programs.ToList();
                var ProgramTypes = await unitOfWork.ProgramType.GetObjects(); ProgramTypes.ToList();
                var Episodes = await unitOfWork.Episode.GetObjects(); Episodes.ToList();
                var Seasons = await unitOfWork.Season.GetObjects(); Seasons.ToList();
                #endregion

                #region Apply Query in db 
                var Result = (from category in Categories
                              join program in Programs
                              on category.CategoryId equals program.CategoryId
                              join programType in ProgramTypes
                              on program.ProgramTypeId equals programType.ProgramTypeId
                              join interviewer in Interviewers
                                on program.InterviewerId equals interviewer.InterviewerId
                              join season in Seasons
                              on program.ProgramId equals season.ProgramId
                              join episode in Episodes
                              on season.SessionId equals episode.SessionId
                              select new
                              {
                                  season.SessionId,
                                  season.SessionTitle,
                                  episode.EpisodeId,
                                  episode.EpisodeTitle,
                                  episode.EpisodeDescription,
                                  episode.EpisodeViews,
                                  episode.EpisodePublishDate,
                                  episode.EpisodeIamgePath,
                                  program.ProgramId,
                                  program.ProgramName,
                                  program.ProgramImg,
                                  category.CategoryId,
                                  category.CategoryTitle,
                                  programType.ProgramTypeId,
                                  programType.ProgramTypeTitle,
                                  episode.YoutubeUrl,
                                  episode.EpisodeVisible,
                                  interviewer.InterviewerName
                              }).Distinct();
                #endregion

                #region Fill output List from returned list from db
                foreach (var item in Result)
                {
                    string ConvertedDate = item.EpisodePublishDate.ToString();
                    string date = ConvertedDate.Substring(0, ConvertedDate.IndexOf(" "));

                    EpisodeModel model = new EpisodeModel()
                    {
                        SessionId = item.SessionId,
                        SeasonTitle = item.SessionTitle,
                        CategoryId = item.CategoryId,
                        CategoryTitle = item.CategoryTitle,
                        EpisodeId = item.EpisodeId,
                        EpisodePublishDate = item.EpisodePublishDate,
                        EpisodeTitle = item.EpisodeTitle,
                        EpisodeViews = item.EpisodeViews,
                        ProgramId = item.ProgramId,
                        ProgramImg = item.ProgramImg,
                        ProgramName = item.ProgramName,
                        ProgramTypeId = item.ProgramTypeId,
                        ProgramTypeTitle = item.ProgramTypeTitle,
                        EpisodeImg = item.EpisodeIamgePath,
                        EpisodeUrl = item.YoutubeUrl,
                        EpisodeDescription = item.EpisodeDescription,
                        EpisodeVisible = item.EpisodeVisible,
                        InterviewerName = item.InterviewerName,
                        Hour = item.EpisodePublishDate.Hour,
                        Minute = item.EpisodePublishDate.Minute,
                        Date = date
                    };
                    EpisodeModel.Add(model);
                }
                #endregion

                #region Order Episodes by Publish Date 
                EpisodeModelOrdered = EpisodeModel.OrderBy(o => o.EpisodePublishDate).ToList();
                #endregion

                #region Fill DataList And Url
                RetrieveData<EpisodeModel> RetrieveData = new RetrieveData<EpisodeModel>();
                RetrieveData.DataList = EpisodeModel;
                RetrieveData.Url = helper.LivePathImages;
                #endregion

                #region Return Result
                return RetrieveData;
                #endregion
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion


        #region Get All Episode Name , ID  related with Season id
        /// <summary>
        /// Get All Episode Name , ID 
        /// </summary>
        /// <returns>
        /// List of Object that Contains 
        /// Id , Name
        /// </returns>
        [ApiAuthentication]
        [Route("getepisode_id_name")]
        [HttpGet]
        public async Task<ActionResult<List<ListOfNameID<Object_ID_Name>>>> GetEpisode_ID_Name(int ID)
        {
            try
            {
                var _Objects = await unitOfWork.Episode.GetObjects(x => x.SessionId == ID); _Objects.ToList();

                List<ListOfNameID<Object_ID_Name>> Collection = new List<ListOfNameID<Object_ID_Name>>();
                foreach (var item in _Objects)
                {
                    ListOfNameID<Object_ID_Name> obj = new ListOfNameID<Object_ID_Name>() { ID = item.EpisodeId, Name = item.EpisodeTitle };
                    Collection.Add(obj);
                }
                return Collection.ToList();
            }
            catch (Exception ex)
            {
                // Log error in db
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Get All Episodes API Key
        /// <summary>
        /// get all episodes with API Key
        /// </summary>
        /// <returns>
        /// list of episode that cantain EpisodeId ,EpisodeTitle,EpisodeDescription,YoutubeUrl,EpisodeVisible,CreationDate,LikeRate,DislikeRate
        /// EpisodeViews,seasonID,EpisodePublishDate and EpisodeIamgePath + LivePathImages
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("getallepisodesapikey")]
        public async Task<ActionResult<RetrieveData<Episode>>> getallepisodesapikey()
        {
            try
            {
                var Collection = new RetrieveData<Episode>();
                var ResultEpisodes = await unitOfWork.Episode.GetObjects();
                Collection.Url = helper.LivePathImages;
                Collection.DataList = ResultEpisodes.ToList();
                return Ok(Collection);
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
            }
        }
        #endregion

        #region Get All Episodes by season id
        /// <summary>
        /// get all episodes with API Key
        /// </summary>
        /// <returns>
        /// list of episode that cantain EpisodeId ,EpisodeTitle,EpisodeDescription,YoutubeUrl,EpisodeVisible,CreationDate,LikeRate,DislikeRate
        /// EpisodeViews,seasonID,EpisodePublishDate and EpisodeIamgePath + LivePathImages
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("getallepisodesbyseasonid")]
        public async Task<ActionResult<RetrieveData<Episode>>> getallepisodesbyseasonid(int ID)
        {
            try
            {
                //Get All Episodes 
                var Collection = new RetrieveData<Episode>();
                var ResultEpisodes = await unitOfWork.Episode.GetObjects(x => x.SessionId == ID);
                Collection.Url = helper.LivePathImages;
                Collection.DataList = ResultEpisodes.ToList();
                return Ok(Collection);
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
            }
        }
        #endregion


        #region Insert New Episode 
        /// <summary>
        /// insert new episode 
        /// </summary>
        /// <param name="model">
        /// EpisodeTitle EpisodeDescription,YoutubeUrl,EpisodeVisible,LikeRate,DislikeRate,EpisodeViews,SeasonId,EpisodeIamge,EpisodePublishDate
        /// </param>
        /// <returns>
        /// status of operation - Created Successfully - or Status500InternalServerError
        /// </returns>
        [ApiAuthentication]
        [HttpPost]
        [Route("createepisode")]
        public async Task<ActionResult<EpisodeModelInput>> createepisodeAsync([FromQuery] EpisodeModelInput model)
        {
            try
            {
                DateTime EpisodePublishDate = new DateTime();
                var ProgramImage = HttpContext.Request.Form.Files["EpisodeIamge"];
                model.EpisodeIamge = ProgramImage;

                #region Check values of Episodes is not null or empty
                if (string.IsNullOrEmpty(model.EpisodeTitle))
                    return BadRequest("Episode Title cannot be null or empty");

                if (string.IsNullOrEmpty(model.EpisodeDescription))
                    return BadRequest("Episode Description cannot be null or empty");

                if (string.IsNullOrEmpty(model.YoutubeUrl))
                    return BadRequest("Episode Url cannot be null or empty");

                if (model.EpisodeIamge == null)
                    return BadRequest("Episode Image cannot be null");


                if (model.EpisodeVisible != true || model.EpisodeVisible != false)
                    model.EpisodeVisible = true;

                if (model.LikeRate == null)
                    model.LikeRate = 0;

                if (model.DislikeRate == null)
                    model.DislikeRate = 0;

                if (model.EpisodeViews == null)
                    model.EpisodeViews = 0;

                var SeasonId =await unitOfWork.Season.FindObjectAsync(model.SeasonId);
                if (SeasonId == null)
                    return BadRequest("Season ID not found ");

                if (model.EpisodePublishDate.Contains("T"))
                {
                    model.EpisodePublishDate = model.EpisodePublishDate.Substring(0, model.EpisodePublishDate.IndexOf("T"));
                }

                EpisodePublishDate = DateTime.ParseExact(model.EpisodePublishDate, "yyyy-MM-dd", null);
                EpisodePublishDate = EpisodePublishDate.AddHours(model.Hour);
                EpisodePublishDate = EpisodePublishDate.AddMinutes(model.Minute);

                #endregion

                #region Fill Episode object with values to insert
                Episode _episode = new Episode()
                {
                    CreationDate = DateTime.Now,
                    DislikeRate = (int)model.LikeRate,
                    EpisodeDescription = model.EpisodeDescription,
                    EpisodePublishDate = EpisodePublishDate,
                    EpisodeTitle = model.EpisodeTitle,
                    LikeRate = (int)model.LikeRate,
                    EpisodeViews = (int)model.EpisodeViews,
                    EpisodeVisible = model.EpisodeVisible,
                    SessionId = model.SeasonId,
                    YoutubeUrl = model.YoutubeUrl,
                    EpisodeIamgePath = helper.UploadImage(model.EpisodeIamge)
                };
                #endregion

                #region Create Operation
                bool result = await unitOfWork.Episode.Create(_episode);
                #endregion

                #region check if Create Operation successed
                if (!result)
                    return BadRequest("Create Operation Failed");
                #endregion

                #region save changes in db
                await unitOfWork.Complete();

                #endregion

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }
        #endregion

        #region Edit Episode
        [ApiAuthentication]
        [HttpPut]
        [Route("putepisode")]
        public async Task<ActionResult<Episode>> putepisodeAsync([FromQuery] EpisodeToUpdate model)
        {
            try
            {
                DateTime EpisodePublishDate = new DateTime();
                var ProgramImage = HttpContext.Request.Form.Files["EpisodeIamge"];
                model.EpisodeImage = ProgramImage;

                #region check Episode id exist

                var _EpisodeObj = await unitOfWork.Episode.FindObjectAsync(model.EpisodeId);
                if (_EpisodeObj == null)
                    return NotFound("Episode ID Not Found");
                #endregion

                #region Check values of Episode is not null or empty

                if (string.IsNullOrEmpty(model.EpisodeTitle))
                    model.EpisodeTitle = _EpisodeObj.EpisodeTitle;

                if (string.IsNullOrEmpty(model.EpisodeDescription))
                    model.EpisodeDescription = _EpisodeObj.EpisodeDescription;

                if (string.IsNullOrEmpty(model.YoutubeUrl))
                    model.YoutubeUrl = _EpisodeObj.YoutubeUrl;

                if (model.EpisodeVisible != true || model.EpisodeVisible != false)
                    model.EpisodeVisible = _EpisodeObj.EpisodeVisible;

                if (model.SeasonId == null)
                    model.SeasonId = _EpisodeObj.SessionId;

                if (model.EpisodePublishDate == null)
                    model.EpisodePublishDate = _EpisodeObj.EpisodePublishDate.ToString();

                if(model.changeDate != true)
                {
                    EpisodePublishDate = _EpisodeObj.EpisodePublishDate;
                    EpisodePublishDate = EpisodePublishDate.AddHours(-EpisodePublishDate.Hour);
                    EpisodePublishDate = EpisodePublishDate.AddHours(model.Hour);
                    EpisodePublishDate = EpisodePublishDate.AddMinutes(-EpisodePublishDate.Minute);
                    EpisodePublishDate = EpisodePublishDate.AddMinutes(model.Minute);
                }
                else
                {
                    if (model.EpisodePublishDate.Contains("T"))
                        model.EpisodePublishDate = model.EpisodePublishDate.Substring(0, model.EpisodePublishDate.IndexOf("T"));

                    EpisodePublishDate = DateTime.ParseExact(model.EpisodePublishDate, "yyyy-MM-dd", null);
                    EpisodePublishDate = EpisodePublishDate.AddHours(model.Hour);
                    EpisodePublishDate = EpisodePublishDate.AddMinutes(model.Minute);
                }
               

             

                #endregion

                #region check if image updated or not 
                if (model.EpisodeImagePath == null)
                {
                    if (model.EpisodeImage != null)
                        model.EpisodeImagePath = helper.UploadImage(model.EpisodeImage);
                }
                if (model.EpisodeImagePath == null && model.EpisodeImage == null)
                {
                    model.EpisodeImagePath = _EpisodeObj.EpisodeIamgePath;
                }
                #endregion

                #region fill category object with values to insert 
                Episode _Episode = new Episode()
                {
                    EpisodeId = model.EpisodeId,
                    CreationDate = DateTime.Now,
                    DislikeRate = _EpisodeObj.DislikeRate,
                    LikeRate = _EpisodeObj.LikeRate,
                    EpisodeDescription = model.EpisodeDescription,
                    EpisodePublishDate = EpisodePublishDate,
                    EpisodeTitle = model.EpisodeTitle,
                    EpisodeViews = _EpisodeObj.EpisodeViews,
                    EpisodeVisible = model.EpisodeVisible,
                    YoutubeUrl = model.YoutubeUrl,
                    SessionId = (int)model.SeasonId,
                    EpisodeIamgePath = model.EpisodeImagePath
                };
                #endregion

                #region update operation
                bool result = unitOfWork.Episode.Update(_Episode);
                #endregion

                #region check operation is updated successed
                if (!result)
                    return BadRequest("UPDATE OPERATION FAILED");
                #endregion

                #region save changes into db
                await unitOfWork.Complete();
                #endregion

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Delete Episode

        //[Authorize]
        [HttpDelete("{ID}")]
        public async Task<ActionResult<Category>> deleteepisodeAsync(int ID)
        {
            try
            {
                #region Check ID If Exist
                var checkIDIfExist = await unitOfWork.Episode.FindObjectAsync(ID);
                if (checkIDIfExist == null)
                    return NotFound("Episode ID Not Found");
                #endregion

                #region Delete Operation
                bool result = await unitOfWork.Episode.DeleteObject(ID);
                #endregion

                #region check Delete Operation  successed
                if (!result)
                    return NotFound("DELETE OPERATION FAILED ");
                #endregion

                #region save changes in db
                await unitOfWork.Complete();
                #endregion

                #region Delete image File From Specified Directory 
                helper.DeleteFiles(checkIDIfExist.EpisodeIamgePath);
                #endregion

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

        #region Get Episodes Filter ForRecently
        [ApiAuthentication]
        [HttpGet]
        [Route("episodesfilterforrecently")]
        public async Task<ActionResult<RetrieveData<EpisodesRelatedForRecentlyModel>>> episodesfilterforrecently([FromQuery] EpisodesFilterForRecentlyInputs inputs)
        {
            try
            {
                #region get Categories , Programs , ProgramTypes , Episodes
                var Interviewers = await unitOfWork.Interviewer.GetObjects(x => x.InterviewerId == inputs.InterviewerID || inputs.InterviewerID == null); Interviewers.ToList();
                var Categories = await unitOfWork.category.GetObjects(x => x.CategoryId == inputs.CategoryID || inputs.CategoryID == null); Categories.ToList();
                var Programs = await unitOfWork.Program.GetObjects(x => x.ProgramId == inputs.ProgramID || inputs.ProgramID == null); Programs.ToList();
                var ProgramTypes = await unitOfWork.ProgramType.GetObjects(x => x.ProgramTypeId == inputs.ProgramTypeID || inputs.ProgramTypeID == null); ProgramTypes.ToList();
                var Episodes = await unitOfWork.Episode.GetObjects(x => x.EpisodePublishDate >= inputs.DateFrom || inputs.DateFrom == null &&
                              x.EpisodePublishDate <= inputs.DateTo || inputs.DateTo == null && x.EpisodeId == inputs.EpisodeID ||  inputs.EpisodeID == null); Episodes.ToList();
                var Seasons = await unitOfWork.Season.GetObjects(x=>x.SessionId == inputs.SeasonID || inputs.SeasonID ==null ); Seasons.ToList();
                #endregion

                #region declare list to fetch output 
                List<EpisodesRelatedForRecentlyModel> episodesRelatedForRecently = new List<EpisodesRelatedForRecentlyModel>();
                List<EpisodesRelatedForRecentlyModel> episodesRelatedForRecentlyOrdered = new List<EpisodesRelatedForRecentlyModel>();

                #endregion

                #region Apply Query in db 
                var Result = (from category in Categories
                              join program in Programs
                              on category.CategoryId equals program.CategoryId
                              join programType in ProgramTypes
                              on program.ProgramTypeId equals programType.ProgramTypeId
                              join interviewer in Interviewers
                                on program.InterviewerId equals interviewer.InterviewerId
                              join season in Seasons
                              on program.ProgramId equals season.ProgramId
                              join episode in Episodes
                              on season.SessionId equals episode.SessionId
                              //where (category.CategoryId == inputs.CategoryID || inputs.CategoryID is null) &&
                              //      (programType.ProgramTypeId == inputs.ProgramTypeID || inputs.ProgramTypeID is null) &&
                              //      (interviewer.InterviewerId == inputs.InterviewerID || inputs.InterviewerID is null) &&
                              //      (program.ProgramId == inputs.ProgramID || inputs.ProgramID is null) &&
                              //      (episode.EpisodePublishDate >= inputs.DateFrom || inputs.DateFrom is null) &&
                              //      (episode.EpisodePublishDate <= inputs.DateTo || inputs.DateTo is null)
                              select new
                              {
                                  season.SessionId,
                                  season.SessionTitle,
                                  season.SeasonIndex,
                                  episode.EpisodeId,
                                  episode.EpisodeTitle,
                                  episode.EpisodeDescription,
                                  episode.EpisodeViews,
                                  episode.EpisodePublishDate,
                                  episode.EpisodeIamgePath,
                                  program.ProgramId,
                                  program.ProgramName,
                                  program.ProgramImg,
                                  category.CategoryId,
                                  category.CategoryTitle,
                                  programType.ProgramTypeId,
                                  programType.ProgramTypeTitle,
                                  episode.YoutubeUrl,
                                  episode.EpisodeVisible,
                              }).Distinct().Take(trendingDuration.Top);
                #endregion

                #region Fill output List from returned list from db
                foreach (var item in Result)
                {
                    EpisodesRelatedForRecentlyModel model = new EpisodesRelatedForRecentlyModel()
                    {
                        SessionId = item.SessionId,
                        SeasonTitle = item.SessionTitle,
                        CategoryId = item.CategoryId,
                        CategoryTitle = item.CategoryTitle,
                        EpisodeId = item.EpisodeId,
                        EpisodePublishDate = item.EpisodePublishDate,
                        EpisodeTitle = item.EpisodeTitle,
                        EpisodeViews = item.EpisodeViews,
                        ProgramId = item.ProgramId,
                        ProgramImg = item.ProgramImg,
                        ProgramName = item.ProgramName,
                        ProgramTypeId = item.ProgramTypeId,
                        ProgramTypeTitle = item.ProgramTypeTitle,
                        EpisodeImg = item.EpisodeIamgePath,
                        EpisodeUrl = item.YoutubeUrl,
                        EpisodeDescription = item.EpisodeDescription,
                        EpisodeVisible = item.EpisodeVisible ,
                        SeasonIndex = item.SeasonIndex
                    };
                    episodesRelatedForRecently.Add(model);
                }
                #endregion

                #region check IsRecently condition

                if (inputs.IsRecently == "desc" || inputs.IsRecently == "DESC")
                {
                    episodesRelatedForRecentlyOrdered = episodesRelatedForRecently.OrderByDescending(o => o.EpisodePublishDate).ToList();
                }
                else
                {
                    episodesRelatedForRecentlyOrdered = episodesRelatedForRecently.OrderBy(o => o.EpisodePublishDate).ToList();
                }
                #endregion

                RetrieveData<EpisodesRelatedForRecentlyModel> RetrieveData = new RetrieveData<EpisodesRelatedForRecentlyModel>();
                RetrieveData.DataList = episodesRelatedForRecentlyOrdered;
                RetrieveData.Url = helper.LivePathImages;


                return RetrieveData;
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Get Episodes Filter For Seasons
        [ApiAuthentication]
        [HttpGet] 
        [Route("episodesfilterforseasons")]
        public async Task<ActionResult<RetrieveData<EpisodesRelatedForSeasons>>> episodesfilterforseasons([FromQuery] EpisodesFilterForRecentlyInputs inputs)
        {
            try
            {
                #region get Categories , Programs , ProgramTypes , Episodes
                var Interviewers = await unitOfWork.Interviewer.GetObjects(x => x.InterviewerId == inputs.InterviewerID || inputs.InterviewerID == null); Interviewers.ToList();
                var Categories = await unitOfWork.category.GetObjects(x => x.CategoryId == inputs.CategoryID || inputs.CategoryID == null); Categories.ToList();
                var Programs = await unitOfWork.Program.GetObjects(x => x.ProgramId == inputs.ProgramID || inputs.ProgramID == null); Programs.ToList();
                var ProgramTypes = await unitOfWork.ProgramType.GetObjects(x => x.ProgramTypeId == inputs.ProgramTypeID || inputs.ProgramTypeID == null); ProgramTypes.ToList();
                var Episodes = await unitOfWork.Episode.GetObjects(x => x.EpisodePublishDate >= inputs.DateFrom || inputs.DateFrom == null &&
                              x.EpisodePublishDate <= inputs.DateTo || inputs.DateTo == null && x.EpisodeId == inputs.EpisodeID || inputs.EpisodeID == null); Episodes.ToList();
                var Seasons = await unitOfWork.Season.GetObjects(x => x.SessionId == inputs.SeasonID || inputs.SeasonID == null); Seasons.ToList();
                #endregion

                #region declare list to fetch output 
                List<EpisodesRelatedForSeasons> episodesRelatedForRecently = new List<EpisodesRelatedForSeasons>();
                List<EpisodesRelatedForSeasons> episodesRelatedForRecentlyOrdered = new List<EpisodesRelatedForSeasons>();

                #endregion

                #region Apply Query in db 
                var Result = (from category in Categories
                              join program in Programs
                              on category.CategoryId equals program.CategoryId
                              join programType in ProgramTypes
                              on program.ProgramTypeId equals programType.ProgramTypeId
                              join interviewer in Interviewers
                                on program.InterviewerId equals interviewer.InterviewerId
                              join season in Seasons
                              on program.ProgramId equals season.ProgramId
                              join episode in Episodes
                              on season.SessionId equals episode.SessionId
                              //where (category.CategoryId == inputs.CategoryID || inputs.CategoryID is null) &&
                              //      (programType.ProgramTypeId == inputs.ProgramTypeID || inputs.ProgramTypeID is null) &&
                              //      (interviewer.InterviewerId == inputs.InterviewerID || inputs.InterviewerID is null) &&
                              //      (program.ProgramId == inputs.ProgramID || inputs.ProgramID is null) &&
                              //      (episode.EpisodePublishDate >= inputs.DateFrom || inputs.DateFrom is null) &&
                              //      (episode.EpisodePublishDate <= inputs.DateTo || inputs.DateTo is null)
                              select new
                              {
                                  season.SessionId,
                                  season.SessionTitle,
                                  season.SeasonIndex,
                                  program.ProgramId,
                                  program.ProgramName,
                                  program.ProgramImg,
                                  category.CategoryId,
                                  category.CategoryTitle,
                                  programType.ProgramTypeId,
                                  programType.ProgramTypeTitle,
                              }).Distinct().Take(trendingDuration.Top);
                #endregion

                #region Fill output List from returned list from db
                foreach (var item in Result)
                {
                    EpisodesRelatedForSeasons model = new EpisodesRelatedForSeasons()
                    {
                        SessionId = item.SessionId,
                        SeasonTitle = item.SessionTitle,
                        CategoryId = item.CategoryId,
                        CategoryTitle = item.CategoryTitle,
                        ProgramId = item.ProgramId,
                        ProgramImg = item.ProgramImg,
                        ProgramName = item.ProgramName,
                        ProgramTypeId = item.ProgramTypeId,
                        ProgramTypeTitle = item.ProgramTypeTitle,
                        SeasonIndex = item.SeasonIndex
                    };
                    episodesRelatedForRecently.Add(model);
                }
                #endregion


                RetrieveData<EpisodesRelatedForSeasons> RetrieveData = new RetrieveData<EpisodesRelatedForSeasons>();
                RetrieveData.DataList = episodesRelatedForRecently;
                RetrieveData.Url = helper.LivePathImages;


                return RetrieveData;
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion


        #region Get Program Filter ForRecently
        [ApiAuthentication]
        [HttpGet]
        [Route("Programfilterforrecently")]
        public async Task<ActionResult<RetrieveData<ProgramFilterModel>>> Programfilterforrecently([FromQuery] EpisodesFilterForRecentlyInputs inputs)
        {
            try
            {
                #region get Categories , Programs , ProgramTypes , Episodes
                var Interviewers = await unitOfWork.Interviewer.GetObjects(); Interviewers.ToList();
                var Categories = await unitOfWork.category.GetObjects(); Categories.ToList();
                var Programs = await unitOfWork.Program.GetObjects(); Programs.ToList();
                var ProgramTypes = await unitOfWork.ProgramType.GetObjects(); ProgramTypes.ToList();
                var Episodes = await unitOfWork.Episode.GetObjects(); Episodes.ToList();
                var Seasons = await unitOfWork.Season.GetObjects(); Seasons.ToList();
                #endregion

                #region declare list to fetch output 
                List<ProgramFilterModel> episodesRelatedForRecently = new List<ProgramFilterModel>();
                List<ProgramFilterModel> episodesRelatedForRecentlyOrdered = new List<ProgramFilterModel>();

                #endregion

                #region Apply Query in db 
                var Result = (from category in Categories
                              join program in Programs
                              on category.CategoryId equals program.CategoryId
                              join programType in ProgramTypes
                              on program.ProgramTypeId equals programType.ProgramTypeId
                              join interviewer in Interviewers
                                on program.InterviewerId equals interviewer.InterviewerId
                              //join season in Seasons
                              //on program.ProgramId equals season.ProgramId
                              //join episode in Episodes
                              //on season.SessionId equals episode.SessionId
                              where (category.CategoryId == inputs.CategoryID || inputs.CategoryID == null) &&
                                    (program.ProgramId == inputs.ProgramID || inputs.ProgramID == null) &&
                                    (interviewer.InterviewerId == inputs.InterviewerID || inputs.InterviewerID == null) &&
                                    (programType.ProgramTypeId == inputs.ProgramTypeID || inputs.ProgramTypeID == null) 
                                    //(season.SessionId == inputs.SeasonID || inputs.SeasonID == null) &&
                                    //(episode.EpisodePublishDate >= inputs.DateFrom || inputs.DateFrom == null && episode.EpisodePublishDate <= inputs.DateTo || inputs.DateTo == null)
                              select new
                              {
                                  program.ProgramId,
                                  program.ProgramDescription,
                                  program.ProgramName,
                                  program.ProgramImg,
                                  program.ProgramPromoUrl,
                                  program.ProgramVisible,
                                  program.CategoryId,
                                  program.ProgramStartDate,
                                  program.InterviewerId,
                                  program.ProgramOrder,
                                  program.ProgramTypeId,
                                  program.ProgramViews,
                                  program.CreationDate,
                                  category.CategoryTitle,
                                  interviewer.InterviewerName,
                                  programType.ProgramTypeTitle,
                                  //season.SessionId

                              }).Distinct().Take(trendingDuration.Top);
                #endregion

                #region Fill output List from returned list from db
                foreach (var item in Result)
                {
                    var LocalSeasons = await unitOfWork.Season.GetObjects(x => x.ProgramId == item.ProgramId);
                    var _FirstSeasonID = LocalSeasons.OrderBy(x => x.CreationDate).FirstOrDefault();

                    //var LocalEpisodes = await unitOfWork.Episode.GetObjects(x => x.SessionId == item.SessionId);
                    //var _FirstEpisodeID = LocalEpisodes.OrderBy(x => x.CreationDate).FirstOrDefault();

                    ProgramFilterModel model = new ProgramFilterModel()
                    {
                        CategoryId = item.CategoryId,
                        CreationDate = item.CreationDate,
                        InterviewerId = item.InterviewerId,
                        ProgramDescription = item.ProgramDescription,
                        ProgramId = item.ProgramId,
                        ProgramImg = item.ProgramImg,
                        ProgramName = item.ProgramName,
                        ProgramOrder = item.ProgramOrder,
                        ProgramPromoUrl = item.ProgramPromoUrl,
                        ProgramStartDate = item.ProgramStartDate,
                        ProgramTypeId = (int)item.ProgramTypeId,
                        ProgramViews = item.ProgramViews,
                        CategoryName = item.CategoryTitle,
                        InterviewerName = item.InterviewerName,
                        ProgramTypeName = item.ProgramTypeTitle,
                        ProgramVisible = (Boolean)item.ProgramVisible,
                        //FirstSeasonID = _FirstSeasonID.SessionId,
                        ////FirstEpisodeID = _FirstEpisodeID.EpisodeId
                    };
                    episodesRelatedForRecently.Add(model);
                }
                #endregion

                #region check IsRecently condition

                if (inputs.IsRecently == "desc" || inputs.IsRecently == "DESC")
                {
                    episodesRelatedForRecentlyOrdered = episodesRelatedForRecently.OrderByDescending(o => o.ProgramStartDate).ToList();
                }
                else
                {
                    episodesRelatedForRecentlyOrdered = episodesRelatedForRecently.OrderBy(o => o.ProgramStartDate).ToList();
                }
                #endregion

                RetrieveData<ProgramFilterModel> RetrieveData = new RetrieveData<ProgramFilterModel>();
                RetrieveData.DataList = episodesRelatedForRecentlyOrdered;
                RetrieveData.Url = helper.LivePathImages;


                return RetrieveData;
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Get Episodes Filter ForViews
        /// <summary>
        /// this function filter Episodes    
        /// </summary>
        /// <param name="CategoryID"></param>
        /// <param name="ProgramTypeID"></param>
        /// <param name="ProgramID"></param>
        /// <param name="DateFrom"></param>
        /// <param name="DateTo"></param>
        /// <param name="IsRecently"></param>
        /// <returns>
        ///  return  return list of Model that contains that contains
        ///  EpisodeId,EpisodeTitle,EpisodeViews,EpisodePublishDate,ProgramId
        ///  ProgramName,ProgramImg,CategoryId,CategoryTitle,ProgramTypeId,ProgramTypeTitle
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("episodesfilterforviews")]
        public async Task<ActionResult<RetrieveData<EpisodesRelatedForRecentlyModel>>> episodesfilterforviews([FromQuery] EpisodesFilterForRecentlyInputs inputs)
        {
            try
            {
                #region get Categories , Programs , ProgramTypes , Episodes
                var Interviewers = await unitOfWork.Interviewer.GetObjects(x => x.InterviewerId == inputs.InterviewerID || inputs.InterviewerID == null); Interviewers.ToList();
                var Categories = await unitOfWork.category.GetObjects(x => x.CategoryId == inputs.CategoryID || inputs.CategoryID == null); Categories.ToList();
                var Programs = await unitOfWork.Program.GetObjects(x => x.ProgramId == inputs.ProgramID || inputs.ProgramID == null); Programs.ToList();
                var ProgramTypes = await unitOfWork.ProgramType.GetObjects(x => x.ProgramTypeId == inputs.ProgramTypeID || inputs.ProgramTypeID == null); ProgramTypes.ToList();
                var Episodes = await unitOfWork.Episode.GetObjects(x => x.EpisodePublishDate >= inputs.DateFrom || inputs.DateFrom == null &&
                              x.EpisodePublishDate <= inputs.DateTo || inputs.DateTo == null); Episodes.ToList();
                var Seasons = await unitOfWork.Season.GetObjects(); Seasons.ToList();
                #endregion

                #region declare list to fetch output 
                List<EpisodesRelatedForRecentlyModel> episodesRelatedForRecently = new List<EpisodesRelatedForRecentlyModel>();
                List<EpisodesRelatedForRecentlyModel> episodesRelatedForRecentlyOrdered = new List<EpisodesRelatedForRecentlyModel>();
                #endregion

                #region Apply Query in db 
                var Result = (from category in Categories
                              join program in Programs
                              on category.CategoryId equals program.CategoryId
                              join programType in ProgramTypes
                              on program.ProgramTypeId equals programType.ProgramTypeId
                              join interviewer in Interviewers
                              on program.InterviewerId equals interviewer.InterviewerId
                              join season in Seasons
                               on program.ProgramId equals season.ProgramId
                              join episode in Episodes
                              on season.SessionId equals episode.SessionId
                              //where (category.CategoryId == inputs.CategoryID || inputs.CategoryID is null) &&
                              //     (programType.ProgramTypeId == inputs.ProgramTypeID || inputs.ProgramTypeID is null) &&
                              //     (interviewer.InterviewerId == inputs.InterviewerID || inputs.InterviewerID is null) &&
                              //     (program.ProgramId == inputs.ProgramID || inputs.ProgramID is null)
                              //&&
                              //(episode.EpisodePublishDate >= inputs.DateFrom || inputs.DateFrom is null) &&
                              //(episode.EpisodePublishDate <= inputs.DateTo || inputs.DateTo is null)
                              select new
                              {
                                  season.SessionId,
                                  season.SessionTitle,
                                  season.SeasonIndex,

                                  episode.EpisodeId,
                                  episode.EpisodeTitle,
                                  episode.EpisodeViews,
                                  episode.EpisodeIamgePath,
                                  episode.YoutubeUrl,
                                  episode.EpisodePublishDate,
                                  episode.EpisodeDescription,

                                  program.ProgramId,
                                  program.ProgramName,
                                  program.ProgramImg,

                                  category.CategoryId,
                                  category.CategoryTitle,

                                  programType.ProgramTypeId,
                                  programType.ProgramTypeTitle

                              }).Distinct().Take(trendingDuration.Top);
                #endregion

                #region Fill output List from returned list from db
                foreach (var item in Result)
                {
                    EpisodesRelatedForRecentlyModel model = new EpisodesRelatedForRecentlyModel()
                    {
                        SessionId = item.SessionId,
                        CategoryId = item.CategoryId,
                        CategoryTitle = item.CategoryTitle,
                        EpisodeId = item.EpisodeId,
                        EpisodePublishDate = item.EpisodePublishDate,
                        EpisodeTitle = item.EpisodeTitle,
                        EpisodeViews = item.EpisodeViews,
                        ProgramId = item.ProgramId,
                        ProgramImg = item.ProgramImg,
                        ProgramName = item.ProgramName,
                        ProgramTypeId = item.ProgramTypeId,
                        ProgramTypeTitle = item.ProgramTypeTitle,
                        EpisodeUrl = item.YoutubeUrl,
                        EpisodeImg = item.EpisodeIamgePath,
                        EpisodeDescription = item.EpisodeDescription,
                        SeasonTitle = item.SessionTitle,
                        SeasonIndex = item.SeasonIndex
                    };
                    episodesRelatedForRecently.Add(model);
                }
                #endregion

                #region check IsRecently condition
                if (inputs.IsRecently == "DESC" || inputs.IsRecently == "desc")
                {
                    episodesRelatedForRecentlyOrdered = episodesRelatedForRecently.OrderByDescending(o => o.EpisodeViews).ToList();
                }

                if (inputs.IsRecently == "ASC" || inputs.IsRecently == "asc")
                {
                    episodesRelatedForRecentlyOrdered = episodesRelatedForRecently.OrderBy(o => o.EpisodeViews).ToList();
                }
                #endregion

                #region Create DTO To Retrieve Data
                RetrieveData<EpisodesRelatedForRecentlyModel> _RetrieveData = new RetrieveData<EpisodesRelatedForRecentlyModel>();
                _RetrieveData.DataList = episodesRelatedForRecentlyOrdered;
                _RetrieveData.Url = helper.LivePathImages;
                #endregion


                return _RetrieveData;
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Get Episodes Related ForRecently
        [ApiAuthentication]
        [HttpGet]
        [Route("episodesrelatedforrecently")]
        public async Task<ActionResult<RetrieveData<EpisodesRelatedForRecentlyModel>>> episodesrelatedforrecently([FromQuery] EpisodesRelatedForRecentlyInputs inputs)
        {
            try
            {
                #region get Categories , Programs , Seasons , ProgramTypes , Episodes
                var Categories = await unitOfWork.category.GetObjects(); Categories.ToList();
                var Programs = await unitOfWork.Program.GetObjects(); Programs.ToList();
                var ProgramTypes = await unitOfWork.ProgramType.GetObjects(); ProgramTypes.ToList();
                var Episodes = await unitOfWork.Episode.GetObjects(); Episodes.ToList();
                var Keyword = await unitOfWork.Keyword.GetObjects(); Keyword.ToList();
                var EpisodeKeyword = await unitOfWork.EpisodesKeyword.GetObjects(); EpisodeKeyword.ToList();
                var Seasons = await unitOfWork.Season.GetObjects(); Seasons.ToList();
                #endregion

                #region get all keyword ID that have keyword related with Episode ID 
                List<EpisodeKeywordModel> episodeKeywordsList = new List<EpisodeKeywordModel>();
                var EpisodeKeywords = (from episode in Episodes
                                       join episodekeyword in EpisodeKeyword
                                       on episode.EpisodeId equals episodekeyword.EpisodeId
                                       join _keyword in Keyword
                                       on episodekeyword.KeywordId equals _keyword.KeywordId
                                       where episode.EpisodeId == inputs.EpisodeID
                                       select new
                                       {
                                           _keyword.KeywordId
                                       }).ToList();
                foreach (var item in EpisodeKeywords)
                {
                    EpisodeKeywordModel episodeKeyword = new EpisodeKeywordModel()
                    {
                        KeywordId = item.KeywordId
                    };
                    episodeKeywordsList.Add(episodeKeyword);
                }
                #endregion

                #region declare list to fetch output 
                List<EpisodesRelatedForRecentlyModel> episodesRelatedForRecently = new List<EpisodesRelatedForRecentlyModel>();
                List<EpisodesRelatedForRecentlyModel> episodesRelatedForRecentlyOrdered = new List<EpisodesRelatedForRecentlyModel>();
                #endregion

                #region Apply Query in db 
                var Result = from category in Categories
                             join program in Programs
                             on category.CategoryId equals program.CategoryId
                             join programType in ProgramTypes
                             on program.ProgramTypeId equals programType.ProgramTypeId
                             join season in Seasons
                             on program.ProgramId equals season.ProgramId
                             join episode in Episodes
                             on season.SessionId equals episode.SessionId
                             join episodeKeyword in EpisodeKeyword
                             on episode.EpisodeId equals episodeKeyword.EpisodeId
                             join keyword in Keyword
                             on episodeKeyword.KeywordId equals keyword.KeywordId
                             where (category.CategoryId == inputs.CategoryID || inputs.CategoryID is null) &&
                                   (programType.ProgramTypeId == inputs.ProgramTypeID || inputs.ProgramTypeID is null) &&
                                   (program.ProgramId == inputs.ProgramID || inputs.ProgramID is null) &&
                                   (episode.EpisodePublishDate >= inputs.DateFrom || inputs.DateFrom is null) &&
                                   (episode.EpisodePublishDate <= inputs.DateTo || inputs.DateTo is null) &&
                                   (episodeKeywordsList.Exists(a => a.KeywordId == episodeKeyword.KeywordId)) &&
                                   (episode.EpisodeId != inputs.EpisodeID)
                             select new
                             {
                                 season.SessionId,
                                 episode.EpisodeId,
                                 season.SeasonIndex,
                                 season.SessionTitle,
                                 episode.EpisodeTitle,
                                 episode.EpisodeViews,
                                 episode.EpisodePublishDate,
                                 episode.EpisodeIamgePath,
                                 episode.YoutubeUrl,
                                 program.ProgramId,
                                 program.ProgramName,
                                 program.ProgramImg,
                                 category.CategoryId,
                                 category.CategoryTitle,
                                 programType.ProgramTypeId,
                                 programType.ProgramTypeTitle
                             };
                #endregion

                #region Fill output List from returned list from db
                foreach (var item in Result)
                {
                    EpisodesRelatedForRecentlyModel model = new EpisodesRelatedForRecentlyModel()
                    {
                        SessionId = item.SessionId,
                        CategoryId = item.CategoryId,
                        CategoryTitle = item.CategoryTitle,
                        EpisodeId = item.EpisodeId,
                        EpisodePublishDate = item.EpisodePublishDate,
                        EpisodeTitle = item.EpisodeTitle,
                        EpisodeViews = item.EpisodeViews,
                        ProgramId = item.ProgramId,
                        ProgramImg = item.ProgramImg,
                        ProgramName = item.ProgramName,
                        ProgramTypeId = item.ProgramTypeId,
                        ProgramTypeTitle = item.ProgramTypeTitle,
                        EpisodeImg = item.EpisodeIamgePath,
                        EpisodeUrl = item.YoutubeUrl , 
                        SeasonIndex = item.SeasonIndex,
                        SeasonTitle = item.SessionTitle

                    };
                    episodesRelatedForRecently.Add(model);
                }
                #endregion

                #region check IsRecently condition
                if (inputs.IsRecently == "no")
                {
                    episodesRelatedForRecentlyOrdered = episodesRelatedForRecently.OrderBy(o => o.EpisodePublishDate).ToList();
                }
                else if (inputs.IsRecently == "yes")
                {
                    episodesRelatedForRecentlyOrdered = episodesRelatedForRecently.OrderByDescending(o => o.EpisodePublishDate).ToList();
                }
                else
                {
                    return BadRequest("please choose Order");
                }
                #endregion

                #region Create DTO To Retrieve Data
                RetrieveData<EpisodesRelatedForRecentlyModel> _RetrieveData = new RetrieveData<EpisodesRelatedForRecentlyModel>();
                _RetrieveData.DataList = episodesRelatedForRecentlyOrdered;
                _RetrieveData.Url = helper.LivePathImages;
                #endregion

                return _RetrieveData;
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Episodes Trending For Days
        /// <summary>
        /// this function get Popular Episodes Over The Past Specific Days    
        /// </summary>
        /// <param name="CategoryID"></param>
        /// <param name="ProgramTypeID"></param>
        /// <param name="ProgramID"></param>
        /// <param name="IsRecently"></param>
        /// <returns>
        ///  return list of Model that contains
        ///  EpisodeId,EpisodeTitle,EpisodeViews,EpisodePublishDate,ProgramId
        ///  ProgramName,ProgramImg,CategoryId,CategoryTitle,ProgramTypeId,ProgramTypeTitle
        /// </returns>

        [ApiAuthentication]
        [HttpGet]
        [Route("episodestrending")]
        public async Task<ActionResult<RetrieveData<EpisodesRelatedForRecentlyModel>>> episodestrending([FromQuery] EpisodesTrendingModel model)
        {
            try
            {
                EpisodesFilterForRecentlyInputs episodesFilterForRecentlyInputs = new EpisodesFilterForRecentlyInputs()
                {
                    InterviewerID = model.InterviewerID,
                    CategoryID = model.CategoryID,
                    ProgramID = model.ProgramID,
                    ProgramTypeID = model.ProgramTypeID,
                    DateFrom = DateTime.Now.AddDays(trendingDuration.TrendingDays),
                    DateTo = DateTime.Now,
                    IsRecently = model.IsRecently
                };
                return await episodesfilterforviews(episodesFilterForRecentlyInputs);
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Episodes Most Viewes ( Episodes Trending For Months )  IsRecently => DESC
        /// <summary>
        /// this function get Popular Episodes Over The Past Specific Months
        /// </summary>
        /// <param name="CategoryID"></param>
        /// <param name="ProgramTypeID"></param>
        /// <param name="ProgramID"></param>
        /// <returns>
        ///  return list of Model that contains
        ///  EpisodeId,EpisodeTitle,EpisodeViews,EpisodePublishDate,ProgramId
        ///  ProgramName,ProgramImg,CategoryId,CategoryTitle,ProgramTypeId,ProgramTypeTitle
        /// </returns>

        [ApiAuthentication]
        [HttpGet]
        [Route("EpisodesMostViewes")]
        public async Task<ActionResult<RetrieveData<EpisodesRelatedForRecentlyModel>>> EpisodesMostViewes([FromQuery] EpisodesMostViewesModel model)
        {
            try
            {
                EpisodesFilterForRecentlyInputs episodesFilterForRecentlyInputs = new EpisodesFilterForRecentlyInputs()
                {
                    InterviewerID = model.InterviewerID,
                    CategoryID = model.CategoryID,
                    ProgramID = model.ProgramID,
                    ProgramTypeID = model.ProgramTypeID,
                    DateFrom = DateTime.Now.AddMonths(trendingDuration.MostViewedMonth),
                    DateTo = DateTime.Now,
                    IsRecently = "DESC" , 
                    
                };
                return await episodesfilterforviews(episodesFilterForRecentlyInputs);
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Episodes Show More ( Episodes Trending For Months ) IsRecently => ASC
        /// <summary>
        /// this function get Popular Episodes Over The Past Specific Months
        /// </summary>
        /// <param name="CategoryID"></param>
        /// <param name="ProgramTypeID"></param>
        /// <param name="ProgramID"></param>
        /// <returns>
        ///  return list of Model that contains
        ///  EpisodeId,EpisodeTitle,EpisodeViews,EpisodePublishDate,ProgramId
        ///  ProgramName,ProgramImg,CategoryId,CategoryTitle,ProgramTypeId,ProgramTypeTitle
        /// </returns>

        [ApiAuthentication]
        [HttpGet]
        [Route("episodesshowmore")]
        public async Task<ActionResult<RetrieveData<EpisodesRelatedForRecentlyModel>>> episodesshowmore([FromQuery] EpisodesMostViewesModel model)
        {
            try
            {
                EpisodesFilterForRecentlyInputs episodesFilterForRecentlyInputs = new EpisodesFilterForRecentlyInputs()
                {
                    InterviewerID = model.InterviewerID,
                    CategoryID = model.CategoryID,
                    ProgramID = model.ProgramID,
                    ProgramTypeID = model.ProgramTypeID,
                    DateFrom = DateTime.Now.AddMonths(trendingDuration.ShowMoreMonth),
                    DateTo = DateTime.Now,
                    IsRecently = "ASC"
                };
                return await episodesfilterforviews(episodesFilterForRecentlyInputs);
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Episodes Recently
        /// <summary>
        /// this function get All Episodes Ordered Descending    
        /// </summary>
        /// <returns>
        ///  return list of Model that contains
        ///  EpisodeId,EpisodeTitle,EpisodeViews,EpisodePublishDate,ProgramId
        ///  ProgramName,ProgramImg,CategoryId,CategoryTitle,ProgramTypeId,ProgramTypeTitle
        /// </returns>

        [ApiAuthentication]
        [HttpGet]
        [Route("episodesrecently")]
        public async Task<ActionResult<RetrieveData<EpisodesRelatedForRecentlyModel>>> episodesrecently([FromQuery] EpisodesRecently episodesRecently)
        {
            try
            {
                EpisodesFilterForRecentlyInputs _EpisodesFilterForRecentlyInputs = new EpisodesFilterForRecentlyInputs()
                {
                    InterviewerID = episodesRecently.InterviewerID,
                    CategoryID = episodesRecently.CategoryID,
                    DateFrom = episodesRecently.DateFrom,
                    ProgramTypeID = episodesRecently.ProgramTypeID,
                    ProgramID = episodesRecently.ProgramID,
                    IsRecently = "DESC",
                    DateTo = DateTime.Now
                };

                return await episodesfilterforrecently(_EpisodesFilterForRecentlyInputs);
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion
  

        #region Detail API
        /// <summary>
        ///     
        /// </summary>
        /// <returns>
        ///  return list of Model that contains
        /// 
        /// </returns>

        [ApiAuthentication]
        [HttpGet]
        [Route("detailapi")]
        public async Task<ActionResult<RetrieveData<DetailAPIModel>>> detailapi(int EPISODEID)
        {
            try
            {

                #region Check If Episode Exist Or Not --
                var Episode = await unitOfWork.Episode.FindObjectAsync(EPISODEID);
                if (Episode == null)
                    return NoContent();
                #endregion

                #region get Categories , Programs , ProgramTypes , Episodes
                var Categories = await unitOfWork.category.GetObjects(); Categories.ToList();
                var Programs = await unitOfWork.Program.GetObjects(); Programs.ToList();
                var ProgramTypes = await unitOfWork.ProgramType.GetObjects(); ProgramTypes.ToList();
                var Episodes = await unitOfWork.Episode.GetObjects(); Episodes.ToList();
                var Seasons = await unitOfWork.Season.GetObjects(); Seasons.ToList();
                var Interviewers = await unitOfWork.Interviewer.GetObjects(); Interviewers.ToList();
                #endregion

                #region declare list to fetch output 
                List<DetailAPIModel> DetailAPIModelList = new List<DetailAPIModel>();
                #endregion

                #region Apply Query in db 
                var Result = (from category in Categories
                              join program in Programs
                              on category.CategoryId equals program.CategoryId
                              join programType in ProgramTypes
                              on program.ProgramTypeId equals programType.ProgramTypeId
                              join season in Seasons
                              on program.ProgramId equals season.ProgramId
                              join episode in Episodes
                              on season.SessionId equals episode.SessionId
                              join interviewer in Interviewers
                              on program.InterviewerId equals interviewer.InterviewerId
                              where (episode.EpisodeId == EPISODEID) && (episode.EpisodeVisible == true)
                              select new
                              {
                                  program.ProgramId,
                                  program.ProgramName,
                                  program.ProgramImg,
                                  program.ProgramDescription,
                                  program.ProgramStartDate,
                                  category.CategoryId,
                                  category.CategoryTitle,
                                  programType.ProgramTypeId,
                                  programType.ProgramTypeTitle,
                                  interviewer.InterviewerId,
                                  interviewer.InterviewerName,
                                  interviewer.InterviewerPicture,
                                  season.SessionId,
                                  season.SessionTitle,
                                  episode.EpisodeId,
                                  episode.EpisodeTitle,
                                  episode.EpisodeDescription,
                                  episode.EpisodeIamgePath,
                                  episode.EpisodePublishDate,
                                  episode.EpisodeViews,
                                  episode.YoutubeUrl,
                              }).Distinct();
                #endregion

                #region Number Of Episodes In Season that episode id exist on it
                var episodeObj = await unitOfWork.Episode.FindObjectAsync(EPISODEID);

                var SeasonCount = await unitOfWork.Episode.GetObjects(x => x.SessionId == episodeObj.SessionId);
                int EPISODECOUNT = SeasonCount.Count();
                #endregion

                #region Fill list to return as a result 
                foreach (var item in Result)
                {
                    DetailAPIModel detailAPI = new DetailAPIModel()
                    {
                        CategoryId = item.CategoryId,
                        CategoryTitle = item.CategoryTitle,
                        EpisodeDescription = item.EpisodeDescription,
                        EpisodeIamgePath = item.EpisodeIamgePath,
                        EpisodeId = item.EpisodeId,
                        EpisodePublishDate = item.EpisodePublishDate,
                        ProgramStartDate = item.ProgramStartDate,
                        EpisodeTitle = item.EpisodeTitle,
                        EpisodeViews = item.EpisodeViews,
                        InterviewerId = item.InterviewerId,
                        InterviewerName = item.InterviewerName,
                        InterviewerPicture = item.InterviewerPicture,
                        ProgramDescription = item.ProgramDescription,
                        ProgramId = item.ProgramId,
                        ProgramImg = item.ProgramImg,
                        ProgramName = item.ProgramName,
                        ProgramTypeId = item.ProgramTypeId,
                        ProgramTypeTitle = item.ProgramTypeTitle,
                        SessionId = item.SessionId,
                        SessionTitle = item.SessionTitle,
                        EpisodesCount = EPISODECOUNT,
                        YoutubeUrl = item.YoutubeUrl,
                    };
                    DetailAPIModelList.Add(detailAPI);
                }
                #endregion

                #region Increase Episode Views 
                episodeObj.EpisodeViews += 1;
                unitOfWork.Episode.Update(episodeObj);
                #endregion

                #region Increase Season Views
                var Season = await unitOfWork.Season.FindObjectAsync((int)episodeObj.SessionId);
                Season.SeasonViews += 1;
                 unitOfWork.Season.Update(Season);
                #endregion

                #region Increase program Views
                var Program = await unitOfWork.Program.FindObjectAsync(Season.ProgramId);
                Program.ProgramViews += 1;
                unitOfWork.Program.Update(Program);
                #endregion

                #region Increase Category Views
                var Category = await unitOfWork.category.FindObjectAsync(Program.CategoryId);
                Category.CategoryViews += 1;
                unitOfWork.category.Update(Category);
                #endregion

                #region update operation
                await unitOfWork.Complete();
                #endregion

                #region Create DTO To Retrieve Data
                RetrieveData<DetailAPIModel> _RetrieveData = new RetrieveData<DetailAPIModel>();
                _RetrieveData.DataList = DetailAPIModelList;
                _RetrieveData.Url = helper.LivePathImages;
                #endregion

                return Ok(_RetrieveData);
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Season Related
        /// <summary>
        /// get all seasons exist in program that specified EPISODEID  exist in it    
        /// </summary>
        /// <returns>
        ///  return list of Model that contains
        /// 
        /// </returns>

        [ApiAuthentication]
        [HttpGet]
        [Route("seasonrelated")]
        public async Task<ActionResult<List<SeasonTitleModel>>> seasonrelated(int EPISODEID)
        {
            try
            {
                #region get Categories , Programs , ProgramTypes , Episodes
                var Programs = await unitOfWork.Program.GetObjects(); Programs.ToList();
                var Episodes = await unitOfWork.Episode.GetObjects(); Episodes.ToList();
                var Seasons = await unitOfWork.Season.GetObjects(); Seasons.ToList();
                #endregion

                #region declare list to fetch output 
                List<SeasonTitleModel> SeasonTitlelList = new List<SeasonTitleModel>();
                #endregion

                #region
                var PROGRAMID = (from program in Programs
                                 join season in Seasons
                                 on program.ProgramId equals season.ProgramId
                                 join episode in Episodes
                                 on season.SessionId equals episode.SessionId
                                 where
                                 episode.EpisodeId == EPISODEID && episode.EpisodeVisible == true
                                 select program.ProgramId).FirstOrDefault();
                #endregion

                #region Apply Query in db 
                var Result1 = (from program in Programs
                               join season in Seasons
                               on program.ProgramId equals season.ProgramId
                               where program.ProgramId == PROGRAMID
                               select new
                               {
                                   season.SessionId,
                                   season.SessionTitle
                               }).ToList();
                #endregion

                #region
                foreach (var item in Result1)
                {
                    SeasonTitleModel _seasontitle = new SeasonTitleModel()
                    {
                        seasontitle = item.SessionTitle,
                        seasonid = item.SessionId
                    };
                    SeasonTitlelList.Add(_seasontitle);
                }
                #endregion
                return Ok(SeasonTitlelList);
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Get All Categories

        #region Get All Categories with API Key
        /// <summary>
        /// get all categories with API Key
        /// </summary>
        /// <returns>
        /// List Of Category that contain CategoryId,CategoryTitle,CreationDate,CategoryDescription,CategoryVisible,CategoryOrder,CategoryViews
        /// and CategoryImg concatenating with LivePathImages
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("getallcategorieswithapikey")]
        public async Task<ActionResult<RetrieveData<Category>>> getallcategorieswithapikey()
        {
            try
            {
                RetrieveData<Category> collection = new RetrieveData<Category>();
                var ResultCategories = await unitOfWork.category.GetOrderedObjects(x => x.CategoryOrder);
                collection.Url = helper.LivePathImages;
                collection.DataList = ResultCategories.ToList();

                #region Update Category Views
                foreach (var item in ResultCategories)
                {
                    item.CategoryViews += 1;
                }
                await unitOfWork.Complete();
                #endregion

                return Ok(collection);
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
            }
        }
        #endregion

        #region Get All Categories => Dashboard API 
        /// <summary>
        /// get all categories with Bearer Toacken
        /// </summary>
        /// <returns>
        /// List Of Category that contain CategoryId,CategoryTitle,CreationDate,CategoryDescription,CategoryVisible,CategoryOrder,CategoryViews
        /// and CategoryImg concatenating with LivePathImages
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("getallcategories")]
        public async Task<ActionResult<RetrieveData<CategoryModel>>> getallcategories()
        {
            try
            {
                RetrieveData<CategoryModel> collection = new RetrieveData<CategoryModel>();
                List<CategoryModel> categories = new List<CategoryModel>();
                var ResultCategories = await unitOfWork.category.GetOrderedObjects(x => x.CategoryOrder);
                foreach (var item in ResultCategories)
                {
                    #region Update Category Views
                    item.CategoryViews += 1;
                    #endregion

                    var ProgramsCount = await unitOfWork.Program.GetObjects(x => x.CategoryId == item.CategoryId); ProgramsCount.ToList();
                    CategoryModel category = new CategoryModel()
                    {
                        CategoryId = item.CategoryId,
                        CategoryDescription = item.CategoryDescription,
                        CategoryImg = item.CategoryImg,
                        CategoryOrder = item.CategoryOrder,
                        CategoryTitle = item.CategoryTitle,
                        CategoryViews = item.CategoryViews,
                        CategoryVisible = item.CategoryVisible,
                        CreationDate = item.CreationDate,
                        ProgramsCount = ProgramsCount.Count()
                    };
                    categories.Add(category);
                };

                collection.Url = helper.LivePathImages;
                collection.DataList = categories;

                await unitOfWork.Complete();

                return Ok(collection);
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
            }
        }
        #endregion
         
        #endregion

    }
}
