using BalarinaAPI.Authentication;
using BalarinaAPI.Core.Model;
using BalarinaAPI.Core.ViewModel;
using BalarinaAPI.Core.ViewModel.ADS;
using BalarinaAPI.Core.ViewModel.Category;
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
using BalarinaAPI.Core.ViewModel.Clients;

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
        public ADSController(IUnitOfWork _unitOfWork, IOptions<Helper> _helper, UserManager<ApplicationUser> userManager)
        {
            unitOfWork = _unitOfWork;
            this.helper = _helper.Value;
            _userManager = userManager;
        }
        #endregion

        #region CRUD OPERATIONS

        #region Get All Advertisement 
        /// <summary>
        /// Reteive All Data in Advertisement 
        /// </summary>
        /// <returns>
        /// List Of Advertisement Object that Contains AdId , AdTitle , ImagePath , URL , Views , PlaceHolderID , ClientID , PublishStartDate and PublishEndDate  
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("getallads")]
        public async Task<ActionResult<RetrieveData<AdsClientModel>>> getallads()
        {
            try
            {
                RetrieveData<AdsClientModel> Collection = new RetrieveData<AdsClientModel>();

                var ADSs = await unitOfWork.ADS.GetObjects();
                var _ClientsObject = await unitOfWork.ApplicationUser.GetObjects(); _ClientsObject.ToList();
                var _Placements = await unitOfWork.ADPLACEHOLDER.GetObjects(); _Placements.ToList();
                var Result = (from ads in ADSs
                             join client in _ClientsObject
                             on ads.ClientID equals client.Id
                             join placement in _Placements
                             on ads.PlaceHolderID equals placement.ADPlaceholderID
                             select new
                             {
                                 ads.AdId,
                                 ads.AdTitle,
                                 ads.ClientID,
                                 ads.ImagePath,
                                 ads.PlaceHolderID,
                                 ads.PublishEndDate,
                                 ads.PublishStartDate,
                                 ads.URL,
                                 ads.Views,
                                 client.FirstName,
                                 client.LastName,
                                 placement.ADPlaceholderCode   
                             }).ToList();
                foreach (var item in Result)
                {
                    AdsClientModel model = new AdsClientModel()
                    {
                        AdId = item.AdId,
                        AdTitle = item.AdTitle,
                        ClientID=item.ClientID,
                        ClientName=item.FirstName+' '+item.LastName,
                        ImagePath=item.ImagePath,
                        PlaceHolderID=item.PlaceHolderID,
                        PublishEndDate=item.PublishEndDate,
                        PublishStartDate=item.PublishStartDate,
                        URL=item.URL,
                        Views=item.Views,
                        PlaceHolderCode = item.ADPlaceholderCode
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
        public async Task<ActionResult<RetrieveData<ADS>>> getalladsapikey()
        {
            try
            {
                var ADSs = await unitOfWork.ADS.GetObjects();

                RetrieveData<ADS> Collection = new RetrieveData<ADS>();
                Collection.Url = helper.LivePathImages;
                Collection.DataList = ADSs.ToList();
                return Ok(Collection);
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
        public async Task<ActionResult<RetrieveData<ADS>>> getalladsbyplaceid(int Code)
        {
            try
            {
                //var Collection = new Dictionary<string, ADS>(); 
                var ADPLACEHOLDERs = await unitOfWork.ADPLACEHOLDER.GetObjects(x => x.ADPlaceholderCode == Code);
                if (ADPLACEHOLDERs.Count() == 0)
                    return BadRequest("Placement Code Not Found ");

                var PlacementObj = ADPLACEHOLDERs.SingleOrDefault();
                var ADSs = await unitOfWork.ADS.GetOrderedObjects(x => x.PlaceHolderID == PlacementObj.ADPlaceholderID &&
                                                           x.PublishStartDate <= DateTime.Now &&
                                                           x.PublishEndDate >= DateTime.Now, a => a.Views);
                if (ADSs.Count() == 0 )
                    return BadRequest("Placement Code Not Contain Any Advertisements ");

                //Collection.Add(helper.LivePathImages, ADSs.FirstOrDefault());
                RetrieveData<ADS> Collection = new RetrieveData<ADS>();
                Collection.Url = helper.LivePathImages;
                Collection.DataList.Add(ADSs.FirstOrDefault());

                #region Update View Of ADS 
                ADSs.FirstOrDefault().Views += 1;
                unitOfWork.ADS.Update(ADSs.FirstOrDefault());
                await unitOfWork.Complete();
                #endregion

                return Ok(Collection);
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Get All Clients Name , ID 
        /// <summary>
        /// Get All Client Name , ID 
        /// </summary>
        /// <returns>
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("getclient_id_name")]
        public async Task<ActionResult<List<Object_ID_Name_string>>> GetClient_ID_Name() 
        {
            try
            {
                var _ClientsObject = await unitOfWork.ApplicationUser.GetObjects(); _ClientsObject.ToList();

                List<Object_ID_Name_string> Collection = new List<Object_ID_Name_string>();
                foreach (var item in _ClientsObject)
                {
                    Object_ID_Name_string obj = new Object_ID_Name_string() { ID=item.Id , Name=item.FirstName+' '+item.LastName};
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

        #region Get All placement Name , ID 
        /// <summary>
        /// Get All Client Name , ID 
        /// </summary>
        /// <returns>
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("getplacement_id_name")]
        public async Task<ActionResult<List<Object_ID_Name>>> GetPlacement_ID_Name()
        {
            try
            {
                var _placementObject = await unitOfWork.ADPLACEHOLDER.GetObjects(); _placementObject.ToList();

                List<Object_ID_Name> Collection = new List<Object_ID_Name>();

                foreach (var item in _placementObject)
                {
                    Object_ID_Name obj = new Object_ID_Name() {ID = item.ADPlaceholderID,Name=item.ADPlaceholderCode.ToString() };
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
        public async Task<ActionResult<RetrieveData<ADS>>> findads(int ID)
        {
            try
            {
                var _ADS = await unitOfWork.ADS.FindObjectAsync(ID);

                if (_ADS == null)
                    return BadRequest("ADS ID NOT FOUND ");

                RetrieveData<ADS> Collection = new RetrieveData<ADS>();
                Collection.Url = helper.LivePathImages;
                Collection.DataList.Add(_ADS);
                return Ok(Collection);
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

                var placeholderID = await unitOfWork.ADPLACEHOLDER.FindObjectAsync(model.PlaceHolderID);
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
                var ADSObj = await unitOfWork.ADS.FindObjectAsync(model.AdId);
                if (ADSObj == null)
                    return BadRequest("ADS ID NOT FOUND");
                #endregion

                #region Check values of Advertisement is not null or empty

                if (string.IsNullOrEmpty(model.AdTitle))
                    model.AdTitle = ADSObj.AdTitle;

                if (model.URL == null)
                    model.URL = ADSObj.URL;

                if (model.PlaceHolderID == null)
                   model.PlaceHolderID = ADSObj.PlaceHolderID;



                var placeholderID = unitOfWork.ADPLACEHOLDER.FindObjectAsync((int)model.PlaceHolderID);
                if (placeholderID == null)
                    return BadRequest(" placement id not found ");

                var user = await _userManager.FindByIdAsync(model.ClientID);

                if (user is null)
                    model.ClientID = ADSObj.ClientID;

                if (model.PublishStartDate == null)
                    model.PublishStartDate = ADSObj.PublishStartDate;

                if (model.PublishEndDate == null)
                    model.PublishEndDate = ADSObj.PublishEndDate;

                if (!DateTime.TryParse((model.PublishStartDate).ToString(), out _))
                    return BadRequest("Publish Start Date Not Invalid");

                if (!DateTime.TryParse((model.PublishEndDate).ToString(), out _))
                    return BadRequest("Publish End Date Not Invalid");

                if(model.Views == null)
                    model.Views = ADSObj.Views;
               

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
                    ImagePath = model.ImagePath,
                    ClientID = model.ClientID,
                    PlaceHolderID = (int)model.PlaceHolderID,
                    PublishStartDate = (DateTime) model.PublishStartDate,
                    PublishEndDate = (DateTime) model.PublishEndDate,
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
