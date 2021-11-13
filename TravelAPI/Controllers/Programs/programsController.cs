using BalarinaAPI.Authentication;
using BalarinaAPI.Core.Model;
using BalarinaAPI.Core.ViewModel;
using BalarinaAPI.Core.ViewModel.Category;
using BalarinaAPI.Core.ViewModel.Program;
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

namespace BalarinaAPI.Controllers.Programs
{
    [Route("api/[controller]")]
    [ApiController]
    public class programsController : ControllerBase
    {
        #region variables
        private readonly IUnitOfWork unitOfWork;
        private readonly Helper helper;
        #endregion

        #region Constructor
        public programsController(IUnitOfWork _unitOfWork, IOptions<Helper> _helper)
        {
            unitOfWork = _unitOfWork;
            this.helper = _helper.Value;

        }

        #endregion

        #region CRUD OPERATION

        #region Get All Programs API Key Authentication
        [ApiAuthentication]
        [HttpGet]
        [Route("getallprogramsapikey")]
        public async Task<ActionResult<RetrieveData<ProgramFilterModel>>> getallprogramsapikey()
        {
            try
            {
                // Create ProgramsList to return It
                List<ProgramFilterModel> _programsList = new List<ProgramFilterModel>();
                //Get All Programs 
                var ResultPrograms      = await unitOfWork.Program.GetObjects(); 
                var ResultCategory      = await unitOfWork.category.GetObjects();
                var ResultInterviewer   = await unitOfWork.Interviewer.GetObjects();
                var ResulProgramType    = await unitOfWork.ProgramType.GetObjects();

                var Result = (from program in ResultPrograms
                             join category in ResultCategory
                             on program.CategoryId equals category.CategoryId
                             join interviewer in ResultInterviewer
                             on program.InterviewerId equals interviewer.InterviewerId
                             join programtype in ResulProgramType
                             on program.ProgramTypeId equals programtype.ProgramTypeId
                             select new
                             {
                                 category.CategoryId,
                                 category.CategoryTitle,
                                 program.ProgramDescription,
                                 program.ProgramId,
                                 program.ProgramImg,
                                 program.ProgramName,
                                 program.ProgramPromoUrl,
                                 program.ProgramOrder,
                                 program.ProgramVisible,
                                 program.ProgramStartDate,
                                 program.CreationDate,
                                 interviewer.InterviewerId,
                                 interviewer.InterviewerName,
                                 programtype.ProgramTypeId,
                                 programtype.ProgramTypeTitle,
                             }).OrderBy(x=>x.ProgramOrder);
                //.Include(x => x.Category).Include(x => x.Interviewer).Include(x => x.ProgramType).OrderBy(x => x.ProgramOrder).ToList();

                #region Fill ProgramsList and Handle Image Path For all Program
                foreach (var item in Result)
                {
                    // Create Category Object
                    ProgramFilterModel _program = new ProgramFilterModel()
                    {
                        CategoryId          = item.CategoryId,
                        InterviewerId       = item.InterviewerId,
                        ProgramDescription  = item.ProgramDescription,
                        ProgramId           = item.ProgramId,
                        ProgramImg          = item.ProgramImg,
                        ProgramName         = item.ProgramName,
                        ProgramOrder        = item.ProgramOrder,
                        ProgramStartDate    = item.ProgramStartDate,
                        ProgramTypeId       = item.ProgramTypeId,
                        ProgramVisible      = (bool)item.ProgramVisible ,
                        CreationDate        = item.CreationDate,
                        CategoryName        = item.CategoryTitle,
                        InterviewerName     = item.InterviewerName,
                        ProgramTypeName     = item.ProgramTypeTitle,
                        ProgramPromoUrl = item.ProgramPromoUrl
                        
                    };
                    // Finally Add It Into Programs List
                    _programsList.Add(_program);
                }
                #endregion

                RetrieveData<ProgramFilterModel> Collection = new RetrieveData<ProgramFilterModel>();
                Collection.Url = helper.LivePathImages;
                Collection.DataList = _programsList;

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

        #region Get All Programs
        [Authorize]
        [HttpGet]
        [Route("getallprograms")]
        public async Task<ActionResult<RetrieveData<ProgramFilterModel>>> getallprogramsAsync()
        {
            try
            {
                // Create ProgramsList to return It
                List<ProgramFilterModel> _programsList = new List<ProgramFilterModel>();
                //Get All Programs 
                var ResultPrograms = await unitOfWork.Program.GetObjects();
                var ResultCategory = await unitOfWork.category.GetObjects();
                var ResultInterviewer = await unitOfWork.Interviewer.GetObjects();
                var ResulProgramType = await unitOfWork.ProgramType.GetObjects();

                var Result = (from program in ResultPrograms
                              join category in ResultCategory
                              on program.CategoryId equals category.CategoryId
                              join interviewer in ResultInterviewer
                              on program.InterviewerId equals interviewer.InterviewerId
                              join programtype in ResulProgramType
                              on program.ProgramTypeId equals programtype.ProgramTypeId
                              select new
                              {
                                  category.CategoryId,
                                  category.CategoryTitle,
                                  program.ProgramDescription,
                                  program.ProgramId,
                                  program.ProgramImg,
                                  program.ProgramName,
                                  program.ProgramOrder,
                                  program.ProgramVisible,
                                  program.ProgramPromoUrl,                                 
                                  program.ProgramStartDate,
                                  program.CreationDate,
                                  interviewer.InterviewerId,
                                  interviewer.InterviewerName,
                                  programtype.ProgramTypeId,
                                  programtype.ProgramTypeTitle,
                              }).OrderBy(x => x.ProgramOrder);
                //.Include(x => x.Category).Include(x => x.Interviewer).Include(x => x.ProgramType).OrderBy(x => x.ProgramOrder).ToList();

                #region Fill ProgramsList and Handle Image Path For all Program
                foreach (var item in Result)
                {
                    // Create Category Object
                    ProgramFilterModel _program = new ProgramFilterModel()
                    {
                        CategoryId = item.CategoryId,
                        InterviewerId = item.InterviewerId,
                        ProgramDescription = item.ProgramDescription,
                        ProgramId = item.ProgramId,
                        ProgramImg = item.ProgramImg,
                        ProgramName = item.ProgramName,
                        ProgramOrder = item.ProgramOrder,
                        ProgramStartDate = item.ProgramStartDate,
                        ProgramTypeId = item.ProgramTypeId,
                        ProgramVisible = (bool)item.ProgramVisible,
                        CreationDate = item.CreationDate,
                        CategoryName = item.CategoryTitle,
                        InterviewerName = item.InterviewerName,
                        ProgramTypeName = item.ProgramTypeTitle,
                        ProgramPromoUrl=item.ProgramPromoUrl
                    };
                    // Finally Add It Into Programs List
                    _programsList.Add(_program);
                }
                #endregion

                RetrieveData<ProgramFilterModel> Collection = new RetrieveData<ProgramFilterModel>();
                Collection.Url = helper.LivePathImages;
                Collection.DataList = _programsList;

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

        #region Get All categories Name , ID 
        /// <summary>
        /// Get All categories Name , ID 
        /// </summary>
        /// <returns>
        /// List of Object that Contains 
        /// CategoryId , CategoryName
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("getprogram_id_name")]
        public async Task<ActionResult<List<ListOfNameID<Object_ID_Name>>>> GetPrograms_ID_Name()
        {
            try
            {
                var _ProgramsObject = await unitOfWork.Program.GetObjects(); _ProgramsObject.ToList();

                List<ListOfNameID<Object_ID_Name>> Collection = new List<ListOfNameID<Object_ID_Name>>();
                foreach (var item in _ProgramsObject)
                {
                    ListOfNameID<Object_ID_Name> obj = new ListOfNameID<Object_ID_Name>() { ID = item.ProgramId, Name = item.ProgramName };
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

        #region Insert New program 
        //[Authorize]
        //[ApiAuthentication]
        [HttpPost]
        [Route("createprogram")]
        public async Task<ActionResult<ProgramViewModel>> createprogram([FromQuery]ProgramViewModel model)
        {
            try 
            {  
                var ProgramImage = HttpContext.Request.Form.Files["ProgramImage"];

                var typeID = unitOfWork.ProgramType.GetObjects(x=>x.ProgramTypeId == model.ProgramTypeId);
                if (typeID == null)
                    return BadRequest("program type ID Not Found ");

                var CategoryID = unitOfWork.category.GetObjects(Obj=>Obj.CategoryId == model.CategoryId);
                if (CategoryID == null)
                    return BadRequest("Category ID ID Not Found ");

                var interviewerID = unitOfWork.Interviewer.GetObjects(Obj=>Obj.InterviewerId == model.InterviewerId);
                if (interviewerID == null)
                    return BadRequest("Interviewer ID ID Not Found ");

                if (string.IsNullOrEmpty(model.ProgramName))
                    return BadRequest("program name cannot be null or empty");

                if (string.IsNullOrEmpty(model.ProgramDescription))
                    return BadRequest("program Description cannot be null or empty");

                if (ProgramImage != null)
                    model.ProgramImg = ProgramImage;

                if (model.ProgramImg == null)
                    return BadRequest("program Image cannot be null ");

                if (model.ProgramPromoUrl == null)
                    return BadRequest("program Promo Image cannot be null ");

                if (model.ProgramOrder < 0)
                    return BadRequest("program Order cannot be less than 0 ");

                if (model.ProgramStartDate == null)
                    return BadRequest("program Start Date  cannot be null or empty");

                if (model.ProgramVisible == null)
                    return BadRequest("Program Visible cannot be null ");

                var ProgramStartDate = DateTime.ParseExact(model.ProgramStartDate, "dd-MM-yyyy", null);
               

                Program _program = new Program()
                {

                    CreationDate = DateTime.Now,
                    CategoryId = model.CategoryId,
                    InterviewerId = model.InterviewerId,
                    ProgramTypeId = model.ProgramTypeId,

                    ProgramDescription = model.ProgramDescription,
                    ProgramImg = helper.UploadImage(model.ProgramImg),
                    ProgramPromoUrl = model.ProgramPromoUrl,
                    ProgramName = model.ProgramName,
                    ProgramOrder = model.ProgramOrder,
                    ProgramStartDate = ProgramStartDate,
                    ProgramVisible = (bool)model.ProgramVisible,
                };

                bool result = await unitOfWork.Program.Create(_program);

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

        #region Get All Programs Related with Category ID 
        [ApiAuthentication]
        [HttpGet]
        [Route("getallprogramsbycategoryid")]
        public async Task<ActionResult<RetrieveData<ProgramFilterModel>>> getallprogramsbycategoryid(int ID)
        {
            // Create ProgramsList to return It
            List<ProgramFilterModel> _programsList = new List<ProgramFilterModel>();
            try
            {
                #region Check Category ID Exist or Not
                var categoryObj = await unitOfWork.category.FindObjectAsync(ID);
                if (categoryObj == null)
                    return BadRequest("Category ID Not Found ");
                #endregion

                //Get All Programs 
                var ResultPrograms = await unitOfWork.Program.GetObjects(x=>x.CategoryId == ID);
                var ResultCategory = await unitOfWork.category.GetObjects();
                var ResultInterviewer = await unitOfWork.Interviewer.GetObjects();
                var ResulProgramType = await unitOfWork.ProgramType.GetObjects();

                var Result = (from program in ResultPrograms
                              join category in ResultCategory
                              on program.CategoryId equals category.CategoryId
                              join interviewer in ResultInterviewer
                              on program.InterviewerId equals interviewer.InterviewerId
                              join programtype in ResulProgramType
                              on program.ProgramTypeId equals programtype.ProgramTypeId
                              select new
                              {
                                  category.CategoryId,
                                  category.CategoryTitle,
                                  program.ProgramDescription,
                                  program.ProgramId,
                                  program.ProgramImg,
                                  program.ProgramName,
                                  program.ProgramOrder,
                                  program.ProgramVisible,
                                  program.ProgramStartDate,
                                  program.CreationDate,
                                  interviewer.InterviewerId,
                                  interviewer.InterviewerName,
                                  programtype.ProgramTypeId,
                                  programtype.ProgramTypeTitle,
                              }).OrderBy(x => x.ProgramOrder);
                //.Include(x => x.Category).Include(x => x.Interviewer).Include(x => x.ProgramType).OrderBy(x => x.ProgramOrder).ToList();

                #region Fill ProgramsList and Handle Image Path For all Program
                foreach (var item in Result)
                {
                    // Create Category Object
                    ProgramFilterModel _program = new ProgramFilterModel()
                    {
                        CategoryId = item.CategoryId,
                        InterviewerId = item.InterviewerId,
                        ProgramDescription = item.ProgramDescription,
                        ProgramId = item.ProgramId,
                        ProgramImg = item.ProgramImg,
                        ProgramName = item.ProgramName,
                        ProgramOrder = item.ProgramOrder,
                        ProgramStartDate = item.ProgramStartDate,
                        ProgramTypeId = item.ProgramTypeId,
                        ProgramVisible = (bool)item.ProgramVisible,
                        CreationDate = item.CreationDate,
                        CategoryName = item.CategoryTitle,
                        InterviewerName = item.InterviewerName,
                        ProgramTypeName = item.ProgramTypeTitle,
                    };
                    // Finally Add It Into Programs List
                    _programsList.Add(_program);
                }
                #endregion
                RetrieveData<ProgramFilterModel> Collection = new RetrieveData<ProgramFilterModel>();
                Collection.Url = helper.LivePathImages;
                Collection.DataList = _programsList;
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

        #region Get All Programs Related with type ID 
        [ApiAuthentication]
        [HttpGet]
        [Route("getallprogramsbytypeid")]
        public async Task<ActionResult<RetrieveData<ProgramFilterModel>>> getallprogramsbytypeid(int ID)
        {
            try
            {
                //Get All Programs 
                List<ProgramFilterModel> _programsList = new List<ProgramFilterModel>();
                var ResultPrograms = await unitOfWork.Program.GetObjects(x => x.ProgramTypeId == ID); ResultPrograms.ToList();
                #region Fill ProgramsList and Handle Image Path For all Program
                foreach (var item in ResultPrograms)
                {
                    // Create Category Object
                    ProgramFilterModel _program = new ProgramFilterModel()
                    {
                        CategoryId = item.CategoryId,
                        InterviewerId = item.InterviewerId,
                        ProgramDescription = item.ProgramDescription,
                        ProgramId = item.ProgramId,
                        ProgramImg =  item.ProgramImg,
                        ProgramName = item.ProgramName,
                        ProgramOrder = item.ProgramOrder,
                        ProgramStartDate = item.ProgramStartDate,
                        ProgramTypeId = item.ProgramTypeId,
                        ProgramVisible = (bool)item.ProgramVisible,
                        CreationDate = item.CreationDate,
                    };
                    // Finally Add It Into Programs List
                    _programsList.Add(_program);
                }
                #endregion
                RetrieveData< ProgramFilterModel> Collection = new RetrieveData< ProgramFilterModel>();
                Collection.Url = helper.LivePathImages;
                Collection.DataList = _programsList;
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

        #region Edit Program
        //[Authorize]
        //[ApiAuthentication]
        [HttpPut]
        [Route("putprogram")]
        public async Task<ActionResult<ProgramInsertModel>> putprogram([FromQuery] ProgramInsertModel model )
        {
            try
            {
                DateTime ProgramStartDate = new DateTime();
                var ProgramImage = HttpContext.Request.Form.Files["ProgramImage"];

                model.ProgramImg = ProgramImage;

                if (model.ProgramId == null)
                    return BadRequest("Program ID Invalid !! ");

                var _programObject = await unitOfWork.Program.FindObjectAsync( (int)model.ProgramId);
                if (_programObject == null)
                    return BadRequest("Program ID Not Found !! ");

                if (model.InterviewerId == null)
                    model.InterviewerId = _programObject.InterviewerId;
                else
                {
                    var _InterviewerObject =await unitOfWork.Interviewer.FindObjectAsync( (int)model.InterviewerId);
                    if (_InterviewerObject == null)
                        return BadRequest("Interviewer ID Not Found !! ");
                }

                if (model.ProgramTypeId == null)
                    model.ProgramTypeId = _programObject.ProgramTypeId;
                else
                {
                    var _ProgramTypeObject =await unitOfWork.ProgramType.FindObjectAsync((int)model.ProgramTypeId) ;
                    if (_ProgramTypeObject == null)
                        return BadRequest("program type ID Not Found !! ");
                }


                if (model.CategoryId == null)
                    model.CategoryId = _programObject.CategoryId;
                else
                {
                    var _CategoryObject =await  unitOfWork.category.FindObjectAsync( (int)model.CategoryId) ;
                    if (_CategoryObject == null)
                        return BadRequest("Category ID Not Found !! ");
                }

                if(model.ProgramDescription == null)
                    model.ProgramDescription = _programObject.ProgramDescription;

                if (model.ProgramName == null)
                    model.ProgramName = _programObject.ProgramName;

                #region check if image updated or not 

                if (model.ProgramImgPath == null)
                 {
                    if(model.ProgramImg != null)
                    model.ProgramImgPath =helper.UploadImage(model.ProgramImg);
                }
                if (model.ProgramImgPath == null && model.ProgramImg == null)
                {
                    model.ProgramImgPath = _programObject.ProgramImg;
                }
                #endregion


                if (model.ProgramVisible != true || model.ProgramVisible != false)
                    model.ProgramVisible = _programObject.ProgramVisible;


                if (model.ProgramOrder == null)
                    model.ProgramOrder = _programObject.ProgramOrder;

                if (model.ProgramViews == null)
                    model.ProgramViews = _programObject.ProgramViews;

               

                if (model.ProgramStartDate == null)
                    ProgramStartDate = _programObject.ProgramStartDate;

                if(model.ProgramStartDate.Contains("T"))
                {
                    model.ProgramStartDate = model.ProgramStartDate.Substring(0, model.ProgramStartDate.IndexOf("T"));
                }
               
                    ProgramStartDate = DateTime.ParseExact(model.ProgramStartDate, "dd-MM-yyyy", null);
              
                

                #region Handle Order Update 
                await UpdateOrder(model, _programObject.ProgramOrder);
                #endregion

                Program _program = new Program()
                {
                    ProgramId           = (int)   model.ProgramId,
                    CategoryId          = (int)   model.CategoryId,
                    InterviewerId       = (int)   model.InterviewerId,
                    ProgramTypeId       = (int)   model.ProgramTypeId,
                    ProgramDescription  =         model.ProgramDescription,
                    ProgramImg          =         model.ProgramImgPath,
                    ProgramName         =         model.ProgramName,
                    ProgramOrder        = (int)   model.ProgramOrder,
                    ProgramStartDate    = ProgramStartDate,
                    ProgramVisible      = (bool)  model.ProgramVisible,
                    CreationDate        =         DateTime.Now,
                    ProgramViews        = (int)model.ProgramViews
                };

                bool result = unitOfWork.Program.Update(_program);

                if (!result)
                    return BadRequest("UPDATE OPERATION FAILED ");

                await unitOfWork.Complete();

                return StatusCode(StatusCodes.Status200OK) ;
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

        #region Delete Program
        //[Authorize]
        [HttpDelete("{ID}")]
        public async Task<ActionResult<Program>> deleteprogramAsync(int ID)
        {
            try
            {
                #region Check Program If Exist or not 
                var ProgramObj = await unitOfWork.Program.FindObjectAsync(ID);
                if (ProgramObj == null)
                    return BadRequest("Program ID Not Found ");
                #endregion

                bool result = await unitOfWork.Program.DeleteObject(ID);
                if (!result)
                    return NotFound("Program Not Exist");
                await unitOfWork.Complete();

                #region Delete image File From Specified Directory 
                helper.DeleteFiles(ProgramObj.ProgramImg);
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

        #region Get Program Filter ForRecently API Key Authentication
        [ApiAuthentication]
        [HttpGet]
        [Route("programfilter")]
        public async Task<ActionResult<RetrieveData<ProgramFilterModelWithTitle>>> programfilterAsync([FromQuery] ProgramFilterInputs inputs)
        {
            try
            {
                #region get  Programs , ProgramTypes 
                var Programs = await unitOfWork.Program.GetObjects(); Programs.ToList();
                var ProgramTypes = await unitOfWork.ProgramType.GetObjects(); ProgramTypes.ToList();
                #endregion

                #region declare list to fetch output 
                List<ProgramFilterModelWithTitle> ProgramFilter = new List<ProgramFilterModelWithTitle>();
                #endregion

                #region Apply Query in db 
                var Result = from program in Programs
                             join programType in ProgramTypes
                             on program.ProgramTypeId equals programType.ProgramTypeId
                             where (program.CategoryId == inputs.CategoryID || inputs.CategoryID is null) &&
                                   (programType.ProgramTypeId == inputs.ProgramTypeID || inputs.ProgramTypeID is null) &&
                                   (program.ProgramName == inputs.ProgramName || inputs.ProgramName is null) &&
                                   (program.ProgramStartDate >= inputs.DateFrom || inputs.DateFrom is null) &&
                                   (program.ProgramStartDate <= inputs.DateTo || inputs.DateTo is null)
                             select new
                             {
                                 program.ProgramId,
                                 program.ProgramDescription,
                                 program.ProgramName,
                                 program.ProgramImg,
                                 program.CreationDate,
                                 program.CategoryId,
                                 program.ProgramStartDate,
                                 program.InterviewerId,
                                 program.ProgramOrder,
                                 program.ProgramTypeId,
                                 programType.ProgramTypeTitle
                             };
                #endregion

                #region Fill output List from returned list from db
                foreach (var item in Result)
                {
                    ProgramFilterModelWithTitle model = new ProgramFilterModelWithTitle()
                    {
                        CategoryId = item.CategoryId,
                        ProgramTypeTitle = item.ProgramTypeTitle,
                        ProgramName = item.ProgramName,
                        CreationDate = item.CreationDate,
                        InterviewerId = item.InterviewerId,
                        ProgramDescription = item.ProgramDescription,
                        ProgramId = item.ProgramId,
                        ProgramImg =helper.LivePathImages+ item.ProgramImg,
                        ProgramOrder = item.ProgramOrder,
                        ProgramStartDate = item.ProgramStartDate,
                        ProgramTypeId = item.ProgramTypeId
                    };
                    ProgramFilter.Add(model);
                }
                #endregion

                RetrieveData<ProgramFilterModelWithTitle> Collection = new RetrieveData<ProgramFilterModelWithTitle>();
                Collection.Url = helper.LivePathImages;
                Collection.DataList = ProgramFilter;

                return Collection;
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region most viewed programs API Key Authentication
        [ApiAuthentication]
        [HttpGet]
        [Route("mostviewedprograms")]
        public async Task<ActionResult<RetrieveData<ProgramsMostViewsModel>>> mostviewedprograms([FromQuery] MostViewProgramInput input)
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
                var SeasonList = await  unitOfWork.Season.GetObjects(); SeasonList.ToList();
                #endregion

                #region Iterate on Seasons to calculate Top Views 
                foreach (var SeasonItem in SeasonList)
                {
                    #region Fetch All Episodes related with Season ID
                    var EpisodesLIst = await unitOfWork.Episode.GetObjects(x => x.SessionId == SeasonItem.SessionId); EpisodesLIst.ToList();
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
                List<(Seasons,int)> seasons = new List<(Seasons,int)>();
                int count = SeasonSorted.Count;
                foreach (var item in SeasonSorted)
                {
                    var _season =await  unitOfWork.Season.FindObjectAsync(item.Item1);
                    seasons.Add(((Seasons, int))(_season,item.Item2));
                }
                #endregion

                #region Fetch All program from db
                var ProgramList = await unitOfWork.Program.GetObjects(); ProgramList.ToList();
                #endregion

                #region Iterate on program to calculate Top Views 
                foreach (var ProgramItem in ProgramList)
                {
                    int SumSeasonViews = 0;
                    foreach (var item in seasons)
                    {
                        if(item.Item1.ProgramId == ProgramItem.ProgramId)
                        {
                            SumSeasonViews += item.Item2;
                            //seasons.Remove((item.Item1, item.Item2));
                        }
                    }
                    ProgramSorted.Add((ProgramItem, SumSeasonViews));
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
                                ProgramImg = item.Item1.ProgramImg
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
                            CategoryId = item.Item1.CategoryId,
                            InterviewerId = item.Item1.InterviewerId,
                            ProgramDescription = item.Item1.ProgramDescription,
                            ProgramName = item.Item1.ProgramName,
                            ProgramOrder = item.Item1.ProgramOrder,
                            ProgramStartDate = item.Item1.ProgramStartDate,
                            ProgramTypeId = item.Item1.ProgramTypeId,
                            ProgramVisible = (bool)item.Item1.ProgramVisible,
                            Views = item.Item2,
                            ProgramImg =  item.Item1.ProgramImg
                        };
                        mostViewsModels.Add(model);
                    }
                }
                #endregion

                RetrieveData<ProgramsMostViewsModel> Collection = new RetrieveData<ProgramsMostViewsModel>();
                Collection.Url = helper.LivePathImages;
                Collection.DataList = mostViewsModels ;

                return Collection;
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Function To Update Order
        /// <summary>
        /// Update Category Order  
        /// </summary>
        /// <param name="Entity to Update"></param>
        /// <param name="Old Order"></param>
        /// <returns>
        /// update Orders between rows("Old Order and New Order")
        /// </returns>
        private async Task<ActionResult> UpdateOrder(ProgramInsertModel model, int _OldOrder)
        {
            try
            {
                #region check category id exist
                var _EntityObj = await unitOfWork.Program.FindObjectAsync( (int)model.ProgramId);
                if (_EntityObj == null)
                    return NotFound("Entity ID Not Found");
                #endregion
                var MaxOrder =await unitOfWork.category.GetObjects(); 
                int _MaxOrder = MaxOrder.Count();
                string messege = "Order Update Cannot be Greater than Max Order " + _MaxOrder;
                if (model.ProgramOrder > _MaxOrder)
                    return BadRequest(messege);

                if (model.ProgramOrder <= 0)
                    model.ProgramOrder = _EntityObj.ProgramOrder;

                #region Handle Order Update
                //Get Max Order
                var __Entities =await unitOfWork.Program.GetObjects(); __Entities.OrderBy(Obj => Obj.ProgramOrder).ToList();
                //int _MaxOrder = _categories.Count();

                //var category = unitOfWork.Program.GetObjects(obj => obj.ProgramId == (int)model.ProgramId).AsNoTracking<Program>().FirstOrDefault();
                int NewOrder = (int)model.ProgramOrder;
                int OldOrder = _OldOrder;

                if (OldOrder < NewOrder)
                {
                    var _SubCategories = __Entities.Where(obj => obj.ProgramOrder > OldOrder && obj.ProgramOrder <= NewOrder).OrderBy(o => o.ProgramOrder).ToList();
                    foreach (var item in _SubCategories)
                    {
                        item.ProgramOrder = item.ProgramOrder - 1;
                        bool _result = unitOfWork.Program.Update(item);
                        if (!_result)
                            return BadRequest("update order operation failed !! ");
                        await unitOfWork.Complete();
                    }
                }
                else if (OldOrder > NewOrder)
                {
                    var _SubCategories = __Entities.Where(obj => obj.ProgramOrder >= NewOrder && obj.ProgramOrder < OldOrder).OrderBy(o => o.ProgramOrder).ToList();
                    foreach (var item in _SubCategories)
                    {
                        item.ProgramOrder = item.ProgramOrder + 1;
                        bool _result = unitOfWork.Program.Update(item);
                        if (!_result)
                            return BadRequest("update order operation failed !! ");
                        await unitOfWork.Complete();
                    }
                }
                else
                {
                    return BadRequest("order Incorrect ");
                }

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
    }
}
