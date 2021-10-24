using BalarinaAPI.Authentication;
using BalarinaAPI.Core.Model;
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
        public async Task<ActionResult<List<Seasons>>> getallseasonsapikeyAsync()
        {
            try
            {
                //Get All Programs 
                var ResultSeasons = await unitOfWork.Season.GetObjects(); ResultSeasons.ToList();  
                return Ok(ResultSeasons);
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
            }
        }
        #endregion

        #region Get All Season
        [Authorize]
        [HttpGet]
        [Route("getallseasons")] 
        public async Task<ActionResult<List<Seasons>>> getallseasonsAsync()
        {
            try
            {
                //Get All Programs 
                var ResultSeasons = await unitOfWork.Season.GetObjects(); ResultSeasons.ToList();
                return Ok(ResultSeasons);
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
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
                unitOfWork.Complete();

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

        #region Insert New Season 
        [Authorize]
        [HttpPost]
        [Route("createseason")]
        public async Task<ActionResult<SeasonInput>> createseasonAsync([FromQuery] SeasonInput  model )
        {
            try
            {
                var programID = unitOfWork.Program.FindObjectAsync(model.ProgramId);
                if (programID == null)
                    return BadRequest("program ID Not Found ");

                if (string.IsNullOrEmpty(model.SessionTitle))
                    return BadRequest(" Season title cannot be null or empty");

                Seasons _season = new Seasons()
                {
                     SessionTitle = model.SessionTitle,
                     ProgramId = model.ProgramId,
                     CreationDate = DateTime.Now
                };

                bool result = await unitOfWork.Season.Create(_season);

                if (!result)
                    return BadRequest("Create Operation Failed");

                unitOfWork.Complete();

                return Ok("season Inserted Successfully ");
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Edit Season
        [Authorize]
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
                  ProgramId = model.ProgramId,
                  SessionTitle = model.SessionTitle,
                };

                bool result = unitOfWork.Season.Update(_season);

                if (!result)
                    return BadRequest("UPDATE OPERATION FAILED ");

                unitOfWork.Complete();

                return Ok("Season Update Successfully ");
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
        [Authorize]
        [HttpDelete("{ID}")]
        public async Task<ActionResult<Seasons>> deleteseasonAsync(int ID)
        {
            try
            {
                bool result = await unitOfWork.Season.DeleteObject(ID);
                if (!result)
                    return BadRequest("season Not Exist");
                unitOfWork.Complete();

                return Ok("season Deleted Successfully ");
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
 