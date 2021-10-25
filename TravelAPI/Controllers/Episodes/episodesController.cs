using BalarinaAPI.Authentication;
using BalarinaAPI.Core.Model;
using BalarinaAPI.Core.ViewModel;
using BalarinaAPI.Core.ViewModel.DetailsAPI;
using BalarinaAPI.Core.ViewModel.Episode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<List<EpisodeModelOutput>>> getallepisodes()
        {
            try
            {
                List<EpisodeModelOutput> episodes = new List<EpisodeModelOutput>();
                //Get All Episodes 
                var ResultEpisodes = await unitOfWork.Episode.GetObjects();
                ResultEpisodes.ToList();
                foreach (var item in ResultEpisodes)
                {
                    EpisodeModelOutput episode = new EpisodeModelOutput()
                    {
                        seasonID = item.EpisodeId,
                        CreationDate = item.CreationDate,
                        DislikeRate = (int)item.DislikeRate,
                        EpisodeDescription = item.EpisodeDescription,
                        EpisodeId = item.EpisodeId,
                        EpisodePublishDate = item.EpisodePublishDate,
                        EpisodeTitle = item.EpisodeTitle,
                        EpisodeViews = item.EpisodeViews,
                        EpisodeVisible = item.EpisodeVisible,
                        LikeRate = (int)item.LikeRate,
                        YoutubeUrl = item.YoutubeUrl,
                        EpisodeIamgePath = helper.LivePathImages + item.EpisodeIamgePath
                    };
                    episodes.Add(episode);
                }
                return episodes;

            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
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
        public async Task<ActionResult<List<EpisodeModelOutput>>> getallepisodesapikey()
        {
            try
            {
                List<EpisodeModelOutput> episodes = new List<EpisodeModelOutput>();
                //Get All Episodes 
                var ResultEpisodes = await unitOfWork.Episode.GetObjects();
                ResultEpisodes.ToList();
                foreach (var item in ResultEpisodes)
                {
                    EpisodeModelOutput episode = new EpisodeModelOutput()
                    {
                        seasonID = item.EpisodeId,
                        CreationDate = item.CreationDate,
                        DislikeRate = (int)item.DislikeRate,
                        EpisodeDescription = item.EpisodeDescription,
                        EpisodeId = item.EpisodeId,
                        EpisodePublishDate = item.EpisodePublishDate,
                        EpisodeTitle = item.EpisodeTitle,
                        EpisodeViews = item.EpisodeViews,
                        EpisodeVisible = item.EpisodeVisible,
                        LikeRate = (int)item.LikeRate,
                        YoutubeUrl = item.YoutubeUrl,
                        EpisodeIamgePath = helper.LivePathImages + item.EpisodeIamgePath
                    };
                    episodes.Add(episode);
                }
                return episodes;

            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
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
        [Route("getallepisodesbyseasonid")]
        public async Task<ActionResult<List<Episode>>> getallepisodesbyseasonid(int ID)
        {
            try
            {
                //Get All Episodes 
                var ResultEpisodes = await unitOfWork.Episode.GetObjects(x=>x.SessionId == ID);
                ResultEpisodes.ToList();

                foreach (var item in ResultEpisodes)
                {
                    item.EpisodeIamgePath = helper.LivePathImages + item.EpisodeIamgePath;
                }
                return ResultEpisodes.ToList();
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
        [Authorize]
        [HttpPost]
        [Route("createepisode")]
        public async Task<ActionResult<EpisodeModelInput>> createepisodeAsync([FromQuery] EpisodeModelInput model)
        {
            try
            {
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

                var SeasonId = unitOfWork.Season.FindObjectAsync(model.SeasonId);
                if (SeasonId == null)
                    return BadRequest("Season ID not found ");
                #endregion

                #region Fill Episode object with values to insert
                Episode _episode = new Episode()
                {
                    CreationDate = DateTime.Now,
                    DislikeRate = (int)model.LikeRate,
                    EpisodeDescription = model.EpisodeDescription,
                    EpisodePublishDate = model.EpisodePublishDate,
                    EpisodeTitle = model.EpisodeTitle,
                    LikeRate = (int)model.LikeRate,
                    EpisodeViews = (int)model.EpisodeViews,
                    EpisodeVisible = model.EpisodeVisible,
                    SessionId = model.SeasonId,
                    YoutubeUrl = model.YoutubeUrl,
                    EpisodeIamgePath = UploadImage(model.EpisodeIamge)
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

                return Ok("Episode Created Successfully ");
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }
        #endregion

        #region Edit Episode
        [Authorize]
        [HttpPut]
        [Route("putepisode")]
        public async Task<ActionResult<Episode>> putepisodeAsync([FromQuery] EpisodeToUpdate model)
        {
            try
            {
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

                if (model.EpisodePublishDate == null)
                    model.EpisodePublishDate = _EpisodeObj.EpisodePublishDate;

                if (model.SeasonId == null)
                    model.SeasonId = _EpisodeObj.SessionId;

                #endregion

                #region check if image updated or not 
                if (model.EpisodeImagePath == null)
                {
                    if (model.EpisodeImage != null)
                        model.EpisodeImagePath = UploadImage(model.EpisodeImage);
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
                    EpisodePublishDate = (DateTime)model.EpisodePublishDate,
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

                return Ok("Episode Updated Successfully ");
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Delete Episode

        [Authorize]
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

                return Ok("Episode Deleted Successfully ");
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
        public async Task<ActionResult<List<EpisodesRelatedForRecentlyModel>>> episodesfilterforrecently([FromQuery] EpisodesFilterForRecentlyInputs inputs)
        {
            try
            {
                #region get Categories , Programs , ProgramTypes , Episodes ==> From Database
                var Categories = await unitOfWork.category.GetObjects(); Categories.ToList();
                var Programs = await unitOfWork.Program.GetObjects(); Programs.ToList();
                var ProgramTypes = await unitOfWork.ProgramType.GetObjects(); ProgramTypes.ToList();
                var Episodes = await unitOfWork.Episode.GetObjects(); Episodes.ToList();
                var Seasons = await unitOfWork.Season.GetObjects(); Seasons.ToList();
                #endregion

                #region declare list to fetch output 
                List<EpisodesRelatedForRecentlyModel> episodesRelatedForRecently = new List<EpisodesRelatedForRecentlyModel>();
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
                              where (category.CategoryId == inputs.CategoryID || inputs.CategoryID is null) &&
                                    (programType.ProgramTypeId == inputs.ProgramTypeID || inputs.ProgramTypeID is null) &&
                                    (program.ProgramId == inputs.ProgramID || inputs.ProgramID is null) &&
                                    (episode.EpisodePublishDate >= inputs.DateFrom || inputs.DateFrom is null) &&
                                    (episode.EpisodePublishDate <= inputs.DateTo || inputs.DateTo is null)
                              select new
                              {
                                  episode.EpisodeId,
                                  episode.EpisodeTitle,
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
                                  episode.YoutubeUrl
                              }).Distinct();
                #endregion

                #region Fill output List from returned list from db
                foreach (var item in Result)
                {
                    EpisodesRelatedForRecentlyModel model = new EpisodesRelatedForRecentlyModel()
                    {
                        CategoryId = item.CategoryId,
                        CategoryTitle = item.CategoryTitle,
                        EpisodeId = item.EpisodeId,
                        EpisodePublishDate = item.EpisodePublishDate,
                        EpisodeTitle = item.EpisodeTitle,
                        EpisodeViews = item.EpisodeViews,
                        ProgramId = item.ProgramId,
                        ProgramImg = helper.LivePathImages + item.ProgramImg,
                        ProgramName = item.ProgramName,
                        ProgramTypeId = item.ProgramTypeId,
                        ProgramTypeTitle = item.ProgramTypeTitle,
                        EpisodeImg = helper.LivePathImages + item.EpisodeIamgePath,
                        EpisodeUrl = item.YoutubeUrl
                    };
                    episodesRelatedForRecently.Add(model);
                }
                #endregion

                #region check IsRecently condition

                if (inputs.IsRecently == "desc" || inputs.IsRecently == "DESC")
                {
                    episodesRelatedForRecently.OrderBy(o => o.EpisodePublishDate);
                }
                else
                {
                    episodesRelatedForRecently.OrderByDescending(o => o.EpisodePublishDate);
                }


                #endregion

                return episodesRelatedForRecently;
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
        public async Task<ActionResult<List<EpisodesRelatedForRecentlyModel>>> episodesfilterforviews([FromQuery] EpisodesFilterForRecentlyInputs inputs)
        {
            try
            {
                #region get Categories , Programs , ProgramTypes , Episodes
                var Categories = await unitOfWork.category.GetObjects(); Categories.ToList();
                var Programs = await unitOfWork.Program.GetObjects(); Programs.ToList();
                var ProgramTypes = await unitOfWork.ProgramType.GetObjects(); ProgramTypes.ToList();
                var Episodes = await unitOfWork.Episode.GetObjects(); Episodes.ToList();
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
                              join season in Seasons
                               on program.ProgramId equals season.ProgramId
                              join episode in Episodes
                              on season.SessionId equals episode.SessionId
                              where (category.CategoryId == inputs.CategoryID || inputs.CategoryID is null) &&
                                   (programType.ProgramTypeId == inputs.ProgramTypeID || inputs.ProgramTypeID is null) &&
                                   (program.ProgramId == inputs.ProgramID || inputs.ProgramID is null)
                              &&
                              (episode.EpisodePublishDate >= inputs.DateFrom || inputs.DateFrom is null) &&
                              (episode.EpisodePublishDate <= inputs.DateTo || inputs.DateTo is null)
                              select new
                              {
                                  episode.EpisodeId,
                                  episode.EpisodeTitle,
                                  episode.EpisodeViews,
                                  episode.EpisodeIamgePath,
                                  episode.YoutubeUrl,
                                  episode.EpisodePublishDate,
                                  program.ProgramId,
                                  program.ProgramName,
                                  program.ProgramImg,
                                  category.CategoryId,
                                  category.CategoryTitle,
                                  programType.ProgramTypeId,
                                  programType.ProgramTypeTitle

                              }).Distinct();
                #endregion

                #region Fill output List from returned list from db
                foreach (var item in Result)
                {
                    EpisodesRelatedForRecentlyModel model = new EpisodesRelatedForRecentlyModel()
                    {
                        CategoryId = item.CategoryId,
                        CategoryTitle = item.CategoryTitle,
                        EpisodeId = item.EpisodeId,
                        EpisodePublishDate = item.EpisodePublishDate,
                        EpisodeTitle = item.EpisodeTitle,
                        EpisodeViews = item.EpisodeViews,
                        ProgramId = item.ProgramId,
                        ProgramImg = helper.LivePathImages + item.ProgramImg,
                        ProgramName = item.ProgramName,
                        ProgramTypeId = item.ProgramTypeId,
                        ProgramTypeTitle = item.ProgramTypeTitle,
                        EpisodeUrl = item.YoutubeUrl,
                        EpisodeImg = helper.LivePathImages + item.EpisodeIamgePath
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


                return episodesRelatedForRecentlyOrdered;
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
        public async Task<ActionResult<List<EpisodesRelatedForRecentlyModel>>> episodesrelatedforrecently([FromQuery] EpisodesRelatedForRecentlyInputs inputs)
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
                        EpisodeID = item.KeywordId
                    };
                    episodeKeywordsList.Add(episodeKeyword);
                }
                #endregion

                #region declare list to fetch output 
                List<EpisodesRelatedForRecentlyModel> episodesRelatedForRecently = new List<EpisodesRelatedForRecentlyModel>();
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
                                   (episodeKeywordsList.Exists(a => a.EpisodeID == episodeKeyword.KeywordId)) &&
                                   (episode.EpisodeId != inputs.EpisodeID)
                             select new
                             {
                                 episode.EpisodeId,
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
                        CategoryId = item.CategoryId,
                        CategoryTitle = item.CategoryTitle,
                        EpisodeId = item.EpisodeId,
                        EpisodePublishDate = item.EpisodePublishDate,
                        EpisodeTitle = item.EpisodeTitle,
                        EpisodeViews = item.EpisodeViews,
                        ProgramId = item.ProgramId,
                        ProgramImg = helper.LivePathImages + item.ProgramImg,
                        ProgramName = item.ProgramName,
                        ProgramTypeId = item.ProgramTypeId,
                        ProgramTypeTitle = item.ProgramTypeTitle,
                        EpisodeImg = helper.LivePathImages + item.EpisodeIamgePath,
                        EpisodeUrl = item.YoutubeUrl

                    };
                    episodesRelatedForRecently.Add(model);
                }
                #endregion

                #region check IsRecently condition
                if (inputs.IsRecently == "no")
                {
                    episodesRelatedForRecently.OrderBy(o => o.EpisodePublishDate);
                }
                else if (inputs.IsRecently == "yes")
                {
                    episodesRelatedForRecently.OrderByDescending(o => o.EpisodePublishDate);
                }
                else
                {
                    return BadRequest("please choose Order");
                }
                #endregion

                return episodesRelatedForRecently;
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Function take image and return image name that store in db 
        /// <summary>
        /// generate unique name of image and save image in specified path 
        /// </summary>
        /// <param name="categoryImage"></param>
        /// <returns>
        /// unique name of iamge concatenating with extension of image 
        /// </returns>
        private string UploadImage(IFormFile categoryImage)
        {
            try
            {
                var pathToSave = helper.PathImage;
                if (categoryImage.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(categoryImage.FileName);
                    var fullPath = Path.Combine(pathToSave, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        categoryImage.CopyTo(stream);
                    }
                    return fileName;
                }
                else
                {
                    return "error";
                }
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return "error";
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
        public async Task<ActionResult<List<EpisodesRelatedForRecentlyModel>>> episodestrending([FromQuery] EpisodesTrendingModel model)
        {
            try
            {
                EpisodesFilterForRecentlyInputs episodesFilterForRecentlyInputs = new EpisodesFilterForRecentlyInputs()
                {
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
        public async Task<ActionResult<List<EpisodesRelatedForRecentlyModel>>> EpisodesMostViewes([FromQuery] EpisodesMostViewesModel model)
        {
            try
            {
                EpisodesFilterForRecentlyInputs episodesFilterForRecentlyInputs = new EpisodesFilterForRecentlyInputs()
                {
                    CategoryID = model.CategoryID,
                    ProgramID = model.ProgramID,
                    ProgramTypeID = model.ProgramTypeID,
                    DateFrom = DateTime.Now.AddMonths(trendingDuration.MostViewedMonth),
                    DateTo = DateTime.Now,
                    IsRecently = "DESC"
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
        public async Task<ActionResult<List<EpisodesRelatedForRecentlyModel>>> episodesshowmore([FromQuery] EpisodesMostViewesModel model)
        {
            try
            {
                EpisodesFilterForRecentlyInputs episodesFilterForRecentlyInputs = new EpisodesFilterForRecentlyInputs()
                {
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
        public async Task<ActionResult<List<EpisodesRelatedForRecentlyModel>>> episodesrecently()
        {
            try
            {
                EpisodesFilterForRecentlyInputs _EpisodesFilterForRecentlyInputs = new EpisodesFilterForRecentlyInputs()
                {
                    CategoryID = null,
                    DateFrom = null,
                    ProgramTypeID = null,
                    ProgramID = null,
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
        public async Task<ActionResult<List<DetailAPIModel>>> detailapiAsync(int EPISODEID)
        {
            try
            {
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
                                  episode.YoutubeUrl
                              }).Distinct();
                #endregion

                #region Season Count that episode id exist in it
                var episodeObj = await unitOfWork.Episode.FindObjectAsync( EPISODEID);

                var SeasonCount =await unitOfWork.Episode.GetObjects(x => x.SessionId == episodeObj.SessionId);
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
                        EpisodeIamgePath = helper.LivePathImages + item.EpisodeIamgePath,
                        EpisodeId = item.EpisodeId,
                        EpisodePublishDate = item.EpisodePublishDate,
                        ProgramStartDate = item.ProgramStartDate,
                        EpisodeTitle = item.EpisodeTitle,
                        EpisodeViews = item.EpisodeViews,
                        InterviewerId = item.InterviewerId,
                        InterviewerName = item.InterviewerName,
                        InterviewerPicture = helper.LivePathImages + item.InterviewerPicture,
                        ProgramDescription = item.ProgramDescription,
                        ProgramId = item.ProgramId,
                        ProgramImg = helper.LivePathImages + item.ProgramImg,
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

                return Ok(DetailAPIModelList);
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

    }
}
