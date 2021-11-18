using BalarinaAPI.Authentication;
using BalarinaAPI.Core.Models;
using BalarinaAPI.Core.ViewModel;
using BalarinaAPI.Hub;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
        private readonly IHubContext<NotificationHub, IHubClient> hubClient;

        #endregion

        #region Constructor
        public NotificationsController(IUnitOfWork _unitOfWork, IOptions<Helper> _helper, IHubContext<NotificationHub, IHubClient> hubClient)
        {
            unitOfWork = _unitOfWork;
            this.helper = _helper.Value;
            this.hubClient = hubClient;

        }
        #endregion

        #region CREUD OPERATION

        #region Get All Notification 

        [ApiAuthentication]
        [HttpGet]
        [Route("getallnotification")]
        public async Task<ActionResult<RetrieveData<NotificationModel>>> getallnotification()
        {
            try
            {
                List<NotificationModel> notifications = new List<NotificationModel>();
                var _Objects = await unitOfWork.Notification.GetObjects(); _Objects.ToList();
                var _EpisodeObjects = await unitOfWork.Episode.GetObjects(); _EpisodeObjects.ToList();
                var _ProgramObjects = await unitOfWork.Program.GetObjects(); _ProgramObjects.ToList();
                var _SeasonObjects = await unitOfWork.Season.GetObjects(); _SeasonObjects.ToList();

                var result = (from notify in _Objects
                              join episode in _EpisodeObjects
                              on notify.EpisodeID equals episode.EpisodeId
                              join season in _SeasonObjects
                              on  episode.SessionId equals season.SessionId
                              join program in _ProgramObjects
                              on season.ProgramId equals program.ProgramId
                              select new
                              {
                                  notify.ID,
                                  notify.title,
                                  notify.Descriptions,
                                  notify.IMG,
                                  notify.EpisodeID,
                                  program.ProgramName,
                                  notify.Visible,
                                  episode.EpisodeTitle
                              }).ToList();

                foreach (var item in result)
                {
                    NotificationModel notification = new NotificationModel()
                    {
                        Descriptions = item.Descriptions,
                        EpisodeID = item.EpisodeID,
                        EpisodeName = item.EpisodeTitle,
                        ID = item.ID,
                        IMG = item.IMG,
                        title = item.title,
                        Visible = item.Visible,
                        ProgramName = item.ProgramName
                    };
                    notifications.Add(notification);
                }

                RetrieveData<NotificationModel> Collection = new RetrieveData<NotificationModel>();
                Collection.Url = helper.LivePathImages;
                Collection.DataList = notifications;
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
                string ImagePath = null;
                var Image = HttpContext.Request.Form.Files["NotificationImg"];
                model.IMG = Image;

                #region Check values of Notification is not null or empty
                var episodeId = unitOfWork.Episode.FindObjectAsync(model.EpisodeID);

                if (episodeId == null)
                    return BadRequest("Episode ID Not Found ");

                if (string.IsNullOrEmpty(model.title))
                    return BadRequest("Notifications Title cannot be null or empty");

                if (string.IsNullOrEmpty(model.Descriptions))
                    return BadRequest("Notifications Description cannot be null or empty");

                if (model.IMG != null)
                    ImagePath = helper.UploadImage(model.IMG);

                if (Image == null)
                    return BadRequest("Notifications Image cannot be null ");

                #endregion

                #region Fill notification object with values to insert

                Notification notification = new Notification()
                {
                    title = model.title,
                    IMG = ImagePath,
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

                #region Send Notification
                await hubClient.Clients.All.BroadCastNotification();
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

        #region Delete Episode

        //[Authorize]
        [HttpDelete("{ID}")]
        public async Task<ActionResult<Notification>> deleteNotification(int ID)
        {
            try
            {
                #region Check ID If Exist
                var checkIDIfExist = await unitOfWork.Notification.FindObjectAsync(ID);
                if (checkIDIfExist == null)
                    return NotFound("Notification ID Not Found");
                #endregion

                #region Delete Operation
                bool result = await unitOfWork.Notification.DeleteObject(ID);
                #endregion

                #region check Delete Operation  successed
                if (!result)
                    return NotFound("DELETE OPERATION FAILED ");
                #endregion

                #region save changes in db
                await unitOfWork.Complete();
                #endregion

                #region Delete image File From Specified Directory 
                helper.DeleteFiles(checkIDIfExist.IMG);
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
