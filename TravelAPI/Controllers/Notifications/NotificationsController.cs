using BalarinaAPI.Authentication;
using BalarinaAPI.Core.Models;
using BalarinaAPI.Core.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelAPI.Core;
using TravelAPI.Core.Helper;

namespace BalarinaAPI.Controllers.Notifications
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        #region variables
        private readonly IUnitOfWork unitOfWork;
        private readonly Helper helper;
        #endregion

        #region Constructor
        public NotificationsController(IUnitOfWork _unitOfWork, IOptions<Helper> _helper)
        {
            unitOfWork = _unitOfWork;
            this.helper = _helper.Value;
        }
        #endregion

        #region CREUD OPERATION

        #region Get All Notification 

        [ApiAuthentication]
        [HttpGet]
        [Route("getallnotification")]
        public async Task<ActionResult<RetrieveData<Notification>>> getallnotification()
        {
            try
            {
                var _Objects = await unitOfWork.Notification.GetObjects(x => x.Visible == true); _Objects.ToList();

                RetrieveData<Notification> Collection = new RetrieveData<Notification>();
                Collection.Url = helper.LivePathImages;
                Collection.DataList = _Objects.ToList();
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

        #region Insert New Notifications 
        [ApiAuthentication]
        [HttpPost]
        [Route("createnotification")]
        public async Task<ActionResult<CategoryModelInput>> createnotification([FromQuery] NotificationsInsert model)
        {
            try
            {
                var Image = HttpContext.Request.Form.Files["NotificationImg"];

                #region Check values of Notification is not null or empty
                var episodeId = unitOfWork.Episode.FindObjectAsync(model.EpisodeID);

                if (episodeId == null)
                    return BadRequest("Episode ID Not Found ");

                if (string.IsNullOrEmpty(model.title))
                    return BadRequest("Notifications Title cannot be null or empty");

                if (string.IsNullOrEmpty(model.Descriptions))
                    return BadRequest("Notifications Description cannot be null or empty");

                if (Image != null)
                    model.IMG = helper.UploadImage(Image);

                if (Image == null)
                    return BadRequest("Notifications Image cannot be null ");

                #endregion

                #region Fill notification object with values to insert

                Notification notification = new Notification()
                {
                    title = model.title,
                    IMG = model.IMG,
                    EpisodeID = model.EpisodeID,
                    Descriptions = model.Descriptions,
                    Visible = model.Visible
                };
                #endregion

                #region Create Operation
                bool result = await unitOfWork.Notification.Create(notification);
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

        #region Edit Notification
        [ApiAuthentication]
        [HttpPut]
        [Route("putnotification")]
        public async Task<ActionResult<NotificationUpdate>> putnotification([FromQuery] NotificationUpdate model)
        {
            try
            {
                string ImageName;
                var Image = HttpContext.Request.Form.Files["NotificationImg"];

                model.IMG = Image;

                #region check category id exist
                var Objects = await unitOfWork.Notification.FindObjectAsync(model.ID);
                if (Objects == null)
                    return NotFound("Notification ID Not Found");

                if (model.EpisodeID != null)
                {
                    int id = (int)model.EpisodeID;
                    var EpisodeID = await unitOfWork.Episode.FindObjectAsync(id);
                    if (EpisodeID == null)
                        return NotFound("Episode ID Not Found");
                }

                #endregion

                #region Check values of Notification is not null or empty

                if (string.IsNullOrEmpty(model.title))
                    model.title = Objects.title;

                if (string.IsNullOrEmpty(model.Descriptions))
                    model.Descriptions = Objects.Descriptions;

                if (Image != null)
                    ImageName = helper.UploadImage(Image);
                else
                    ImageName = Objects.IMG;

                #endregion

                Notification notification = new Notification()
                {
                    title = model.title,
                    IMG = ImageName,
                    EpisodeID = (int)model.EpisodeID,
                    Descriptions = model.Descriptions,
                    Visible = model.Visible
                };


                #region update operation
                bool result = unitOfWork.Notification.Update(notification);
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

        #endregion
    }
}
