﻿using BalarinaAPI.Authentication;
using BalarinaAPI.Core.Model;
using BalarinaAPI.Core.ViewModel.ADPLACEHOLDER;
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

namespace BalarinaAPI.Controllers.ADPLACEHOLDERS 
{
    [Route("api/[controller]")]
    [ApiController]
    public class ADPlaceholdersController : ControllerBase
    {
        #region variables
        private readonly IUnitOfWork unitOfWork;
        private readonly Helper helper;
        #endregion

        #region Constructor
        public ADPlaceholdersController(IUnitOfWork _unitOfWork, IOptions<Helper> _helper)
        {
            unitOfWork = _unitOfWork;
            this.helper = _helper.Value;
        }
        #endregion

        #region CRUD OPERATIONS

        #region Get All AD Placeholders  Authorize
        /// <summary>
        /// Reteive All Data in Placeholders 
        /// </summary>
        /// <returns>
        /// List Of Advertisement Object that Contains  ADPlaceholderID , AdStyleID , AdTargetId , Title , ImagePath , 
        /// </returns>
        [Authorize]
        [HttpGet]
        [Route("getallads")]
        public async Task<ActionResult<IEnumerable<ADPLACEHOLDER>>> getallads()
        {
            try
            {
                var _ADPLACEHOLDER = await unitOfWork.ADPLACEHOLDER.GetObjects();

                foreach (var item in _ADPLACEHOLDER)
                {
                    item.ImagePath = helper.LivePathImages + item.ImagePath;
                }

                return _ADPLACEHOLDER.ToList(); 
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Get All AD Placeholders  ApiAuthentication
        /// <summary>
        /// Reteive All Data in Placeholders 
        /// </summary>
        /// <returns>
        /// List Of Advertisement Object that Contains  ADPlaceholderID , AdStyleID , AdTargetId , Title , ImagePath , 
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("getalladsapikey")]
        public async Task<ActionResult<IEnumerable<ADPLACEHOLDER>>> getalladsapikey()
        {
            try
            {
                var _ADPLACEHOLDER = await unitOfWork.ADPLACEHOLDER.GetObjects();

                foreach (var item in _ADPLACEHOLDER)
                {
                    item.ImagePath = helper.LivePathImages + item.ImagePath;
                }

                return _ADPLACEHOLDER.ToList();
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Find AD Placeholder ApiAuthentication
        /// <summary>
        /// Find AD Placeholder 
        /// </summary>
        /// <returns>
        ///  Advertisement Object that Contains  ADPlaceholderID , AdStyleID , AdTargetId , Title AND ImagePath 
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("findads")]
        public async Task<ActionResult<ADPLACEHOLDER>> findads(int ID)
        {
            try 
            {
                var _ADPLACEHOLDER = await unitOfWork.ADPLACEHOLDER.FindObjectAsync(ID);

                if (_ADPLACEHOLDER == null)
                    return BadRequest("ADS PLACEHOLDER ID NOT FOUND ");

                _ADPLACEHOLDER.ImagePath = helper.LivePathImages + _ADPLACEHOLDER.ImagePath;

                return _ADPLACEHOLDER;
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Insert New AD Placeholder
        /// <summary>
        /// insert new Placeholder 
        /// </summary>
        /// <param name="model">
        ///  Advertisement Object that Contains  AdStyleID , AdTargetId , Title AND ImagePath 
        /// </param>
        /// <returns>
        /// status of operation - Created Successfully - or Status500InternalServerError
        /// </returns>
        [Authorize]
        [HttpPost]
        [Route("createplaceholder")]
        public async Task<ActionResult<PlaceholderInput>> createplaceholder([FromQuery] PlaceholderInput model)
        {
            try
            {

                #region Check values of Advertisement is not null or empty

                if (string.IsNullOrEmpty(model.Title))
                    return BadRequest(" Placeholder Title cannot be null or empty");

                var AdStyleID = unitOfWork.ADPLACEHOLDER.FindObjectAsync(model.AdStyleID);
                if (AdStyleID == null)
                    return BadRequest(" Ads Style ID not found ");
                
                var AdTargetId = unitOfWork.ADPLACEHOLDER.FindObjectAsync(model.AdTargetId);
                if (AdTargetId == null)
                    return BadRequest("Ads Target Id ID not found ");
        
                if (model.Image == null)
                    return BadRequest("ADS Image NOT Valid");
                #endregion

                #region Fill ADTARGET object with values to insert
                ADPLACEHOLDER _ADPLACEHOLDER = new ADPLACEHOLDER()
                {
                    AdStyleID = model.AdStyleID,
                    AdTargetId = model.AdTargetId,
                    Title = model.Title,
                    ImagePath = helper.UploadImage(model.Image)
                };
                #endregion

                #region Create Operation
                bool result = await unitOfWork.ADPLACEHOLDER.Create(_ADPLACEHOLDER);
                #endregion

                #region check if Create Operation successed
                if (!result)
                    return BadRequest("Create Operation Failed");
                #endregion

                #region save changes in db
                await unitOfWork.Complete();
                #endregion

                return Ok(" ADS PLACEHOLDER Created Successfully ");
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region update AD Placeholder
        /// <summary>
        /// update AD Placeholder
        /// </summary>
        /// <param name="model">
        ///  Placeholder Object that Contains ADPlaceholderID ,  AdStyleID , AdTargetId , Title AND ImagePath 
        /// </param>
        /// <returns>
        /// status of operation - Updated Successfully - or Status500InternalServerError
        /// </returns>
        [Authorize]
        [HttpPut]
        [Route("updateplaceholder")]
        public async Task<ActionResult<PlaceholderToUpdate>> updateplaceholder([FromQuery] PlaceholderToUpdate model)
        {
            try
            {
                #region Check If Placeholder id Exist or not
                var PlaceholderObj = await unitOfWork.ADPLACEHOLDER.FindObjectAsync(model.ADPlaceholderID);
                if (PlaceholderObj == null)
                    return BadRequest("Placeholder ID not found ");
                #endregion
                #region Check values of Advertisement is not null or empty

                if (string.IsNullOrEmpty(model.Title))
                    model.Title = PlaceholderObj.Title;

                if(model.AdStyleID == null)
                    model.AdStyleID = PlaceholderObj.AdStyleID;
                else
                {
                    var AdStyleID = unitOfWork.ADPLACEHOLDER.FindObjectAsync((int)model.AdStyleID);
                    if (AdStyleID == null)
                        return BadRequest(" Ads Style ID not found ");
                }

                if (model.AdTargetId == null)
                    model.AdTargetId = PlaceholderObj.AdTargetId;
                else
                {
                    var AdTargetId = unitOfWork.ADPLACEHOLDER.FindObjectAsync((int)model.AdTargetId);
                    if (AdTargetId == null)
                        return BadRequest("Ads Target Id ID not found ");
                }

                if (model.ImagePath == null && model.Image == null)
                {
                    model.ImagePath = PlaceholderObj.ImagePath;
                }
                if (model.ImagePath == null)
                {
                    model.ImagePath = helper.UploadImage(model.Image);
                }

                #endregion

                #region Fill ADTARGET object with values to insert
                ADPLACEHOLDER _ADPLACEHOLDER = new ADPLACEHOLDER()
                {
                   ADPlaceholderID = model.ADPlaceholderID,
                   AdStyleID = (int)model.AdStyleID,
                   AdTargetId = (int)model.AdTargetId,
                   Title = model.Title,
                   ImagePath = model.ImagePath
                };
                #endregion

                #region Create Operation
                bool result =  unitOfWork.ADPLACEHOLDER.Update(_ADPLACEHOLDER);
                #endregion

                #region check if Create Operation successed
                if (!result)
                    return BadRequest("Update Operation Failed");
                #endregion

                #region save changes in db
                await unitOfWork.Complete();
                #endregion

                return Ok(" ADS PLACEHOLDER Updated Successfully ");
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Delete Placeholder 
        /// <summary>
        /// Delete Placeholder By ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns>
        /// status of operation - Deleted Successfully - or Status500InternalServerError
        /// </returns>
        [Authorize] 
        [HttpDelete("{ID}")]
        public async Task<ActionResult<ADSTYLES>> deleteplaceholder(int ID)
        {
            try
            {
                #region check if placeholder ID exist or not
                var PlaceholderObj = await unitOfWork.ADPLACEHOLDER.FindObjectAsync(ID);
                if (PlaceholderObj == null)
                    return BadRequest("Placeholder Not Found");
                #endregion

                #region Apply Operation In Db
                bool result = await unitOfWork.ADPLACEHOLDER.DeleteObject(ID);
                if (!result)
                    return NotFound("ADS Not Exist");
                #endregion

                #region Delete ADS Image
                helper.DeleteFiles(PlaceholderObj.ImagePath);
                #endregion

                await unitOfWork.Complete();

                return Ok(" Placeholder Obj deleted successfully ");
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
