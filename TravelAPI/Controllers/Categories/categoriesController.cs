using BalarinaAPI.Authentication;
using BalarinaAPI.Core.Model;
using BalarinaAPI.Core.ViewModel;
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

namespace BalarinaAPI.Controllers.Categories
{
    [Route("api/[controller]")]
    [ApiController]
    public class categoriesController : ControllerBase
    {
        #region variables
        private readonly IUnitOfWork unitOfWork;
        private readonly Helper helper;
        #endregion

        #region Constructor
        public categoriesController(IUnitOfWork _unitOfWork, IOptions<Helper> _helper)
        {
            unitOfWork = _unitOfWork;
            this.helper = _helper.Value;
        }
        #endregion

        #region CREUD OPERATION

        //Get All Category Exist in Episodes Controller 

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
        [Route("getallprogramsbycatid")]
        public async Task<ActionResult<RetrieveData<Program>>> getallprogramsbycatid(int CategoryID)
        {
            try
            {
                //check If Category ID If Exist
                var _CategoryID = await unitOfWork.category.FindObjectAsync(CategoryID);
                if (_CategoryID == null)
                    return BadRequest("Category ID Not Exist !!");
                //Get All Programs 
                var ResultPrograms = await unitOfWork.Program.GetObjects(x => x.CategoryId == CategoryID); ResultPrograms.ToList();

                RetrieveData<Program> Collection = new RetrieveData<Program>();
                Collection.Url = helper.LivePathImages;          
                Collection.DataList = ResultPrograms.ToList();

                #region Update Category Views
                 _CategoryID.CategoryViews += 1;
                await unitOfWork.Complete();
                #endregion

                return Collection;
            }
            catch (Exception ex)
            {
                // Log error in  
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
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
        [Route("getcategories_id_name")]
        public async Task<ActionResult<List<ListOfNameID<Object_ID_Name>>>> GetCategories_ID_Name() 
        {
            try
            {
                //check If Category ID If Exist
                var _CategoriesObject =await unitOfWork.category.GetObjects(); _CategoriesObject.ToList();
                if (_CategoriesObject == null)
                    return BadRequest("Categories list is empty ");
                //Get All Programs 

                List<ListOfNameID<Object_ID_Name>> Collection = new List<ListOfNameID<Object_ID_Name>>();
                foreach (var item in _CategoriesObject)
                {
                    ListOfNameID<Object_ID_Name> obj = new ListOfNameID<Object_ID_Name>(){ID = item.CategoryId, Name = item.CategoryTitle};
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

        #region Insert New Category 
        /// <summary>
        /// insert new category that have CategoryTitle,CategoryDescription,CategoryVisible,CategoryOrder
        /// and CategoryImg 
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// status of operation - Created Successfully  - or Status500InternalServerError
        /// </returns>
        //[ApiAuthentication]
        [HttpPost]
        [Route("createcategory")]
        public async Task<ActionResult<CategoryModelInput>> createcategoryAsync([FromQuery] CategoryModelInput model)
        {
            try
            {
                var CategoryImage = HttpContext.Request.Form.Files["CategoryImage"];

                #region Check values of Category is not null or empty
                if (string.IsNullOrEmpty(model.CategoryTitle))
                    return BadRequest("Category Title cannot be null or empty");

                if (string.IsNullOrEmpty(model.CategoryDescription))
                    return BadRequest("Category Description cannot be null or empty");

                if (CategoryImage != null)
                    model.CategoryImg = CategoryImage;

                if (model.CategoryImg == null)
                    return BadRequest("Category Image cannot be null ");

                if (model.CategoryOrder < 0)
                    return BadRequest("Category Order cannot be less than 0 ");

                #endregion

                #region Fill Category object with values to insert
                Category _category = new Category()
                {
                    CategoryTitle = model.CategoryTitle,
                    CategoryDescription = model.CategoryDescription,
                    CategoryOrder = model.CategoryOrder,
                    CreationDate = DateTime.Now,
                    CategoryVisible = model.CategoryVisible,
                    CategoryImg = helper.UploadImage(model.CategoryImg),
                    CategoryViews =0
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

                return StatusCode(StatusCodes.Status200OK);
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
        /// update Category(elements or one element)
        /// </summary>
        /// <param name = "model" ></ param >
        /// < returns >
        /// status of operation  -Updated Successfully - or Status500InternalServerError
        /// </returns>

        //[ApiAuthentication]
        [HttpPut]
        [Route("putcategories")]
        public async Task<ActionResult<CategoryModelInput>> putcategories([FromQuery] CategoryToUpdate model)
        {
            try
            {
                var CategoryImage = HttpContext.Request.Form.Files["CategoryImage"];
                model.CategoryImg = CategoryImage;

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
                    model.CategoryImgPath = helper.UploadImage(model.CategoryImg);
                }

                    //model.CategoryViews = _categoryObj.CategoryViews;

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
                    CategoryImg = model.CategoryImgPath,
                    CategoryViews = _categoryObj.CategoryViews
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


                return StatusCode(StatusCodes.Status200OK);
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
                var _categoryObj = await unitOfWork.category.FindObjectAsync(model.CategoryID);
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
                else if (OldOrder > NewOrder)
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
            catch (Exception ex)
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
        [ApiAuthentication]
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
