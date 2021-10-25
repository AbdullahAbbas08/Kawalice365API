using BalarinaAPI.Authentication;
using BalarinaAPI.Core.Model;
using BalarinaAPI.Core.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TravelAPI.Core;
using TravelAPI.Core.Helper;

namespace BalarinaAPI.Controllers
{
    //[Authorize(AuthenticationSchemes ="Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class categoriesController : ControllerBase
    {
        #region variables
        private readonly IUnitOfWork unitOfWork;
        private readonly Helper helper;
        private readonly ILogger<categoriesController> _logger;
        #endregion

        #region Constructor
        public categoriesController(IUnitOfWork _unitOfWork, IOptions<Helper> _helper , ILogger<categoriesController> logger)
        {
            unitOfWork = _unitOfWork;
            this.helper = _helper.Value;
            _logger = logger;
            _logger.LogDebug(1, "NLog injected into categories Controller");
        }

        #endregion

        #region CREUD OPERATION

        #region Get All Categories with API Key
        /// <summary>
        /// get all categories with API Key
        /// </summary>
        /// <returns>
        /// List Of Category that contain CategoryId,CategoryTitle,CreationDate,CategoryDescription,CategoryVisible,CategoryOrder,CategoryViews
        /// and CategoryImg concatenating with LivePathImages
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("getallcategorieswithapikey")]
        public async Task<ActionResult<List<Category>>> getallcategorieswithapikey()
        {
            try
            {
                var ResultCategories = await unitOfWork.category.GetObjects();
                ResultCategories.OrderBy(x=>x.CategoryOrder).ToList();

                foreach (var item in ResultCategories)
                {
                    item.CategoryImg = helper.LivePathImages + item.CategoryImg;
                }
                return Ok(ResultCategories);
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
            }
        }
        #endregion

