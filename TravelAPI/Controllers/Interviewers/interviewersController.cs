using BalarinaAPI.Authentication;
using BalarinaAPI.Core.Model;
using BalarinaAPI.Core.ViewModel;
using BalarinaAPI.Core.ViewModel.Category;
using BalarinaAPI.Core.ViewModel.Episode;
using BalarinaAPI.Core.ViewModel.Interviewer;
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

namespace BalarinaAPI.Controllers.Interviewers
{
    [Route("api/[controller]")]
    [ApiController]
    public class interviewersController : ControllerBase
    {
        #region variables
        private readonly IUnitOfWork unitOfWork;
        private readonly Helper helper;
        private readonly TrendingDuration trendingDuration; 
        private readonly BalarinaDatabaseContext dbContext;

        #endregion

        #region Constructor
        public interviewersController(IUnitOfWork _unitOfWork, IOptions<Helper> _helper , IOptions<TrendingDuration> _trendingDuration, BalarinaDatabaseContext _dbContext)
        {
            unitOfWork = _unitOfWork;
            this.helper = _helper.Value;
            this.trendingDuration = _trendingDuration.Value;
            dbContext = _dbContext;
        }
        #endregion

        #region CRUD Operations For Interviewers

        #region Get All Interviewer 
        [ApiAuthentication]
        [HttpGet]
        [Route("getalliterviewers")]
        public async Task<ActionResult<RetrieveData<InterviewerModel>>> getalliterviewers()
        {
            try
            {
                RetrieveData<InterviewerModel> Collection = new RetrieveData<InterviewerModel>();

                //Get All Interviewer 
                var ResultInterviewers = await unitOfWork.Interviewer.GetObjects();
                foreach (var item in ResultInterviewers)
                {
                    string ConvertedDate = item.BirthDate.ToString();
                    string date = ConvertedDate.Substring(0, ConvertedDate.IndexOf(" "));

                    InterviewerModel model = new InterviewerModel()
                    {
                        InterviewerName = item.InterviewerName,
                        CreationDate = item.CreationDate,
                        FacebookUrl = item.FacebookUrl,
                        InstgramUrl = item.InstgramUrl,
                        InterviewerDescription = item.InterviewerDescription,
                        InterviewerId = item.InterviewerId,
                        InterviewerPicture = item.InterviewerPicture,
                        LinkedInUrl = item.LinkedInUrl,
                        TiktokUrl = item.TiktokUrl,
                        TwitterUrl = item.TwitterUrl,
                        WebsiteUrl = item.WebsiteUrl,
                        YoutubeUrl = item.YoutubeUrl,
                        Date = date
                    };
                    Collection.DataList.Add(model);
                }
                Collection.Url = helper.LivePathImages;
                return Collection;
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
            }
        }
        #endregion

