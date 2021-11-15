using BalarinaAPI.Authentication;
using BalarinaAPI.Core.Model;
using BalarinaAPI.Core.ViewModel;
using BalarinaAPI.Core.ViewModel.AdvertisementTargets;
using BalarinaAPI.Core.ViewModel.Category;
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

namespace BalarinaAPI.Controllers.ADTARGET
{
    [Route("api/[controller]")]
    [ApiController]
    public class ADTargetsController : ControllerBase
    {
        #region variables
        private readonly IUnitOfWork unitOfWork;
        private readonly Helper helper;
        #endregion

        #region Constructor
        public ADTargetsController(IUnitOfWork _unitOfWork, IOptions<Helper> _helper)
        { 
            unitOfWork = _unitOfWork;
            this.helper = _helper.Value;
        }
        #endregion

        #region CRUD OPERATIONS

        #region Get All ADTargets Authorize
        /// <summary>
        /// Reteive All Data in Advertisements 
        /// </summary>
        /// <returns>
        /// List Of Advertisement Object that Contains ADTargetID , ADTargetTitle , ADTargetType and ItemID
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("getalladtargets")]
        public async Task<ActionResult<IEnumerable<ADTARGETS>>> getalladtargets()
        {
            try
            {
                var AdTarget = await unitOfWork.ADTARGETS.GetObjects();
                return AdTarget.ToList();
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
        /// Reteive All Data in Advertisements 
        /// </summary>
        /// <returns>
        /// List Of Advertisement Object that Contains ADTargetID , ADTargetTitle , ADTargetType and ItemID
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("getalladtargetsapikey")]
        public async Task<ActionResult<IEnumerable<ADTARGETS>>> getalladtargetsapikey()
            {
                try
                {
                    var AdTarget = await unitOfWork.ADTARGETS.GetObjects();
                    return AdTarget.ToList();
                }
                catch(Exception ex)
                {
                    helper.LogError(ex);
                    return StatusCode(StatusCodes.Status500InternalServerError);
                } 
            }
        #endregion

        #region Get All Target Name , ID 
        /// <summary>
        /// Get All categories Name , ID 
        /// </summary>
        /// <returns>
        /// List of Object that Contains 
        /// CategoryId , CategoryName
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("gettarget_id_name")]
        public async Task<ActionResult<List<ListOfNameID<Object_ID_Name>>>> GetTarget_ID_Name()
        {
            try
            {
                var _Objects = await unitOfWork.ADTARGETS.GetObjects(); _Objects.ToList();
      
                List<ListOfNameID<Object_ID_Name>> Collection = new List<ListOfNameID<Object_ID_Name>>();
                foreach (var item in _Objects)
                {
                    ListOfNameID<Object_ID_Name> obj = new ListOfNameID<Object_ID_Name>() { ID = item.ADTargetID, Name = item.ADTargetTitle };
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

        #region Find ADTarget By ADTargetID  ApiAuthentication
        /// <summary>
        /// Find ADTarget by ID 
        /// </summary>
        /// <param name="ID">
        /// ADTargetID 
        /// </param>
        /// <returns>
        ///  Advertisement Object that Contains ADTargetID , ADTargetTitle , ADTargetType and ItemID
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("findadtarget")]
        public async Task<ActionResult<ADTARGETS>> findadtarget(int ID)
        {
            try
            {
                var AdTarget = await unitOfWork.ADTARGETS.FindObjectAsync(ID);
                return AdTarget;
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Insert New Advertisement Targets
        /// <summary>
        /// insert new Advertisement Targets
        /// </summary>
        /// <param name="model">
        /// ADTargetTitle , ADTargetType , ItemID
        /// </param>
        /// <returns>
        /// status of operation - Created Successfully - or Status500InternalServerError
        /// </returns>
        [ApiAuthentication]
        [HttpPost]
        [Route("createadtarget")]
        public async Task<ActionResult<AdvertisementTargetsModel>> createadtarget([FromQuery] AdvertisementTargetsModel model)
        {
            try
            {
                #region Check values of ADTarget is not null or empty
                if (string.IsNullOrEmpty(model.ADTargetTitle))
                    return BadRequest("ADTarget Title cannot be null or empty");

                if (string.IsNullOrEmpty(model.ADTargetType))
                    return BadRequest("ADTarget Target Type cannot be null or empty");

                try
                {
                   if(model.ItemID >0) { }
                }
                catch(Exception ex)
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable);
                }

                #endregion

                #region Fill ADTARGET object with values to insert
                ADTARGETS _ADTARGETS = new ADTARGETS()
                {
                   ADTargetTitle = model.ADTargetTitle,
                   ADTargetType = model.ADTargetType,
                   ItemID = model.ItemID,
                };
                #endregion

                #region Create Operation
                bool result = await unitOfWork.ADTARGETS.Create(_ADTARGETS);
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

        #region Update Advertisement Target
        /// <summary>
        /// Update Advertisement Target
        /// </summary>
        /// <param name="model">
        /// ADTargetTitle , ADTargetType , ItemID
        /// </param>
        /// <returns>
        /// status of operation - Updated Successfully - or Status500InternalServerError
        /// </returns>
        [ApiAuthentication]
        [HttpPut]
        [Route("updateadtarget")]
        public async Task<ActionResult<AdvertisementTargetToUpdate>> updateadtarget([FromQuery] AdvertisementTargetToUpdate model)
        {
            try
            {
                #region Check if target exist or not
                var TargetObj = await unitOfWork.ADTARGETS.FindObjectAsync(model.ADTargetID);
                if (TargetObj == null)
                    return BadRequest("Target Object Not Exist");
                #endregion

                #region Check values of ADTarget is not null or empty
                if (string.IsNullOrEmpty(model.ADTargetTitle))
                    model.ADTargetTitle = TargetObj.ADTargetTitle;

                if (string.IsNullOrEmpty(model.ADTargetType))
                    model.ADTargetType = TargetObj.ADTargetType;

                if (model.ItemID == null)
                    model.ItemID = TargetObj.ItemID;
                #endregion

                #region Fill ADTARGET object with values to insert
                ADTARGETS _ADTARGETS = new ADTARGETS()
                {
                    ADTargetID = model.ADTargetID,
                    ADTargetTitle = model.ADTargetTitle,
                   ADTargetType = model.ADTargetType,
                    ItemID = (int)model.ItemID
                };
                #endregion

                #region Update Operation
                bool result = unitOfWork.ADTARGETS.Update(_ADTARGETS);
                #endregion

                #region check if Create Operation successed
                if (!result)
                    return BadRequest("Update Operation Failed");
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

        #region Delete Advertisement Target
        [ApiAuthentication]
        [HttpDelete("{ID}")]
        public async Task<ActionResult<ProgramType>> deleteadtarget(int ID)
        {
            try
            {
                #region check if Advertisement Target exist or not
                var ADTARGETSObj = await unitOfWork.ADTARGETS.FindObjectAsync(ID);
                if (ADTARGETSObj == null)
                    return BadRequest("ADTARGET Not Found");
                #endregion

                #region Apply Operation In Db
                bool result = await unitOfWork.ADTARGETS.DeleteObject(ID);
                if (!result)
                    return NotFound("ADTARGETS Not Exist");
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
