using BalarinaAPI.Authentication;
using BalarinaAPI.Core.Model;
using BalarinaAPI.Core.ViewModel.ADS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelAPI.Core;
using TravelAPI.Core.Helper;
using TravelAPI.Models;

namespace BalarinaAPI.Controllers.Advertisement 
{
    [Route("api/[controller]")]
    [ApiController]
    public class ADSController : ControllerBase
    {
        #region variables
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly Helper helper;
        #endregion

        #region Constructor
        public ADSController(IUnitOfWork _unitOfWork, IOptions<Helper> _helper , UserManager<ApplicationUser> userManager)
        {
            unitOfWork = _unitOfWork;
            this.helper = _helper.Value;
            _userManager = userManager;
        }
        #endregion

        #region CRUD OPERATIONS

        #region Get All Advertisement Authorize
        /// <summary>
        /// Reteive All Data in Advertisement 
        /// </summary>
        /// <returns>
        /// List Of Advertisement Object that Contains AdId , AdTitle , ImagePath , URL , Views , PlaceHolderID , ClientID , PublishStartDate and PublishEndDate  
        /// </returns>
        [Authorize]
        [HttpGet]
        [Route("getallads")]
        public async Task<ActionResult<IEnumerable<ADS>>> getallads()
        {
            try
            {
                var ADSs = await unitOfWork.ADS.GetObjects();
                foreach (var item in ADSs)
                {
                    item.ImagePath = helper.LivePathImages + item.ImagePath;
                }
                return ADSs.ToList();
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Get All Advertisement ApiAuthentication
        /// <summary>
        /// Reteive All Data in Advertisement 
        /// </summary>
        /// <returns>
        /// List Of Advertisement Object that Contains AdId , AdTitle , ImagePath , URL , Views , PlaceHolderID , ClientID , PublishStartDate and PublishEndDate  
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("getalladsapikey")]
        public async Task<ActionResult<IEnumerable<ADS>>> getalladsapikey()
        {
            try
            {
                var ADSs = await unitOfWork.ADS.GetObjects();
                foreach (var item in ADSs)
                {
                    item.ImagePath = helper.LivePathImages + item.ImagePath;
                }
                return ADSs.ToList();
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Get All Advertisement Related with PlaceholderID
        /// <summary>
        /// Reteive All Data in Advertisement related with PlaceholderID
        /// </summary>
        /// <returns>
        /// List Of Advertisement Object that Contains AdId , AdTitle , ImagePath , URL , Views , PlaceHolderID , ClientID , PublishStartDate and PublishEndDate  
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("getalladsbyplaceid")]
        public async Task<ActionResult<ADS>> getalladsbyplaceid(int ID)
        {
            try
            { 
                var ADSs = await unitOfWork.ADS.GetObjects(x=>x.PlaceHolderID == ID);

                var Result = from  _ADS in ADSs
                             where  _ADS.PublishStartDate <= DateTime.Now  && 
                                    _ADS.PublishEndDate >= DateTime.Now
                             select new { _ADS.AdId,_ADS.AdTitle, _ADS, _ADS.ImagePath, _ADS.URL, _ADS.Views, _ADS.ClientID};
                foreach (var item in ADSs)
                {
                    item.ImagePath = helper.LivePathImages + item.ImagePath;
                }
                var ADSsOrdered = ADSs.OrderBy(x => x.Views).ToList();

                #region Update View Of ADS 
                ADSsOrdered[0].Views += 1;
                unitOfWork.ADS.Update(ADSsOrdered[0]);
                await unitOfWork.Complete();
                #endregion
              

                return ADSsOrdered[0];
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Find Advertisement By ID ApiAuthentication
        /// <summary>
        /// Reteive All Data in Advertisement 
        /// </summary>
        /// <returns>
        ///  Advertisement Object that Contains AdId , AdTitle , ImagePath , URL , Views , PlaceHolderID , ClientID , PublishStartDate and PublishEndDate  
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("findads")]
        public async Task<ActionResult<ADS>> findads(int ID)
        {
            try
            {
                var _ADS = await unitOfWork.ADS.FindObjectAsync(ID);

                if (_ADS == null)
                    return BadRequest("ADS ID NOT FOUND ");

                _ADS.ImagePath = helper.LivePathImages + _ADS.ImagePath;

                return _ADS;
            }  
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Insert New Advertisement
        /// <summary>
        /// insert new Advertisement 
        /// </summary>
        /// <param name="model">
        /// AdTitle , Image , URL , Views , PlaceHolderID , ClientID , PublishStartDate AND PublishEndDate
        /// </param>
        /// <returns>
        /// status of operation - Created Successfully - or Status500InternalServerError
        /// </returns>
        [Authorize]
        [HttpPost] 
        [Route("createads")]
        public async Task<ActionResult<ADSInput>> createads([FromQuery] ADSInput model)
        {
            try
            {

                #region Check values of Advertisement is not null or empty

                if (string.IsNullOrEmpty(model.AdTitle))
                    return BadRequest(" AD Title cannot be null or empty");

                if (model.URL == null)
                    return BadRequest("AD URL EMPTY !! ");

                var placeholderID = unitOfWork.ADPLACEHOLDER.FindObjectAsync(model.PlaceHolderID);
                if (placeholderID == null)
                    return BadRequest("placeholderID not found ");

                var user = await _userManager.FindByIdAsync(model.ClientID);

                if (user is null)
                    return BadRequest("Client ID NOt Found ");

                if (!DateTime.TryParse((model.PublishStartDate).ToString(), out _))
                    return BadRequest("Publish Start Date Not Invalid");

                if (!DateTime.TryParse((model.PublishEndDate).ToString(), out _))
                    return BadRequest("Publish End Date Not Invalid");

                if (model.Image == null)
                    return BadRequest("ADS Image NOT Valid");
                #endregion

                #region Fill ADTARGET object with values to insert
                ADS _ADVS = new ADS()
                {
                   AdTitle = model.AdTitle,
                   ImagePath = helper.UploadImage(model.Image),
                   ClientID = model.ClientID,
                   PlaceHolderID = model.PlaceHolderID,
                   PublishStartDate = model.PublishStartDate,
                   PublishEndDate = model.PublishEndDate,
                   URL = model.URL,
                   Views = model.Views,       
                };
                #endregion

                #region Create Operation
                bool result = await unitOfWork.ADS.Create(_ADVS);
                #endregion

                #region check if Create Operation successed
                if (!result)
                    return BadRequest("Create Operation Failed");
                #endregion

                #region save changes in db
                await unitOfWork.Complete();
                #endregion

                return Ok(" ADS Created Successfully ");
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Update Advertisement
        /// <summary>
        /// insert new Advertisement 
        /// </summary>
        /// <param name="model">
        /// AdId , AdTitle , Image , URL , Views , PlaceHolderID , ClientID , PublishStartDate AND PublishEndDate
        /// </param>
        /// <returns>
        /// status of operation - Created Successfully - or Status500InternalServerError
        /// </returns>
        [Authorize]
        [HttpPut]
        [Route("updateads")]
        public async Task<ActionResult<ADSToUpdate>> updateads([FromQuery] ADSToUpdate model)
        {
            try
            {
                #region Check if ADS ID Exist or not 
                var ADSObj =await unitOfWork.ADS.FindObjectAsync(model.AdId);
                if (ADSObj == null)
                    return BadRequest("ADS ID NOT FOUND");
                #endregion

                #region Check values of Advertisement is not null or empty

                if (string.IsNullOrEmpty(model.AdTitle))
                    model.AdTitle = ADSObj.AdTitle;

                if (model.URL == null)
                    model.URL = ADSObj.URL;

                var placeholderID = unitOfWork.ADPLACEHOLDER.FindObjectAsync((int)model.PlaceHolderID);
                if (placeholderID == null)
                    model.PlaceHolderID = ADSObj.PlaceHolderID;

                var user = await _userManager.FindByIdAsync(model.ClientID);

                if (user is null)
                    model.ClientID = ADSObj.ClientID;

                if (!DateTime.TryParse((model.PublishStartDate).ToString(), out _))
                    return BadRequest("Publish Start Date Not Invalid");

                if (!DateTime.TryParse((model.PublishEndDate).ToString(), out _))
                    return BadRequest("Publish End Date Not Invalid");

                if (model.PublishStartDate == null)
                    model.PublishStartDate = ADSObj.PublishStartDate;

                if (model.PublishEndDate == null)
                    model.PublishEndDate = ADSObj.PublishEndDate;

                if (model.ImagePath == null && model.Image == null)
                {
                    model.ImagePath = ADSObj.ImagePath;
                }
                if (model.ImagePath == null)
                {
                    model.ImagePath = helper.UploadImage(model.Image);
                }
                #endregion

                #region Fill ADTARGET object with values to insert
                ADS _ADVS = new ADS()
                {
                    AdId = model.AdId,
                    AdTitle = model.AdTitle,
                    ImagePath = helper.UploadImage(model.Image),
                    ClientID = model.ClientID,
                    PlaceHolderID = (int)model.PlaceHolderID,
                    PublishStartDate =(DateTime)model.PublishStartDate,
                    PublishEndDate = (DateTime)model.PublishEndDate,
                    URL = model.URL,
                    Views = (int)model.Views,
                };
                #endregion

                #region Create Operation
                bool result = unitOfWork.ADS.Update(_ADVS);
                #endregion

                #region check if Create Operation successed
                if (!result)
                    return BadRequest("Update Operation Failed");
                #endregion

                #region save changes in db
                await unitOfWork.Complete();
                #endregion

                return Ok(" ADS Updated Successfully ");
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Delete Advertisement 
        [Authorize]
        [HttpDelete("{ID}")]
        public async Task<ActionResult<ADSTYLES>> deleteads(int ID)
        {
            try
            {
                #region check if Advertisement  exist or not
                var ADSObj = await unitOfWork.ADS.FindObjectAsync(ID);
                if (ADSObj == null)
                    return BadRequest("ADS Not Found");
                #endregion

                #region Apply Operation In Db
                bool result = await unitOfWork.ADS.DeleteObject(ID);
                if (!result)
                    return NotFound("ADS Not Exist");
                #endregion

                #region Delete ADS Image
                helper.DeleteFiles(ADSObj.ImagePath);
                #endregion

                await unitOfWork.Complete();

                return Ok(" ADS  Obj deleted successfully ");
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
