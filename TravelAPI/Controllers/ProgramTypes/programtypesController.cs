using BalarinaAPI.Authentication;
using BalarinaAPI.Core.Model;
using BalarinaAPI.Core.ViewModel;
using BalarinaAPI.Core.ViewModel.Category;
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

        #region Get All ProgramsTypes => Dashboard , API 
        [ApiAuthentication]
        [HttpGet]
        [Route("getallprogramTypes")]
        public async Task<ActionResult<RetrieveData<programTypesOutput>>> getallprogramTypes()
        {
            // Create ProgramsList to return It
            try
            {
                List<programTypesOutput> programTypesOutputs = new List<programTypesOutput>();
                RetrieveData<programTypesOutput> Collection = new RetrieveData<programTypesOutput>();

                var programTypes = await unitOfWork.ProgramType.GetObjects(); programTypes.ToList();

                foreach (var item in programTypes)
                {
                    var _Programs = await unitOfWork.Program.GetObjects(x=>x.ProgramTypeId ==  item.ProgramTypeId);
                    programTypesOutput programTypes1 = new programTypesOutput()
                    {
                        ProgramTypeId = item.ProgramTypeId,
                        ProgramTypeImgPath =item.ProgramTypeImgPath,
                        ProgramTypeOrder = item.ProgramTypeOrder,
                        ProgramTypeTitle = item.ProgramTypeTitle,
                        ProgramCount = _Programs.Count()
                    };

                    programTypesOutputs.Add(programTypes1);
                    Collection.DataList = programTypesOutputs.OrderBy(x => x.ProgramTypeOrder).ToList();
                }
                Collection.Url = helper.LivePathImages;
                return Ok(Collection);
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
            }
        }
        #endregion

        #region Get All Program Type Name , ID 
        /// <summary>
        /// Get All Program Type Name , ID 
        /// </summary>
        /// <returns>
        /// List of Object that Contains 
        /// Id , Name
        /// </returns>
        [ApiAuthentication]
        [HttpGet]
        [Route("GetProgramType_ID_Name")]
        public async Task<ActionResult<List<ListOfNameID<Object_ID_Name>>>> GetProgramType_ID_Name()
        {
            try
            {
                //check If Category ID If Exist
                var _ProgramTypes = await unitOfWork.ProgramType.GetObjects(); _ProgramTypes.ToList();
                //Get All Programs 

                List<ListOfNameID<Object_ID_Name>> Collection = new List<ListOfNameID<Object_ID_Name>>();
                foreach (var item in _ProgramTypes)
                {
                    ListOfNameID<Object_ID_Name> obj = new ListOfNameID<Object_ID_Name>() { ID = item.ProgramTypeId, Name = item.ProgramTypeTitle };
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

        #region Insert New program type
        //[Authorize]
        [HttpPost]
        [Route("createprogramtype")]
        public async Task<ActionResult<ProgramTypeInput>> createprogramtypeAsync([FromQuery] ProgramTypeInput model)
        {
            try
            {
                var Image = HttpContext.Request.Form.Files["ProgramTypeImage"];
                model.ProgramTypeImg = Image; 

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
                  ProgramTypeImgPath =helper.UploadImage(model.ProgramTypeImg),
                };

                bool result = await unitOfWork.ProgramType.Create(_programtype);

                if (!result)
                    return BadRequest("Create Operation Failed");

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

        #region Edit Program type
        //[Authorize]
        [HttpPut]
        [Route("putprogramtype")]
        public async Task<ActionResult<ProgramTypeToUpdate>> putprogramtype([FromQuery] ProgramTypeToUpdate model)
        {
            try
            {
                var Image = HttpContext.Request.Form.Files["ProgramTypeImage"];
                model.ProgramTypeImg = Image;

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
                        model.ProgramTypeImgPath =helper.UploadImage(model.ProgramTypeImg);
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

                await unitOfWork.Complete();

                return StatusCode(StatusCodes.Status200OK);
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
        //[Authorize]
        [HttpDelete("{ID}")]
        public async Task<ActionResult<ProgramType>> deleteprogramtype(int ID)
        {
            try
            {
                #region check if program type exist or not
                var programTypeObj = await unitOfWork.ProgramType.FindObjectAsync(ID);
                if (programTypeObj == null)
                    return BadRequest("Program Type Not Found");
                #endregion

                #region Apply Operation In Db
                bool result = await unitOfWork.ProgramType.DeleteObject(ID);
                if (!result)
                    return NotFound("Program type Not Exist");
                await unitOfWork.Complete();
                #endregion 

                #region Delete image File From Specified Directory 
                helper.DeleteFiles(programTypeObj.ProgramTypeImgPath);
                #endregion

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                string e = ex.StackTrace;
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #endregion
    }
}
