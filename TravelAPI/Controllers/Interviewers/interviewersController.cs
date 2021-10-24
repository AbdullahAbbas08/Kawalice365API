using BalarinaAPI.Authentication;
using BalarinaAPI.Core.Model;
using BalarinaAPI.Core.ViewModel;
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
        private readonly BalarinaDatabaseContext dbContext;

        #endregion

        #region Constructor
        public interviewersController(IUnitOfWork _unitOfWork, IOptions<Helper> _helper , BalarinaDatabaseContext _dbContext)
        {
            unitOfWork = _unitOfWork;
            this.helper = _helper.Value;
            dbContext = _dbContext;
        }
        #endregion

        #region CRUD Operations For Interviewers

        #region Get All Interviewer With API Key
        [ApiAuthentication]
        [HttpGet]
        [Route("getalliterviewersapikey")]
        public async Task<ActionResult<List<InterviewerModel>>> getalliterviewersapikey()
        {
            // Create Categories list to return It
            List<InterviewerModel> Interviewers = new List<InterviewerModel>();
            try
            {
                //Get All Interviewer 
                var ResultInterviewers = await unitOfWork.Interviewer.GetObjects(); ResultInterviewers.ToList();

                #region Fill InterviewerList and Handle Image Path For all Categories
                foreach (var item in ResultInterviewers)
                {
                    // Create Interviewer Object
                    InterviewerModel _interviewer = new InterviewerModel()
                    {
                        InterviewerId = item.InterviewerId,
                        InterviewerName = item.InterviewerName,
                        InterviewerPicture =helper.LivePathImages+ item.InterviewerPicture,
                        CreationDate = DateTime.Now,
                        InterviewerDescription = item.InterviewerDescription,
                        FacebookUrl = item.FacebookUrl,
                        InstgramUrl = item.InstgramUrl,
                        LinkedInUrl = item.LinkedInUrl,
                        TiktokUrl = item.TiktokUrl,
                        TwitterUrl = item.TwitterUrl,
                        WebsiteUrl = item.WebsiteUrl,
                        YoutubeUrl = item.YoutubeUrl,
                    };

                    // Finally Add It Into Interviewers List
                    Interviewers.Add(_interviewer);
                }
                #endregion
                return Interviewers;
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
            }
        }
        #endregion

        #region Get All Interviewer
        [Authorize]
        [HttpGet]
        [Route("getalliterviewers")]
        public async Task<ActionResult<List<InterviewerModel>>> getalliterviewersAsync()
        {
            // Create Categories list to return It
            List<InterviewerModel> Interviewers = new List<InterviewerModel>();
            try
            {
                //Get All Interviewer 
                var ResultInterviewers = await unitOfWork.Interviewer.GetObjects(); ResultInterviewers.ToList();

                #region Fill InterviewerList and Handle Image Path For all Categories
                foreach (var item in ResultInterviewers)
                {
                    // Create Interviewer Object
                    InterviewerModel _interviewer = new InterviewerModel()
                    {
                        InterviewerId = item.InterviewerId,
                        InterviewerName = item.InterviewerName,
                        InterviewerPicture =helper.LivePathImages+ item.InterviewerPicture,
                        CreationDate = DateTime.Now,
                        InterviewerDescription = item.InterviewerDescription,
                        FacebookUrl = item.FacebookUrl,
                        InstgramUrl = item.InstgramUrl,
                        LinkedInUrl = item.LinkedInUrl,
                        TiktokUrl = item.TiktokUrl,
                        TwitterUrl = item.TwitterUrl,
                        WebsiteUrl = item.WebsiteUrl,
                        YoutubeUrl = item.YoutubeUrl,
                    };

                    // Finally Add It Into Interviewers List
                    Interviewers.Add(_interviewer);
                }
                #endregion
                return Interviewers;
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
        [Authorize]
        [HttpPost]
        [Route("createinterviewer")]
        public async Task<ActionResult<InterviewerModelInput>> createinterviewerAsync([FromQuery] InterviewerModelInput model)
        {
            try
            {
                #region Check values of Interviewer is not null or empty

                if (string.IsNullOrEmpty(model.InterviewerName))
                    return BadRequest("Category Title cannot be null or empty");

                if (model._InterviewerPicture == null)
                    return BadRequest("Interviewer Picture cannot be null ");

                if (string.IsNullOrEmpty(model.InterviewerDescription))
                    return BadRequest("Interviewer Description cannot be null or empty");


                #endregion

                #region fill Interviewer object with values to insert 
                Interviewer _interviewer = new Interviewer()
                {
                    InterviewerName = model.InterviewerName,
                    InterviewerPicture = UploadImage(model._InterviewerPicture),
                    CreationDate = DateTime.Now,
                    InterviewerDescription = model.InterviewerDescription,
                    FacebookUrl = model.FacebookUrl,
                    InstgramUrl = model.InstgramUrl,
                    LinkedInUrl = model.LinkedInUrl,
                    TiktokUrl = model.TiktokUrl,
                    TwitterUrl = model.TwitterUrl,
                    WebsiteUrl = model.WebsiteUrl,
                    YoutubeUrl = model.YoutubeUrl,
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
                unitOfWork.Complete();
                #endregion

                return Ok("INTERVIEWER CREATED SUCCESSFIILY ");
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion 

        #region Edit Interviewer
        [Authorize]
        [HttpPut]
        [Route("putinterviewer")]
        public async Task<ActionResult<Interviewer>> putinterviewer([FromQuery] InterviewerToUpdate model)
        {
            try
            {
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

                #region check if image updated or not 

                if (model.InterviewerPicturePath == null && model.InterviewerPicture == null)
                {
                    model.InterviewerPicturePath = _InterviewerObj.InterviewerPicture;
                }
                if (model.InterviewerPicturePath == null)
                {
                    model.InterviewerPicturePath = UploadImage(model.InterviewerPicture);
                }
                #endregion

                #endregion

                #region fill Interviewer object with values to insert 
                Interviewer _interviewer = new Interviewer()
                {
                    InterviewerId = model.InterviewerId,
                    InterviewerName = model.InterviewerName,
                    InterviewerPicture = model.InterviewerPicturePath,
                    CreationDate = DateTime.Now,
                    InterviewerDescription = model.InterviewerDescription,
                    FacebookUrl = model.FacebookUrl,
                    InstgramUrl = model.InstgramUrl,
                    LinkedInUrl = model.LinkedInUrl,
                    TiktokUrl = model.TiktokUrl,
                    TwitterUrl = model.TwitterUrl,
                    WebsiteUrl = model.WebsiteUrl,
                    YoutubeUrl = model.YoutubeUrl,
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
                unitOfWork.Complete();
                #endregion

                return Ok("INTERVIEWER UPDATED SUCCESSFULLY");
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Delete Interviewer
        [Authorize]
        [HttpDelete("{ID}")]
        public async Task<ActionResult<Category>> deleteinterviewerAsync(int ID)
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
                unitOfWork.Complete();
                #endregion

                #region Delete image File From Specified Directory 
                helper.DeleteFiles(checkIDIfExist.InterviewerPicture);
                #endregion


                return Ok("INTERVIEWER DELETED SCCESSFULLY");
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
        public async Task<ActionResult<List<ProgramFilterModel>>> getprogramsbycategoryidAsync(int ID)
        {
            // Create ProgramsList to return It
            List<ProgramFilterModel> _programsList = new List<ProgramFilterModel>();
            try
            {
                //Get All Programs 
                var ResultPrograms =await unitOfWork.Program.GetObjects(x => x.InterviewerId == ID); ResultPrograms.ToList();

                #region Fill ProgramsList and Handle Image Path For all Program
                foreach (var item in ResultPrograms)
                {
                    // Create Category Object
                    ProgramFilterModel _program = new ProgramFilterModel();
                    #region fill Category Object
                    _program.ProgramId = item.ProgramId;
                    _program.ProgramName = item.ProgramName;
                    _program.ProgramOrder = item.ProgramOrder;
                    _program.ProgramDescription = item.ProgramDescription;
                    _program.ProgramStartDate = item.ProgramStartDate;
                    _program.ProgramVisible = (bool)item.ProgramVisible;
                    _program.ProgramImg =helper.LivePathImages+ item.ProgramImg;
                    _program.CreationDate = item.CreationDate;
                    _program.CategoryId = item.CategoryId;
                    _program.InterviewerId = item.InterviewerId;

                    // Finally Add It Into Programs List
                    _programsList.Add(_program);
                    #endregion
                }
                #endregion
                return _programsList;
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
            }
        }
        #endregion

        #region Get All Episodes Related with interviewer ID  API Key Authentication
        [ApiAuthentication]
        [HttpGet]
        [Route("getepisodesbyinterviewerid")]
        public async Task<ActionResult<List<EpisodeModelOutput>>> getepisodesbyintervieweridAsync(int ID)
        {
            // Create EpisodeList to return It
            List<EpisodeModelOutput> _episodeList = new List<EpisodeModelOutput>();
            try
            {
                //Get All Programs 
                var ResultPrograms = await unitOfWork.Program.GetObjects(x => x.InterviewerId == ID); ResultPrograms.ToList();
                var ResultSeasons =  await unitOfWork.Season.GetObjects(); ResultSeasons.ToList();
                var ResultEpisodes = await unitOfWork.Episode.GetObjects(); ResultEpisodes.ToList();
                var EpisodesRelatedWithInterviewer = from program in ResultPrograms
                                                     join season in ResultSeasons
                                                     on program.ProgramId equals season.ProgramId
                                                     join episode in ResultEpisodes
                                                     on season.SessionId equals episode.SessionId
                                                     select new
                                                     {
                                                         episode.EpisodeId,
                                                         episode.EpisodeTitle,
                                                         episode.EpisodeViews,
                                                         episode.EpisodePublishDate,
                                                         episode.CreationDate,
                                                         episode.LikeRate,
                                                         episode.DislikeRate,
                                                         episode.EpisodeDescription,
                                                         episode.YoutubeUrl,
                                                         episode.EpisodeIamgePath
                                                     };
                #region prepare Episode List  to return it 
                foreach (var item in EpisodesRelatedWithInterviewer)
                {
                    #region Create fill Category Object  
                    EpisodeModelOutput _episode = new EpisodeModelOutput()
                    {
                        EpisodeId = item.EpisodeId,
                        EpisodeTitle = item.EpisodeTitle,
                        EpisodeViews = item.EpisodeViews,
                        EpisodeDescription = item.EpisodeDescription,
                        CreationDate = item.CreationDate,
                        EpisodePublishDate = item.EpisodePublishDate,
                        YoutubeUrl = item.YoutubeUrl,
                        LikeRate = (int)item.LikeRate,
                        DislikeRate = (int)item.DislikeRate,
                        EpisodeIamgePath = helper.LivePathImages+item.EpisodeIamgePath,
                        
                    };

                    // Finally Add It Into Programs List
                    _episodeList.Add(_episode);
                    #endregion
                }
                #endregion
                return _episodeList;
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
            }
        }
        #endregion

        #region Get Most Viewed Episodes Related with interviewer ID  API Key Authentication
        [ApiAuthentication]
        [HttpGet]
        [Route("mostviewedepisodesbyinterviewerid")]
        public async Task<ActionResult<List<EpisodeModelOutput>>> mostviewedepisodesbyintervieweridAsync([FromQuery] MostviewedEpisodesByInterviewerIDModel input)
        {
            // Create EpisodeList to return It
            List<EpisodeModelOutput> _episodeList = new List<EpisodeModelOutput>();
            try
            {
                //Get All Programs 
                var ResultPrograms = await unitOfWork.Program.GetObjects(x => x.InterviewerId == input.ID); ResultPrograms.ToList();
                var ResultSeasons =  await unitOfWork.Season.GetObjects(); ResultSeasons.ToList();
                var ResultEpisodes = await unitOfWork.Episode.GetObjects(); ResultEpisodes.ToList();

                var EpisodesRelatedWithInterviewer = from program in ResultPrograms
                                                     join season in ResultSeasons
                                                     on program.ProgramId equals season.ProgramId
                                                     join episode in ResultEpisodes
                                                     on season.SessionId equals episode.SessionId
                                                     select new
                                                     {
                                                         episode.EpisodeId,
                                                         episode.EpisodeTitle,
                                                         episode.EpisodeViews,
                                                         episode.EpisodePublishDate,
                                                         episode.CreationDate,
                                                         episode.LikeRate,
                                                         episode.DislikeRate,
                                                         episode.EpisodeDescription,
                                                         episode.YoutubeUrl,
                                                         episode.EpisodeIamgePath
                                                     };
                if (input.Order == "DESC" || input.Order == "desc")
                {
                    var MostView = EpisodesRelatedWithInterviewer.OrderByDescending(o => o.EpisodeViews).Take(input.Top).ToList();
                    #region prepare Episode List  to return it 
                    foreach (var item in MostView)
                    {
                        #region Create fill Category Object  
                        EpisodeModelOutput _episode = new EpisodeModelOutput()
                        {
                            EpisodeId = item.EpisodeId,
                            EpisodeTitle = item.EpisodeTitle,
                            EpisodeViews = item.EpisodeViews,
                            EpisodeDescription = item.EpisodeDescription,
                            CreationDate = item.CreationDate,
                            EpisodePublishDate = item.EpisodePublishDate,
                            YoutubeUrl = item.YoutubeUrl,
                            LikeRate = (int)item.LikeRate,
                            DislikeRate = (int)item.DislikeRate,
                            EpisodeIamgePath = helper.LivePathImages+item.EpisodeIamgePath
                        };

                        // Finally Add It Into Programs List
                        _episodeList.Add(_episode);
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    var MostView = EpisodesRelatedWithInterviewer.OrderBy(o => o.EpisodeViews).Take(input.Top).ToList();
                    #region prepare Episode List  to return it 
                    foreach (var item in MostView)
                    {
                        #region Create fill Category Object  
                        EpisodeModelOutput _episode = new EpisodeModelOutput()
                        {
                            EpisodeId = item.EpisodeId,
                            EpisodeTitle = item.EpisodeTitle,
                            EpisodeViews = item.EpisodeViews,
                            EpisodeDescription = item.EpisodeDescription,
                            CreationDate = item.CreationDate,
                            EpisodePublishDate = item.EpisodePublishDate,
                            YoutubeUrl = item.YoutubeUrl,
                            LikeRate = (int)item.LikeRate,
                            DislikeRate = (int)item.DislikeRate,
                            EpisodeIamgePath = helper.LivePathImages+item.EpisodeIamgePath
                        };
                        // Finally Add It Into Programs List
                        _episodeList.Add(_episode);
                        #endregion
                    }
                    #endregion
                }

                return _episodeList;
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
        public async Task<ActionResult<List<ProgramsMostViewsModel>>> mostviewedprogramsbyinterviewerid([FromQuery] MostViewProgramByInterviewerInput input)
        {
            try
            {
                #region Declare Variables
                List<(int, int)> SeasonSorted = new List<(int, int)>();
                List<(Program, int)> ProgramSorted = new List<(Program, int)>();
                IDictionary<int, int> SeasonIDWithCount = new Dictionary<int, int>();
                List<ProgramsMostViewsModel> mostViewsModels = new List<ProgramsMostViewsModel>();
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

                return mostViewsModels;
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
                        categoryImage.CopyTo(stream);
                    return fileName;
                }
                else
                    return "error";
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return "error";
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
        public async Task<ActionResult<List<(Interviewer, int)>>> geteallinterviewerbyviewsAsync()
        {
            // Create EpisodeList to return It
            List<InterviewerModel> _episodeList = new List<InterviewerModel>();
            try
            {
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
                //Create list of Interviewers and total views for it's
                List<(Interviewer,int)> CountViews = new List<(Interviewer,int)>();

                #region Select interviewer and Calculate Episode Views
                foreach (var _interviewer in ResultInterviewer)
                {
                    var _count = EpisodesRelatedWithInterviewer.
                                 Where(Obj => Obj.InterviewerId == _interviewer.InterviewerId).
                                 Sum(Obj => Obj.EpisodeViews);
                    CountViews.Add((_interviewer, _count)); 
                }
                #endregion
                //Order Interviewers by views 
                CountViews.OrderByDescending(x => x.Item2);

                return CountViews;
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
        public ActionResult<List<SuperStarModel>> superstar()
        {
            try
            {
                List<SuperStarModel> superStarModels = new List<SuperStarModel>();
                var result = dbContext.SuperStarModel.FromSqlInterpolated($"EXEC [dbo].[SuperStarsSP]").ToList();
                foreach (var item in result)
                {
                    SuperStarModel model = new SuperStarModel()
                    {
                        InterviewerID = item.InterviewerID,
                        EpisodeViews = item.EpisodeViews,
                        InterviewerDescription = item.InterviewerDescription,
                        InterviewerName = item.InterviewerName,
                        InterviewerPicture = helper.LivePathImages + item.InterviewerPicture
                    };
                    superStarModels.Add(model);
                }
                return superStarModels;
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