        #region Get All Programs Related With Category ID 
        /// <summary>
        /// Get All Programs Related With Category ID
        /// </summary>
        /// <param name="CategoryID"></param>
        /// <returns>
        /// List of Object that Contains 
        /// CategoryId , InterviewerId , ProgramDescription , ProgramId , ProgramImg , ProgramName
        /// ProgramOrder , ProgramStartDate , ProgramTypeId , ProgramVisible , CreationDate
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("getallprogramsbycatidapikey")]
        public async Task<ActionResult<List<ProgramFilterModel>>> getallprogramsbycatidapikeyAsync(int CategoryID) 
        {
            //check If Category ID If Exist
            var _CategoryID = unitOfWork.category.FindObjectAsync(CategoryID);
            if (_CategoryID == null)
                return BadRequest("Category ID Not Exist !!");

            // Create ProgramsList to return It
            List<ProgramFilterModel> _programsList = new List<ProgramFilterModel>();
            try
            {
                //Get All Programs 
                var ResultPrograms = await unitOfWork.Program.GetObjects(x => x.CategoryId == CategoryID); ResultPrograms.ToList();
                #region Fill ProgramsList and Handle Image Path For all Program
                foreach (var item in ResultPrograms)
                {
                    // Create Category Object
                    ProgramFilterModel _program = new ProgramFilterModel()
                    {
                        CategoryId          = item.CategoryId,
                        InterviewerId       = item.InterviewerId,
                        ProgramDescription  = item.ProgramDescription,
                        ProgramId           = item.ProgramId,
                        ProgramImg          = helper.LivePathImages+item.ProgramImg,
                        ProgramName         = item.ProgramName,
                        ProgramOrder        = item.ProgramOrder,
                        ProgramStartDate    = item.ProgramStartDate,
                        ProgramTypeId       = item.ProgramTypeId,
                        ProgramVisible      = (bool)item.ProgramVisible,
                        CreationDate        = item.CreationDate
                    };
                    // Finally Add It Into Programs List
                    _programsList.Add(_program);
                }
                #endregion
                return _programsList;
            }
            catch (Exception ex)
            {
                // Log error in db
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region Get All Categories
        /// <summary>
        /// get all categories with Bearer Toacken
        /// </summary>
        /// <returns>
        /// List Of Category that contain CategoryId,CategoryTitle,CreationDate,CategoryDescription,CategoryVisible,CategoryOrder,CategoryViews
        /// and CategoryImg concatenating with LivePathImages
        /// </returns>
        [Authorize]
        [HttpGet]
        [Route("getallcategories")]
        public async Task<ActionResult<List<Category>>> getallcategoriesAsync()
        {
            try
            {
                var ResultCategories = await unitOfWork.category.GetObjects();
                ResultCategories.OrderBy(x => x.CategoryOrder).ToList();

                foreach (var item in ResultCategories)
                {
                    item.CategoryImg = helper.LivePathImages + item.CategoryImg;
                }
                return Ok(ResultCategories);
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
            }
        }
        #endregion

        #region Insert New Category 
        /// <summary>
        /// insert new category that have CategoryTitle,CategoryDescription,CategoryVisible,CategoryOrder
        /// and CategoryImg 
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// status of operation - Created Successfully  - or Status500InternalServerError
        /// </returns>
        [Authorize]
        [HttpPost]
        [Route("createcategory")]
        public async Task<ActionResult<CategoryModelInput>> createcategoryAsync([FromQuery] CategoryModelInput model)
        {
            try
            {
                #region Check values of Category is not null or empty
                if (string.IsNullOrEmpty(model.CategoryTitle))
                    return BadRequest("Category Title cannot be null or empty");

                if (string.IsNullOrEmpty(model.CategoryDescription))
                    return BadRequest("Category Description cannot be null or empty");

                if (model.CategoryImg == null)
                    return BadRequest("Category Image cannot be null ");

                if (model.CategoryOrder < 0)
                    return BadRequest("Category Order cannot be less than 0 ");

                if(model.CategoryImg ==null)
                    return BadRequest("Category Image cannot be null ");
                #endregion

                #region Fill Category object with values to insert
                Category _category = new Category()
                {
                    CategoryTitle = model.CategoryTitle,
                    CategoryDescription = model.CategoryDescription,
                    CategoryOrder = model.CategoryOrder,
                    CreationDate = DateTime.Now,
                    CategoryVisible = model.CategoryVisible,
                    CategoryImg = UploadImage(model.CategoryImg)
                };
                #endregion

                #region Create Operation
                bool result = await unitOfWork.category.Create(_category);
                #endregion

                #region check if Create Operation successed
                if (!result)
                    return BadRequest("Create Operation Failed");
                #endregion

                #region save changes in db
                await unitOfWork.Complete();
                #endregion

                return Ok("Category Created Successfully ");
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }
        #endregion

        #region Edit Category
        /// <summary>
        /// update Category ( elements or one element ) 
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// status of operation  -Updated Successfully - or Status500InternalServerError
        /// </returns>
        [Authorize]
        [HttpPut]
        [Route("putcategory")]
        public async Task<ActionResult<CategoryModelInput>> putcategory([FromQuery] CategoryToUpdate model)
        {
            try
             { 
                #region check category id exist
                var _categoryObj = await unitOfWork.category.FindObjectAsync(model.CategoryID);
                
                if (_categoryObj == null)
                    return NotFound("Category ID Not Found");
                #endregion

                #region Check values of Category is not null or empty

                if (string.IsNullOrEmpty(model.CategoryTitle))
                        model.CategoryTitle = _categoryObj.CategoryTitle;
               
                if (string.IsNullOrEmpty(model.CategoryDescription))
                        model.CategoryDescription = _categoryObj.CategoryDescription;

                if (model.CategoryVisible != true || model.CategoryVisible != false)
                    model.CategoryVisible = _categoryObj.CategoryVisible;

                if (model.CategoryImgPath == null && model.CategoryImg == null)
                {
                    model.CategoryImgPath = _categoryObj.CategoryImg;
                    //model.CategoryImgPath = UploadImage(model.CategoryImg);
                }
                if (model.CategoryImgPath == null)
                {
                    model.CategoryImgPath = UploadImage(model.CategoryImg);
                }
                if (model.CategoryViews == null)
                    model.CategoryViews = _categoryObj.CategoryViews;
                #endregion

                #region Handle Order Update 
                    await UpdateOrder(model, _categoryObj.CategoryOrder);
                #endregion

                #region fill category object with values to insert 
                Category _category = new Category()
                {
                    CategoryId = model.CategoryID,
                    CategoryTitle = model.CategoryTitle,
                    CategoryDescription = model.CategoryDescription,
                    CategoryOrder = model.CategoryOrder,
                    CreationDate = DateTime.Now,
                    CategoryVisible = model.CategoryVisible,
                    CategoryImg = model.CategoryImgPath ,
                    CategoryViews = (int)model.CategoryViews
                };
                #endregion

                #region update operation
                bool result = unitOfWork.category.Update(_category);
                #endregion

                #region check operation is updated successed
                if (!result)
                    return BadRequest("Create Operation Failed");
                #endregion

                #region save changes into db
                await unitOfWork.Complete();
                #endregion


                return Ok("Category Updated Successfully ");
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
        private async Task<ActionResult> UpdateOrder(CategoryToUpdate model, int _OldOrder)
        {
            try
            {
                #region check category id exist
                var _categoryObj =await unitOfWork.category.FindObjectAsync(model.CategoryID);
                if (_categoryObj == null)
                    return NotFound("Category ID Not Found");
                #endregion
                var MaxOrder = await unitOfWork.category.GetObjects();
                int _MaxOrder = MaxOrder.Count();
                string messege = "Order Update Cannot be Greater than Max Order " + _MaxOrder;
                if (model.CategoryOrder > _MaxOrder)
                    return BadRequest(messege);

                if (model.CategoryOrder <= 0)
                    model.CategoryOrder = _categoryObj.CategoryOrder;

                #region Handle Order Update
                //Get Max Order
                var _categories = await unitOfWork.category.GetObjects();
                    _categories.OrderBy(Obj => Obj.CategoryOrder).ToList();
                //int _MaxOrder = _categories.Count();

                var category = await unitOfWork.category.FindObjectAsync(model.CategoryID);
                int NewOrder = model.CategoryOrder;
                int OldOrder = _OldOrder;

                if (OldOrder < NewOrder)
                {
                    var _SubCategories = _categories.Where(obj => obj.CategoryOrder > OldOrder && obj.CategoryOrder <= NewOrder).OrderBy(o => o.CategoryOrder).ToList();
                    foreach (var item in _SubCategories)
                    {
                        item.CategoryOrder = item.CategoryOrder - 1;
                        bool _result = unitOfWork.category.Update(item);
                        if (!_result)
                            return BadRequest("update order operation failed !! ");
                        await unitOfWork.Complete();
                    }
                }
                else if(OldOrder > NewOrder)
                {
                    var _SubCategories = _categories.Where(obj => obj.CategoryOrder >= NewOrder && obj.CategoryOrder < OldOrder).OrderBy(o => o.CategoryOrder).ToList();
                    foreach (var item in _SubCategories)
                    {
                        item.CategoryOrder = item.CategoryOrder + 1;
                        bool _result = unitOfWork.category.Update(item);
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
            catch(Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError); 
            }   
        }
        #endregion

        #region Delete Category
        /// <summary>
        /// Delete specified Category by category id
        /// </summary>
        /// <param name="ID"></param>
        /// <returns>
        /// status of operation - Deleted Successfully - or Status500InternalServerError
        /// </returns>
        [Authorize]
        [HttpDelete("{ID}")]
        public async Task<ActionResult<Category>> deletecategoryAsync(int ID)
        {
            try
            {
                #region Check ID If Exist
                        var checkIDIfExist = await unitOfWork.category.FindObjectAsync(ID);
                        if (checkIDIfExist == null)
                            return NotFound("Category ID Not Found");
                #endregion

                #region Delete Operation
                        bool result = await unitOfWork.category.DeleteObject(ID);
                #endregion

                #region check Delete Operation  successed

                        if (!result)
                            return NotFound("Category Not Exist");
                #endregion

                #region Delete image File From Specified Directory 
                    helper.DeleteFiles(checkIDIfExist.CategoryImg);
                #endregion

                #region save changes in db
                await unitOfWork.Complete();
                #endregion

                return Ok("Category ID Deleted Successfully");
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

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
            catch(Exception ex)
            {
                helper.LogError(ex);
                return "error";
            }
        }

        #endregion

    }
}