        #region Get All Interviewer Name , ID 
        /// <summary>
        /// Get All Interviewer Name , ID 
        /// </summary>
        /// <returns>
        /// List of Object that Contains 
        /// InterviewerId , InterviewerName
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("GetInterviewer_ID_Name")]
        public async Task<ActionResult<List<ListOfNameID<Object_ID_Name>>>> GetInterviewer_ID_Name()
        {
            try
            {
                //check If Category ID If Exist
                var _InterviewerObject = await unitOfWork.Interviewer.GetObjects(); _InterviewerObject.ToList();
                if (_InterviewerObject == null)
                    return BadRequest("Interviewer list is empty ");
                //Get All Programs 

                List<ListOfNameID<Object_ID_Name>> Collection = new List<ListOfNameID<Object_ID_Name>>();
                foreach (var item in _InterviewerObject)
                {
                    ListOfNameID<Object_ID_Name> obj = new ListOfNameID<Object_ID_Name>() { ID = item.InterviewerId, Name = item.InterviewerName };
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

        #region findinterviewer
        [ApiAuthentication]
        [HttpGet]
        [Route("findinterviewer")]
        public async Task<ActionResult<RetrieveData<InterviewerProfile>>> findinterviewer(int ID)
        {
            try
            {
                //Get All Interviewer 
                var ResultInterviewer = await unitOfWork.Interviewer.FindObjectAsync(ID);

                if (ResultInterviewer == null)
                    return BadRequest("interviewer ID not found ");

                InterviewerProfile interviewerProfile = new InterviewerProfile();
                interviewerProfile.BirthDate = ResultInterviewer.BirthDate;
                interviewerProfile.FacebookUrl = ResultInterviewer.FacebookUrl;
                interviewerProfile.InstgramUrl = ResultInterviewer.InstgramUrl;
                interviewerProfile.InterviewerCover = ResultInterviewer.InterviewerCover;
                interviewerProfile.InterviewerDescription = ResultInterviewer.InterviewerDescription;
                interviewerProfile.InterviewerId = ResultInterviewer.InterviewerId;
                interviewerProfile.InterviewerName = ResultInterviewer.InterviewerName;
                interviewerProfile.InterviewerPicture = ResultInterviewer.InterviewerPicture;
                interviewerProfile.LinkedInUrl = ResultInterviewer.LinkedInUrl;
                interviewerProfile.TiktokUrl = ResultInterviewer.TiktokUrl;
                interviewerProfile.TwitterUrl = ResultInterviewer.TwitterUrl;
                interviewerProfile.WebsiteUrl = ResultInterviewer.WebsiteUrl;
                interviewerProfile.YoutubeUrl = ResultInterviewer.YoutubeUrl;
                interviewerProfile.CreationDate = ResultInterviewer.CreationDate;



                var _Programs = await unitOfWork.Program.GetObjects(x=>x.InterviewerId == ResultInterviewer.InterviewerId);
                var ProgramTypes = await unitOfWork.ProgramType.GetObjects();

                var ProgramTypeList = from _program in _Programs
                                      join type in ProgramTypes
                                      on _program.ProgramTypeId equals type.ProgramTypeId
                                      select new { type.ProgramTypeTitle };

                string TypeString = "";
                foreach (var item in ProgramTypeList)
                {
                    TypeString += item.ProgramTypeTitle;
                }

                interviewerProfile.ProgramTypesAsString = TypeString;
                
                RetrieveData<InterviewerProfile> Collection = new RetrieveData<InterviewerProfile>();
                Collection.Url = helper.LivePathImages;
                Collection.DataList.Add(interviewerProfile);

                return Collection;
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
            }
        }
        #endregion

        #region Get All Interviewer api key
        [ApiAuthentication]
        [HttpGet]
        [Route("getalliterviewersapikey")]
        public async Task<ActionResult<RetrieveData<Interviewer>>> getalliterviewerswithapikey()
        {
            try
            {
                //Get All Interviewer 
                var ResultInterviewers = await unitOfWork.Interviewer.GetObjects();
                RetrieveData<Interviewer> Collection = new RetrieveData<Interviewer>();
                Collection.Url = helper.LivePathImages;
                Collection.DataList = ResultInterviewers.ToList();

                return Collection;
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
            }
        }
        #endregion

        #region Insert New Interviewer 
        //[ApiAuthentication]
        [HttpPost]
        [Route("createinterviewer")]
        public async Task<ActionResult<InterviewerModelInput>> createinterviewerAsync([FromQuery] InterviewerModelInput model)
        {
            try
            {
                var InterviewerIamge = HttpContext.Request.Form.Files["InterviewerIamge"];
                var InterviewerCover = HttpContext.Request.Form.Files["InterviewerCover"];

                model.InterviewerPicture = InterviewerIamge;
                model.InterviewerCover = InterviewerCover;

                DateTime EpisodePublishDate = new DateTime();

                #region Check values of Interviewer is not null or empty

                if (string.IsNullOrEmpty(model.InterviewerName))
                    return BadRequest("Category Title cannot be null or empty");

                if (model.InterviewerPicture == null)
                    return BadRequest("Interviewer Picture cannot be null ");

                if (string.IsNullOrEmpty(model.InterviewerDescription))
                    return BadRequest("Interviewer Description cannot be null or empty");

                if(model.BirthDate == null)
                    return BadRequest("Interviewer Birth Date cannot be null or empty");

               if(model.InterviewerPicture == null)
                    return BadRequest("Interviewer Picture cannot be null ");

                if (model.InterviewerCover == null)
                    return BadRequest("Interviewer Cover cannot be null ");


                if (model.BirthDate.Contains("T"))
                {
                    model.BirthDate = model.BirthDate.Substring(0, model.BirthDate.IndexOf("T"));
                }
               
                    EpisodePublishDate = DateTime.ParseExact(model.BirthDate, "yyyy-MM-dd", null);
               


                #endregion

                #region fill Interviewer object with values to insert 
                Interviewer _interviewer = new Interviewer()
                {
                    InterviewerName = model.InterviewerName,
                    InterviewerPicture =helper.UploadImage(model.InterviewerPicture),
                    CreationDate = DateTime.Now,
                    InterviewerDescription = model.InterviewerDescription,
                    FacebookUrl = model.FacebookUrl,
                    InstgramUrl = model.InstgramUrl,
                    LinkedInUrl = model.LinkedInUrl,
                    TiktokUrl = model.TiktokUrl,
                    TwitterUrl = model.TwitterUrl,
                    WebsiteUrl = model.WebsiteUrl,
                    YoutubeUrl = model.YoutubeUrl,
                    BirthDate = EpisodePublishDate ,
                    InterviewerCover = helper.UploadImage(model.InterviewerCover),
                };
                #endregion

                #region Create Operation
                bool result = await unitOfWork.Interviewer.Create(_interviewer);
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

        #region Edit Interviewer
        //[ApiAuthentication]
        [HttpPut]
        [Route("putinterviewer")]
        public async Task<ActionResult<Interviewer>> putinterviewer([FromQuery] InterviewerToUpdate model)
        {
            try
            {
                DateTime? BirthDate = new DateTime();
                string picture, cover;

                var InterviewerIamge = HttpContext.Request.Form.Files["InterviewerIamge"];
                var InterviewerCover = HttpContext.Request.Form.Files["InterviewerCover"];

                model.InterviewerPicture = InterviewerIamge;
                model.InterviewerCover = InterviewerCover;


                #region check Interviewer id exist
                var _InterviewerObj = await unitOfWork.Interviewer.FindObjectAsync(model.InterviewerId);
                if (_InterviewerObj == null)
                    return Ok("Interviewer ID Not Found");
                #endregion

                #region Check values of Interviewer is not null or empty
                if (string.IsNullOrEmpty(model.InterviewerName))
                    model.InterviewerName = _InterviewerObj.InterviewerName;

                if (string.IsNullOrEmpty(model.InterviewerDescription))
                    model.InterviewerDescription = _InterviewerObj.InterviewerDescription;

                //if (model.InterviewerPicture == null)
                //    model.InterviewerPicturePath = _InterviewerObj.InterviewerPicture;

                //if (model.InterviewerCover == null)
                //    model.InterviewerCoverePath = _InterviewerObj.InterviewerCover;

                if (string.IsNullOrEmpty(model.FacebookUrl))
                    model.FacebookUrl = _InterviewerObj.FacebookUrl;

                if (string.IsNullOrEmpty(model.InstgramUrl))
                    model.InstgramUrl = _InterviewerObj.InstgramUrl;

                if (string.IsNullOrEmpty(model.TwitterUrl))
                    model.TwitterUrl = _InterviewerObj.TwitterUrl;

                if (string.IsNullOrEmpty(model.YoutubeUrl))
                    model.YoutubeUrl = _InterviewerObj.YoutubeUrl;

                if (string.IsNullOrEmpty(model.LinkedInUrl))
                    model.LinkedInUrl = _InterviewerObj.LinkedInUrl;

                if (string.IsNullOrEmpty(model.WebsiteUrl))
                    model.WebsiteUrl = _InterviewerObj.WebsiteUrl;

                if (string.IsNullOrEmpty(model.TiktokUrl))
                    model.TiktokUrl = _InterviewerObj.TiktokUrl;


                if (model.BirthDate == null)
                    BirthDate = _InterviewerObj.BirthDate;

                //if (model.BirthDate.Contains("T"))
                //{
                //    model.BirthDate = model.BirthDate.Substring(0, model.BirthDate.IndexOf("T"));
                //}

                //BirthDate = DateTime.ParseExact(model.BirthDate, "yyyy-MM-dd", null);

                if (model.changeDate != true)
                {
                    BirthDate = _InterviewerObj.BirthDate;
                }
                else
                {
                    if (model.BirthDate.Contains("T"))
                        model.BirthDate = model.BirthDate.Substring(0, model.BirthDate.IndexOf("T"));

                    BirthDate = DateTime.ParseExact(model.BirthDate, "yyyy-MM-dd", null);
                }


                #region check if picture updated or not 

                if ( model.InterviewerPicture == null)
                {
                    picture = _InterviewerObj.InterviewerPicture;
                }
                else
                {
                   picture = helper.UploadImage(model.InterviewerPicture);
                }
                #endregion

                #region check if Cover updated or not 

                if (model.InterviewerCover == null)
                {
                    cover = _InterviewerObj.InterviewerPicture;
                }
                else
                {
                    cover = helper.UploadImage(model.InterviewerCover);
                }
                #endregion

                //#region check if image updated or not 

                //if (model.InterviewerCoverePath == null && model.InterviewerCover == null)
                //{
                //    model.InterviewerCoverePath = _InterviewerObj.InterviewerCover;
                //}
                //if (model.InterviewerCoverePath == null)
                //{
                //    model.InterviewerCoverePath = helper.UploadImage(model.InterviewerCover);
                //}
                //#endregion

                #endregion

                #region fill Interviewer object with values to insert 
                Interviewer _interviewer = new Interviewer()
                {
                    InterviewerId = model.InterviewerId,
                    InterviewerName = model.InterviewerName,
                    InterviewerPicture = picture,
                    InterviewerCover = cover,
                    CreationDate = DateTime.Now,
                    InterviewerDescription = model.InterviewerDescription,
                    FacebookUrl = model.FacebookUrl,
                    InstgramUrl = model.InstgramUrl,
                    LinkedInUrl = model.LinkedInUrl,
                    TiktokUrl = model.TiktokUrl,
                    TwitterUrl = model.TwitterUrl,
                    WebsiteUrl = model.WebsiteUrl,
                    YoutubeUrl = model.YoutubeUrl,
                    BirthDate = BirthDate
                };
                #endregion

                #region update operation
                bool result = unitOfWork.Interviewer.Update(_interviewer);
                #endregion

                #region check operation is updated successed
                if (!result)
                    return BadRequest("Create Operation Failed");
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

        #region Delete Interviewer
        //[Authorize]
        [HttpDelete("{ID}")]
        public async Task<ActionResult<Category>> deleteinterviewer(int ID)
        {
            try
            {
                #region Check interviewer ID If Exist or not 
                var checkIDIfExist = await unitOfWork.Interviewer.FindObjectAsync(ID);
                if (checkIDIfExist == null)
                    return BadRequest("Interviewer ID Not Found");
                #endregion

                #region Delete Operation
                bool result = await unitOfWork.Interviewer.DeleteObject(ID);
                #endregion

                #region check Delete Operation  successed
                if (!result)
                    return BadRequest("Interviewer Not Exist");
                #endregion

                #region save changes in db
                await unitOfWork.Complete();
                #endregion

                #region Delete image File From Specified Directory 
                helper.DeleteFiles(checkIDIfExist.InterviewerPicture);
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

        #region Get All Programs Related with interviewer ID  API Key Authentication
        [ApiAuthentication]
        [HttpGet]
        [Route("getprogramsbyinterviewerid")]
        public async Task<ActionResult<RetrieveData<Program>>> getprogramsbycategoryidAsync(int ID)
        {
            // Create ProgramsList to return It
            List<ProgramFilterModel> _programsList = new List<ProgramFilterModel>();
            try
            {
                //Get All Programs 
                var ResultPrograms =await unitOfWork.Program.GetObjects(x => x.InterviewerId == ID); 

                RetrieveData<Program> Collection = new RetrieveData<Program>();
                Collection.Url = helper.LivePathImages;
                Collection.DataList = ResultPrograms.ToList();

                return Collection;
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
            }
        }
        #endregion

        //#region Get All Episodes Related with interviewer ID  API Key Authentication
        //[ApiAuthentication]
        //[HttpGet]
        //[Route("getepisodesbyinterviewerid")]
        //public async Task<ActionResult<List<EpisodeModelOutput>>> getepisodesbyintervieweridAsync(int ID)
        //{
        //    // Create EpisodeList to return It
        //    List<EpisodeModelOutput> _episodeList = new List<EpisodeModelOutput>();
        //    try
        //    {
        //        //Get All Programs 
        //        var ResultPrograms = await unitOfWork.Program.GetObjects(x => x.InterviewerId == ID); ResultPrograms.ToList();
        //        var ResultSeasons =  await unitOfWork.Season.GetObjects(); ResultSeasons.ToList();
        //        var ResultEpisodes = await unitOfWork.Episode.GetObjects(); ResultEpisodes.ToList();
        //        var EpisodesRelatedWithInterviewer = from program in ResultPrograms
        //                                             join season in ResultSeasons
        //                                             on program.ProgramId equals season.ProgramId
        //                                             join episode in ResultEpisodes
        //                                             on season.SessionId equals episode.SessionId
        //                                             select new
        //                                             {
        //                                                 episode.EpisodeId,
        //                                                 episode.EpisodeTitle,
        //                                                 episode.EpisodeViews,
        //                                                 episode.EpisodePublishDate,
        //                                                 episode.CreationDate,
        //                                                 episode.LikeRate,
        //                                                 episode.DislikeRate,
        //                                                 episode.EpisodeDescription,
        //                                                 episode.YoutubeUrl,
        //                                                 episode.EpisodeIamgePath
        //                                             };
        //        #region prepare Episode List  to return it 
        //        foreach (var item in EpisodesRelatedWithInterviewer)
        //        {
        //            #region Create fill Category Object  
        //            EpisodeModelOutput _episode = new EpisodeModelOutput()
        //            {
        //                EpisodeId = item.EpisodeId,
        //                EpisodeTitle = item.EpisodeTitle,
        //                EpisodeViews = item.EpisodeViews,
        //                EpisodeDescription = item.EpisodeDescription,
        //                CreationDate = item.CreationDate,
        //                EpisodePublishDate = item.EpisodePublishDate,
        //                YoutubeUrl = item.YoutubeUrl,
        //                LikeRate = (int)item.LikeRate,
        //                DislikeRate = (int)item.DislikeRate,
        //                EpisodeIamgePath = helper.LivePathImages+item.EpisodeIamgePath,
                        
        //            };

        //            // Finally Add It Into Programs List
        //            _episodeList.Add(_episode);
        //            #endregion
        //        }
        //        #endregion
        //        return _episodeList;
        //    }
        //    catch (Exception ex)
        //    {
        //        helper.LogError(ex);
        //        return StatusCode(StatusCodes.Status500InternalServerError);
        //        // Log error in db
        //    }
        //}
        //#endregion

        #region Get Most Viewed Episodes Related with interviewer ID  API Key Authentication
        [ApiAuthentication]
        [HttpGet]
        [Route("mostviewedepisodesbyinterviewerid")]
        public async Task<ActionResult<RetrieveData<EpisodesRelatedForRecentlyModel>>> mostviewedepisodesbyintervieweridAsync([FromQuery] MostviewedEpisodesByInterviewerIDModel input)
        {
            // Create EpisodeList to return It
            List<EpisodesRelatedForRecentlyModel> _episodeList = new List<EpisodesRelatedForRecentlyModel>();
            RetrieveData<EpisodesRelatedForRecentlyModel> Collection = new RetrieveData<EpisodesRelatedForRecentlyModel>();

            try
            {
                //Get All Programs 
                var ResultPrograms = await unitOfWork.Program.GetObjects(x => x.InterviewerId == input.ID); ResultPrograms.ToList();
                var categories = await unitOfWork.category.GetObjects(); categories.ToList();
                var programtypes = await unitOfWork.ProgramType.GetObjects(); programtypes.ToList();
                var ResultSeasons =  await unitOfWork.Season.GetObjects(); ResultSeasons.ToList();
                var ResultEpisodes = await unitOfWork.Episode.GetObjects(); ResultEpisodes.ToList();

                var EpisodesRelatedWithInterviewer = from program in ResultPrograms
                                                     join season in ResultSeasons
                                                     on program.ProgramId equals season.ProgramId
                                                     join episode in ResultEpisodes
                                                     on season.SessionId equals episode.SessionId
                                                     join category in categories
                                                     on program.CategoryId equals category.CategoryId
                                                     join programType in programtypes
                                                     on program.ProgramTypeId equals programType.ProgramTypeId
                                                     select new
                                                     {
                                                         category.CategoryId,
                                                         category.CategoryTitle,

                                                         season.SessionId,

                                                         episode.EpisodeId,
                                                         episode.EpisodeTitle,
                                                         episode.EpisodeViews,
                                                         episode.EpisodePublishDate,
                                                         episode.EpisodeDescription,
                                                         episode.YoutubeUrl,
                                                         episode.EpisodeIamgePath,

                                                         program.ProgramId,
                                                         program.ProgramName,
                                                         program.ProgramImg,

                                                         programType.ProgramTypeId,
                                                         programType.ProgramTypeTitle
                                                     };
                if (input.Order == "DESC" || input.Order == "desc")
                {
                    var MostView = EpisodesRelatedWithInterviewer.OrderByDescending(o => o.EpisodeViews).Take(trendingDuration.Top).ToList();
                    #region prepare Episode List  to return it 
                    foreach (var item in MostView)
                    {
                        #region Create fill Category Object  
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
                            ProgramImg = helper.LivePathImages + item.ProgramImg,
                            ProgramName = item.ProgramName,
                            ProgramTypeId = item.ProgramTypeId,
                            ProgramTypeTitle = item.ProgramTypeTitle,
                            EpisodeUrl = item.YoutubeUrl,
                            EpisodeImg = helper.LivePathImages + item.EpisodeIamgePath
                        };

                        // Finally Add It Into Programs List
                        _episodeList.Add(model);
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    var MostView = EpisodesRelatedWithInterviewer.OrderBy(o => o.EpisodeViews).Take(trendingDuration.Top).ToList();
                    #region prepare Episode List  to return it 
                    foreach (var item in MostView)
                    {
                        #region Create fill Category Object  
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
                            ProgramImg = helper.LivePathImages + item.ProgramImg,
                            ProgramName = item.ProgramName,
                            ProgramTypeId = item.ProgramTypeId,
                            ProgramTypeTitle = item.ProgramTypeTitle,
                            EpisodeUrl = item.YoutubeUrl,
                            EpisodeImg = helper.LivePathImages + item.EpisodeIamgePath
                        };

                        // Finally Add It Into Programs List
                        _episodeList.Add(model);
                        #endregion
                    }
                    #endregion
                }


                Collection.Url = helper.LivePathImages;
                Collection.DataList = _episodeList;
                return Collection;
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
            }
        }
        #endregion

        #region Most Viewed Programs By Interviewer Id API Key Authentication
        //[ApiAuthentication]
        [HttpGet]
        [Route("mostviewedprogramsbyinterviewerid")]
        public async Task<ActionResult<RetrieveData<ProgramsMostViewsModel>>> mostviewedprogramsbyinterviewerid([FromQuery] MostViewProgramByInterviewerInput input)
        {
            try
            {
                #region Declare Variables
                List<(int, int)> SeasonSorted = new List<(int, int)>();
                List<(Program, int)> ProgramSorted = new List<(Program, int)>();
                IDictionary<int, int> SeasonIDWithCount = new Dictionary<int, int>();
                List<ProgramsMostViewsModel> mostViewsModels = new List<ProgramsMostViewsModel>();
                RetrieveData<ProgramsMostViewsModel> Collection = new RetrieveData<ProgramsMostViewsModel>();

                #endregion

                #region Fetch All Seasons from db
                var SeasonList = await unitOfWork.Season.GetObjects(); SeasonList.ToList();
                #endregion

                #region Iterate on Seasons to calculate Top Views 
                foreach (var SeasonItem in SeasonList)
                {
                    #region Fetch All Episodes related with Season ID
                    var EpisodesLIst =await unitOfWork.Episode.GetObjects(x => x.SessionId == SeasonItem.SessionId); EpisodesLIst.ToList();
                    #endregion

                    #region Calculate SUM Views For All Episode Related with Season ID
                    int CounterOnViews = 0;
                    foreach (var EpisodeItem in EpisodesLIst)
                    {
                        CounterOnViews += EpisodeItem.EpisodeViews;
                    }
                    #endregion

                    #region Add Program ID With Totle Views Of Episodes to List
                    SeasonIDWithCount.Add(SeasonItem.SessionId, CounterOnViews);
                    #endregion
                }
                #endregion

                #region Soert Season with Views
                foreach (KeyValuePair<int, int> item in SeasonIDWithCount.OrderBy(views => views.Value))
                {
                    SeasonSorted.Add((item.Key, item.Value));
                }
                #endregion

                #region get seasons Objects Sorted
                List<(Seasons, int)> seasons = new List<(Seasons, int)>();
                int count = SeasonSorted.Count;
                foreach (var item in SeasonSorted)
                {
                    var _season = await unitOfWork.Season.FindObjectAsync(item.Item1);
                    seasons.Add(((Seasons, int))(_season, item.Item2));
                }
                #endregion

                #region Fetch All program from db
                var ProgramList =await unitOfWork.Program.GetObjects(X => X.InterviewerId == input.InterviewerID); ProgramList.ToList();
                #endregion

                #region Iterate on program to calculate Top Views 
                foreach (var ProgramItem in ProgramList)
                {
                    int SumSeasonViews = 0;
                    foreach (var item in seasons)
                    {
                        if (item.Item1.ProgramId == ProgramItem.ProgramId)
                        {
                            SumSeasonViews += item.Item2;
                            //seasons.Remove((item.Item1, item.Item2));
                        }
                    }
                    ProgramSorted.Add((ProgramItem,SumSeasonViews));
                }
                #endregion            

                #region Reverse Sorted program List if Order = 'D' OR 'd'
                if (input.Order == "desc" || input.Order == "DESC")
                    ProgramSorted.Reverse();

                #endregion

                #region Fetch Top Number Of Programs Views
                if (ProgramSorted.Count > input.Top)
                {
                    int iterate = 0;
                    foreach (var item in ProgramSorted)
                    {
                        if (iterate < input.Top)
                        {
                            ProgramsMostViewsModel model = new ProgramsMostViewsModel()
                            {
                                ProgramId = item.Item1.ProgramId,
                                CategoryId = item.Item1.CategoryId,
                                InterviewerId = item.Item1.InterviewerId,
                                ProgramDescription = item.Item1.ProgramDescription,
                                ProgramName = item.Item1.ProgramName,
                                ProgramOrder = item.Item1.ProgramOrder,
                                ProgramStartDate = item.Item1.ProgramStartDate,
                                ProgramTypeId = item.Item1.ProgramTypeId,
                                ProgramVisible = (bool)item.Item1.ProgramVisible,
                                Views = item.Item2,
                                ProgramImg =helper.LivePathImages+ item.Item1.ProgramImg
                            };
                            mostViewsModels.Add(model);
                            iterate++;
                        }
                    }
                }
                else
                {
                    foreach (var item in ProgramSorted)
                    {
                        ProgramsMostViewsModel model = new ProgramsMostViewsModel()
                        {
                            ProgramId = item.Item1.ProgramId,
                            CategoryId = item.Item1.CategoryId,
                            InterviewerId = item.Item1.InterviewerId,
                            ProgramDescription = item.Item1.ProgramDescription,
                            ProgramName = item.Item1.ProgramName,
                            ProgramOrder = item.Item1.ProgramOrder,
                            ProgramStartDate = item.Item1.ProgramStartDate,
                            ProgramTypeId = item.Item1.ProgramTypeId,
                            ProgramVisible = (bool)item.Item1.ProgramVisible,
                            Views = item.Item2,
                            ProgramImg =helper.LivePathImages+ item.Item1.ProgramImg
                        };
                        mostViewsModels.Add(model);
                    }
                }
                #endregion

                Collection.Url = helper.LivePathImages;
                Collection.DataList = mostViewsModels;

                return Collection;
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Get All Iterviewer Sorted by Views API Key Authentication
        /// <summary>
        /// Get All Interviewers According to Total Views For Episodes Their own
        /// </summary>
        /// <returns>
        /// Take No Parameter
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("geteallinterviewerbyviews")]
        public async Task<ActionResult<RetrieveData<InterviewerViewsModel>>> geteallinterviewerbyviewsAsync()
        {
           
            try
            {
                // Create EpisodeList to return It
                List<InterviewerModel> _episodeList = new List<InterviewerModel>();
                List<InterviewerViewsModel> interviewerViews = new List<InterviewerViewsModel>();
                List<InterviewerViewsModel> interviewerViewsOrdered = new List<InterviewerViewsModel>();
                //Get All Interviewer from Db
                var ResultInterviewer = await unitOfWork.Interviewer.GetObjects(); ResultInterviewer.ToList();
                //Get All Programs from Db
                var ResultPrograms = await unitOfWork.Program.GetObjects(); ResultPrograms.ToList();
                //Get All Seasons from Db
                var ResultSeasons = await unitOfWork.Season.GetObjects(); ResultSeasons.ToList();
                //Get All Episodes from Db
                var ResultEpisodes = await unitOfWork.Episode.GetObjects(); ResultEpisodes.ToList();
                //Get All Episodes for all Interviewers from Db
                var EpisodesRelatedWithInterviewer = from interviewer in ResultInterviewer
                                                     join program in ResultPrograms
                                                     on interviewer.InterviewerId equals program.InterviewerId
                                                     join season in ResultSeasons
                                                     on program.ProgramId equals season.ProgramId
                                                     join episode in ResultEpisodes
                                                     on season.SessionId equals episode.SessionId
                                                     select new
                                                     {
                                                        interviewer.InterviewerName,
                                                        interviewer.InterviewerDescription,
                                                        interviewer.InterviewerPicture,
                                                        interviewer.CreationDate,
                                                        interviewer.FacebookUrl,
                                                        interviewer.InstgramUrl,
                                                        interviewer.InterviewerId,
                                                        interviewer.LinkedInUrl,
                                                        interviewer.TiktokUrl,
                                                        interviewer.TwitterUrl,
                                                        interviewer.WebsiteUrl,
                                                        interviewer.YoutubeUrl,
                                                        episode.EpisodeViews
                                                     };
               
                #region Select interviewer and Calculate Episode Views
                foreach (var _interviewer in ResultInterviewer)
                {
                    InterviewerViewsModel interviewerViewsModel= new InterviewerViewsModel();   
                    var _count = EpisodesRelatedWithInterviewer.
                                 Where(Obj => Obj.InterviewerId == _interviewer.InterviewerId).
                                 Sum(Obj => Obj.EpisodeViews);
                    interviewerViewsModel.Interviewer = _interviewer;
                    interviewerViewsModel.Views = _count;

                    interviewerViews.Add(interviewerViewsModel);
                }
                #endregion
                //Order Interviewers by views 
                interviewerViewsOrdered =  interviewerViews.OrderByDescending(x => x.Views).ToList();

                RetrieveData<InterviewerViewsModel> Collection = new RetrieveData<InterviewerViewsModel>();
                Collection.DataList = interviewerViewsOrdered;
                Collection.Url = helper.LivePathImages;

                return Collection;
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
            }
        }
        #endregion

        #region Get Super Star
        [ApiAuthentication]
        [HttpGet]
        [Route("superstar")]
        public ActionResult<RetrieveData<SuperStarModel>> superstar()
        {
            try
            {
                List<SuperStarModel> superStarModels = new List<SuperStarModel>();
                var result = dbContext.SuperStarModel.FromSqlInterpolated($"EXEC [dbo].[SuperStarsSP]").ToList();
                //foreach (var item in result)
                //{
                //    SuperStarModel model = new SuperStarModel()
                //    {
                //        InterviewerID = item.InterviewerID,
                //        EpisodeViews = item.EpisodeViews,
                //        InterviewerDescription = item.InterviewerDescription,
                //        InterviewerName = item.InterviewerName,
                //        InterviewerPicture = helper.LivePathImages + item.InterviewerPicture
                //    };
                //    superStarModels.Add(model);
                //}

                RetrieveData<SuperStarModel> Collection = new RetrieveData<SuperStarModel>();
                Collection.DataList = result;
                Collection.Url = helper.LivePathImages;

                return Collection;
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
