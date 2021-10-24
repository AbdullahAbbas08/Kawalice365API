using BalarinaAPI.Authentication;
using BalarinaAPI.Core.Model;
using BalarinaAPI.Core.ViewModel.ProgramTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TravelAPI.Core;
using TravelAPI.Core.Helper;

namespace BalarinaAPI.Controllers.ProgramTypes
{
    [Route("api/[controller]")]
    [ApiController]
    public class programtypesController : ControllerBase
    {
        #region variables
        private readonly IUnitOfWork unitOfWork;
        private readonly Helper helper;
        #endregion

        #region Constructor
        public programtypesController(IUnitOfWork _unitOfWork, IOptions<Helper> _helper)
        {
            unitOfWork = _unitOfWork;
            this.helper = _helper.Value;

        }

        #endregion

         #region CRUD OPERATIONS 

           #region Get All ProgramsTypes
        [ApiAuthentication]
        [HttpGet]
        [Route("getallprogramTypes")]
        public async Task<ActionResult<List<programTypesOutput>>> getallprogramTypes()
        {
            // Create ProgramsList to return It
            try
            {
                List<programTypesOutput> programTypesOutputs = new List<programTypesOutput>();
                var programTypes = await unitOfWork.ProgramType.GetObjects(); programTypes.ToList();
                foreach (var item in programTypes)
                {
                    programTypesOutput programTypes1 = new programTypesOutput()
                    {
                        ProgramTypeId = item.ProgramTypeId,
                        ProgramTypeImgPath = helper.LivePathImages+item.ProgramTypeImgPath,
                        ProgramTypeOrder = item.ProgramTypeOrder,
                        ProgramTypeTitle = item.ProgramTypeTitle
                    };
                    programTypesOutputs.Add(programTypes1);
                }
                return programTypesOutputs;
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
            }
        }
        #endregion

           #region Insert New program type
        [Authorize]
        [HttpPost]
        [Route("createprogramtype")]
        public async Task<ActionResult<ProgramTypeInput>> createprogramtypeAsync([FromQuery] ProgramTypeInput model)
        {
            try
            {

                if (string.IsNullOrEmpty(model.ProgramTypeTitle))
                    return BadRequest("program type title cannot be null or empty");

                if (model.ProgramTypeImg == null)
                    return BadRequest("program type Image cannot be null ");

                if (model.ProgramTypeOrder < 0)
                    return BadRequest("program type Order cannot be less than 0 ");

                ProgramType _programtype = new ProgramType()
                {
                  ProgramTypeTitle = model.ProgramTypeTitle,
                  ProgramTypeOrder = model.ProgramTypeOrder,
                  ProgramTypeImgPath = UploadImage(model.ProgramTypeImg),
                };

                bool result = await unitOfWork.ProgramType.Create(_programtype);

                if (!result)
                    return BadRequest("Create Operation Failed");

                unitOfWork.Complete();

                return Ok("Program type Created Successfully");
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

           #region Edit Program type
        [Authorize]
        [HttpPut]
        [Route("putprogramtype")]
        public async Task<ActionResult<ProgramTypeToUpdate>> putprogramtype([FromQuery] ProgramTypeToUpdate model)
        {
            try
            {
                if (model.ProgramTypeId == null)
                    return BadRequest("Program Type ID Invalid !! ");

                var _programObject = await unitOfWork.ProgramType.FindObjectAsync((int)model.ProgramTypeId);
                
                if (_programObject == null)
                    return BadRequest("Program type ID Not Found !! ");

              

                if (model.ProgramTypeTitle == null)
                    model.ProgramTypeTitle = _programObject.ProgramTypeTitle;

                #region check if image updated or not 
                if (model.ProgramTypeImgPath == null)
                {
                    if (model.ProgramTypeImg != null)
                        model.ProgramTypeImgPath = UploadImage(model.ProgramTypeImg);
                }
                if (model.ProgramTypeImgPath == null && model.ProgramTypeImg == null)
                {
                    model.ProgramTypeImgPath = _programObject.ProgramTypeImgPath;
                }
                #endregion

              

                if (model.ProgramTypeOrder == null)
                    model.ProgramTypeOrder = _programObject.ProgramTypeOrder;

                ProgramType _programtype = new ProgramType()
                {
                   ProgramTypeId= (int)model.ProgramTypeId,
                   ProgramTypeImgPath = model.ProgramTypeImgPath,
                   ProgramTypeOrder = (int)model.ProgramTypeOrder,
                   ProgramTypeTitle = model.ProgramTypeTitle
                };

                bool result = unitOfWork.ProgramType.Update(_programtype);

                if (!result)
                    return BadRequest("UPDATE OPERATION FAILED ");

                unitOfWork.Complete();

                return Ok("UPDATE OPERATION Successfully ");
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

           #region Delete Program type
        [Authorize]
        [HttpDelete("{ID}")]
        public async Task<ActionResult<ProgramType>> deleteprogramtypeAsync(int ID)
        {
            try
            {
                bool result = await unitOfWork.ProgramType.DeleteObject(ID);
                if (!result)
                    return NotFound("Program type Not Exist");
                unitOfWork.Complete();

                return Ok("program type id deleted successfully ");
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
                    {
                        categoryImage.CopyTo(stream);
                    }
                    return fileName;
                }
                else
                {
                    return "error : img is null ";
                }
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return "error";
            }
        }

        #endregion

    }
}
