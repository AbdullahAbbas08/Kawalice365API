using BalarinaAPI.Authentication;
using BalarinaAPI.Core.Model;
using BalarinaAPI.Core.ViewModel.AdStyles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelAPI.Core;
using TravelAPI.Core.Helper;

namespace BalarinaAPI.Controllers.AdStyles
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdStylesController : ControllerBase
    {
        #region variables
        private readonly IUnitOfWork unitOfWork;
        private readonly Helper helper;
        #endregion

        #region Constructor
        public AdStylesController(IUnitOfWork _unitOfWork, IOptions<Helper> _helper)
        {
            unitOfWork = _unitOfWork;
            this.helper = _helper.Value;
        }
        #endregion

        #region CRUD OPERATIONS

        #region Get All ADTargets Authorize
        /// <summary>
        /// Reteive All Data in ADSTYLES 
        /// </summary>
        /// <returns>
        /// List Of Advertisement Object that Contains ADStyleId , ADStyleTitle , ADWidth and ADHeight
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("getallstyles")]
        public async Task<ActionResult<IEnumerable<ADSTYLES>>> getallstyles()
        {
            try
            {
                var AdStyles  = await unitOfWork.ADSTYLES.GetObjects();
                return AdStyles.ToList();
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Get All ADTargets ApiAuthentication
        /// <summary>
        /// Reteive All Data in ADSTYLES 
        /// </summary>
        /// <returns>
        /// List Of Advertisement Object that Contains ADStyleId , ADStyleTitle , ADWidth and ADHeight
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("getallstylesapikey")]
        public async Task<ActionResult<IEnumerable<ADSTYLES>>> getallstylesapikey()
        {
            try
            {
                var AdStyles = await unitOfWork.ADSTYLES.GetObjects();
                return AdStyles.ToList();
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Find ADTarget By ADTargetID  ApiAuthentication
        /// <summary>
        /// Find ADTarget by ID 
        /// </summary>
        /// <param name="ID">
        /// ADTargetID 
        /// </param>
        /// <returns>
        ///  AdStyle Object that Contains ADStyleId , ADStyleTitle , ADWidth and ADHeight
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("findadstyle")]
        public async Task<ActionResult<ADSTYLES>> findadstyle(int ID)
        {
            try
            {
                var AdStyle = await unitOfWork.ADSTYLES.FindObjectAsync(ID);
                return AdStyle;
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Insert New AD Style 
        /// <summary>
        /// insert new Advertisement Style
        /// </summary>
        /// <param name="model">
        /// ADStyleTitle , ADWidth , ADHeight
        /// </param>
        /// <returns>
        /// status of operation - Created Successfully - or Status500InternalServerError
        /// </returns>
        [ApiAuthentication]
        [HttpPost]
        [Route("createadstyle")]
        public async Task<ActionResult<AdStyleInput>> createadstyle([FromQuery] AdStyleInput model)
        {
            try
            {
                #region Check values of AD style is not null or empty
                if (string.IsNullOrEmpty(model.ADStyleTitle))
                    return BadRequest(" ADs style Title cannot be null or empty");
                #endregion

                #region Fill ADTARGET object with values to insert
                ADSTYLES _ADStyle = new ADSTYLES()
                {
                    ADStyleTitle = model.ADStyleTitle,
                   ADHeight = model.ADHeight,
                   ADWidth = model.ADWidth,
                };
                #endregion

                #region Create Operation
                bool result = await unitOfWork.ADSTYLES.Create(_ADStyle);
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

        #region Update AD Style 
        /// <summary>
        /// Update Advertisement Style
        /// </summary>
        /// <param name="model">
        /// ADStyleTitle , ADWidth , ADHeight
        /// </param>
        /// <returns>
        /// status of operation - Created Successfully - or Status500InternalServerError
        /// </returns>
        [ApiAuthentication]
        [HttpPut]
        [Route("updateadstyle")]
        public async Task<ActionResult<AdStyleToUpdate>> updateadstyle([FromQuery] AdStyleToUpdate model)
        {
            try
            {
                #region Check IF Ad Styles Exist or not 
                var AdStyleObj =await unitOfWork.ADSTYLES.FindObjectAsync(model.ADStyleId);
                if (AdStyleObj == null)
                    return BadRequest("Ad Style ID Not Found ");
                #endregion
                #region Check values of AD style is not null or empty
                if (string.IsNullOrEmpty(model.ADStyleTitle))
                    model.ADStyleTitle = AdStyleObj.ADStyleTitle;

                if(model.ADHeight == null)
                    model.ADHeight = AdStyleObj.ADHeight;

                if (model.ADWidth == null)
                    model.ADWidth = AdStyleObj.ADWidth;
                
                #endregion

                #region Fill ADTARGET object with values to insert
                ADSTYLES _ADStyle = new ADSTYLES()
                {
                    ADStyleId = model.ADStyleId,
                    ADStyleTitle = model.ADStyleTitle,
                    ADHeight = (float)model.ADHeight,
                    ADWidth = (float)model.ADWidth,
                };
                #endregion

                #region Create Operation
                bool result =  unitOfWork.ADSTYLES.Update(_ADStyle);
                #endregion

                #region check if Create Operation successed
                if (!result)
                    return BadRequest("updated Operation Failed");
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

        #region Delete Advertisement Style
        [ApiAuthentication]
        [HttpDelete("{ID}")]
        public async Task<ActionResult<ADSTYLES>> deleteadstyle(int ID)
        {
            try
            {
                #region check if Advertisement Style exist or not
                var ADStyleObj = await unitOfWork.ADSTYLES.FindObjectAsync(ID);
                if (ADStyleObj == null)
                    return BadRequest("AD Style Not Found");
                #endregion

                #region Apply Operation In Db
                bool result = await unitOfWork.ADSTYLES.DeleteObject(ID);
                if (!result)
                    return NotFound("AD Style Not Exist");
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

        #endregion
    }
}
 