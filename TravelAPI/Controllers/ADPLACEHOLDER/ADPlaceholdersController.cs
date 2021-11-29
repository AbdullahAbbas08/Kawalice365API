using BalarinaAPI.Authentication;
using BalarinaAPI.Core.Model;
using BalarinaAPI.Core.ViewModel;
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

        #region Get All AD Placeholders  => Dashboard
        /// <summary>
        /// Reteive All Data in Placeholders 
        /// </summary>
        /// <returns>
        /// List Of Advertisement Object that Contains  ADPlaceholderID , AdStyleID , AdTargetId , Title , ImagePath , 
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("getallads")]
        public async Task<ActionResult<RetrieveData<PlacementModel>>> getallads()
        {
            try
            {
                RetrieveData<PlacementModel> Collection= new RetrieveData<PlacementModel>();
                
                var _ADPLACEHOLDER = await unitOfWork.ADPLACEHOLDER.GetObjects();
                var Style          = await unitOfWork.ADSTYLES.GetObjects();
                var Target         = await unitOfWork.ADTARGETS.GetObjects();

                var result = (from placement in _ADPLACEHOLDER
                             join style in Style
                             on placement.AdStyleID equals style.ADStyleId
                             join target in Target
                             on placement.AdTargetId equals target.ADTargetID
                             select new
                             {
                                 placement.ADPlaceholderID,
                                 placement.ADPlaceholderCode,
                                 placement.Title,
                                 placement.AdStyleID,
                                 placement.AdTargetId,
                                 placement.ImagePath,
                                 style.ADStyleTitle,
                                 target.ADTargetTitle
                             }).ToList();
                foreach (var item in result)
                {
                    PlacementModel model = new PlacementModel()
                    {
                        ADPlaceholderID = item.ADPlaceholderID,
                        ADPlaceholderCode = item.ADPlaceholderCode,
                        AdStyleID = item.AdStyleID,
                        ADStyleTitle = item.ADStyleTitle,
                        AdTargetId = item.AdTargetId,
                        ADTargetTitle = item.ADTargetTitle,
                        Title = item.Title,
                        ImagePath = item.ImagePath
                    };
                    Collection.DataList.Add(model);
                }
                Collection.Url = helper.LivePathImages ;
                return Collection; 
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
        public async Task<ActionResult<RetrieveData<ADPLACEHOLDER>>> getalladsapikey()
        {
            try
            {
                RetrieveData<ADPLACEHOLDER> Collection = new RetrieveData<ADPLACEHOLDER>();
                var _ADPLACEHOLDER = await unitOfWork.ADPLACEHOLDER.GetObjects();
                Collection.Url = helper.LivePathImages;
                Collection.DataList = _ADPLACEHOLDER.ToList();
                return Collection;
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
        //[ApiAuthentication]
        [HttpGet]
        [Route("findplacement")]
        public async Task<ActionResult<RetrieveData<ADPLACEHOLDER>>> findplacement(int ID)
        {
            try 
            {
               
                RetrieveData<ADPLACEHOLDER> Collection = new RetrieveData<ADPLACEHOLDER>();
                var _ADPLACEHOLDER = await unitOfWork.ADPLACEHOLDER.FindObjectAsync(ID);
                if (_ADPLACEHOLDER == null)
                    return BadRequest("PLACEHOLDER ID NOT FOUND ");

                Collection.Url = helper.LivePathImages;
                Collection.DataList.Add(_ADPLACEHOLDER);
                return Collection;
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
        [ApiAuthentication]
        [HttpPost]
        [Route("createplaceholder")]
        public async Task<ActionResult<PlaceholderInput>> createplaceholder([FromQuery] PlaceholderInput model)
        {
            try
            {
                 model.Image = HttpContext.Request.Form.Files["PlacementIamge"];


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
                    ADPlaceholderCode = model.ADPlaceholderCode,
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

                return StatusCode(StatusCodes.Status200OK);
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
        [ApiAuthentication]
        [HttpPut]
        [Route("updateplaceholder")]
        public async Task<ActionResult<PlaceholderToUpdate>> updateplaceholder([FromQuery] PlaceholderToUpdate model)
        {
            try
            {
                model.Image = HttpContext.Request.Form.Files["PlacementIamge"];


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
                    var AdStyleID =await unitOfWork.ADSTYLES.FindObjectAsync((int)model.AdStyleID);
                    if (AdStyleID == null)
                        return BadRequest(" Ads Style ID not found ");
                }

                if (model.ADPlaceholderCode == null)
                    model.ADPlaceholderCode = PlaceholderObj.ADPlaceholderCode;

                if (model.AdTargetId == null)
                    model.AdTargetId = PlaceholderObj.AdTargetId;
                else
                {
                    var AdTargetId =await unitOfWork.ADTARGETS.FindObjectAsync((int)model.AdTargetId);
                    if (AdTargetId == null)
                        return BadRequest("Ads Target Id ID not found ");
                }

              
                if (model.Image != null)
                {
                    model.ImagePath = helper.UploadImage(model.Image);
                }
                else
                {
                    model.ImagePath = PlaceholderObj.ImagePath;
                }

                #endregion

                #region Fill ADTARGET object with values to insert
                ADPLACEHOLDER _ADPLACEHOLDER = new ADPLACEHOLDER()
                {
                   ADPlaceholderID = model.ADPlaceholderID,
                   AdStyleID = (int)model.AdStyleID,
                   AdTargetId = (int)model.AdTargetId,
                   Title = model.Title,
                   ADPlaceholderCode = (int)model.ADPlaceholderCode,
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

                return StatusCode(StatusCodes.Status200OK);
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
        [ApiAuthentication] 
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
