using BalarinaAPI.Authentication;
using BalarinaAPI.Core.Model;
using BalarinaAPI.Core.Models;
using BalarinaAPI.Core.ViewModel;
using BalarinaAPI.Core.ViewModel.Slider;
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

namespace BalarinaAPI.Controllers.sliders
{
    [Route("api/[controller]")]
    [ApiController]
    public class SliderController : ControllerBase
    {
        #region variables
        private readonly IUnitOfWork unitOfWork;
        private readonly Helper helper;
        private readonly TrendingDuration trendingDuration = new TrendingDuration();

        #endregion

        #region Constructor
        public SliderController(IUnitOfWork _unitOfWork, IOptions<Helper> _helper)
        {
            unitOfWork = _unitOfWork;
            this.helper = _helper.Value;
        }
        #endregion

        #region Get All Sliders With API Key
        [ApiAuthentication]
        [HttpGet]
        [Route("getallslidersapikey")]
        public async Task<ActionResult<RetrieveData<SliderModel>>> getallslidersapikeyAsync()
        {
            try
            {
                #region Declaration
                List<SliderModel> sliders = new List<SliderModel>();
                var ResultSlider = await unitOfWork.Slider.GetObjects(); ResultSlider.OrderBy(x => x.SliderOrder).ToList();
                var Episodes = await unitOfWork.Episode.GetObjects(); Episodes.ToList();
                var Categories = await unitOfWork.category.GetObjects(); Categories.ToList();
                var programs = await unitOfWork.Program.GetObjects(); programs.ToList();
                var Seasons = await unitOfWork.Season.GetObjects(); Seasons.ToList();
                #endregion

                var result = (from category in Categories
                             join program in programs
                             on category.CategoryId equals program.CategoryId
                             join season in Seasons
                             on program.ProgramId equals season.ProgramId
                             join episode in Episodes
                             on season.SessionId equals episode.SessionId
                             join slider in ResultSlider
                             on episode.EpisodeId equals slider.EpisodeID
                             select new
                             {
                                 slider.SliderId           ,
                                 slider.SliderTitle        ,
                                 slider.SliderImagePath    ,
                                 slider.SliderOrder        ,
                                 slider.SliderViews        ,
                                 program.ProgramId        ,
                                 episode.EpisodeId         ,
                                 category.CategoryId       ,
                                 season.SessionId          ,
                                 program.ProgramName       ,
                                 episode.YoutubeUrl

                             }).ToList();

                foreach (var item in result)
                {
                    SliderModel slider = new SliderModel()
                    {
                        SliderId = item.SliderId,
                        SliderImagePath = item.SliderImagePath,
                        SliderTitle = item.SliderTitle,
                        SliderViews = item.SliderViews,
                        SliderOrder = item.SliderOrder,
                        EpisodeId = item.EpisodeId,
                        YoutubeUrl = item.YoutubeUrl,
                        SessionId = item.SessionId,
                        ProgramIDFk = item.ProgramId,
                        ProgramName = item.ProgramName,
                        CategoryId  = item.CategoryId   
                    };
                    sliders.Add(slider);
                }

                RetrieveData <SliderModel> Collection = new RetrieveData<SliderModel>();
                Collection.Url = helper.LivePathImages;
                Collection.DataList = sliders.ToList();
                return Collection;
            }
            catch (Exception ex)
            {
                helper.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
                // Log error in db
            }
        }
        #endregion

        #region Edit Slider
        //[Authorize]
        [HttpPut]
        [Route("putslider")]
        public async Task<ActionResult<Sliders>> putsliderAsync([FromQuery] SliderToUpdated model)
        {
            try
            {
                #region check Episode id exist

                var _SliderObj = await unitOfWork.Slider.FindObjectAsync(model.SliderId);

                if (_SliderObj == null)
                    return NoContent();
                #endregion

                #region Check values of Slider is not null or empty

                if (model.EpisodeID == null)
                    model.EpisodeID = _SliderObj.EpisodeID;

                if (model.SliderTitle == null)
                    model.SliderTitle = _SliderObj.SliderTitle;

                if (model.SliderImagePath == null && model.SliderImage == null)
                {
                    model.SliderImagePath = _SliderObj.SliderImagePath;
                }
                if (model.SliderImagePath == null)
                {
                    model.SliderImagePath =helper.UploadImage(model.SliderImage);
                }
                #endregion

                #region Handle Order Update 
                await UpdateOrderAsync(model, _SliderObj.SliderOrder);
                #endregion

                #region fill Slider object with values to insert 
                Sliders _slider = new Sliders()
                {
                    SliderId = model.SliderId,
                    EpisodeID = (int)model.EpisodeID,
                    SliderImagePath = model.SliderImagePath ,
                    SliderOrder = (int)model.SliderOrder,
                    SliderTitle = model.SliderTitle,
                    SliderViews = _SliderObj.SliderViews
                };
                #endregion

                #region update operation
                bool result = unitOfWork.Slider.Update(_slider);
                #endregion

                #region check operation is updated successed
                if (!result)
                    return BadRequest("UPDATE OPERATION FAILED");
                #endregion

                #region save changes into db
                await unitOfWork.Complete();
                #endregion

                return Ok("Slider Updated Successfully ");
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
        private async Task<ActionResult> UpdateOrderAsync(SliderToUpdated model, int _OldOrder)
        {
            try
            {
                #region check Slider id exist
                var _SliderObj = await unitOfWork.Slider.FindObjectAsync(model.SliderId);
                if (_SliderObj == null)
                    return NotFound("Slider ID Not Found");
                #endregion

                var MaxOrder =await unitOfWork.Slider.GetObjects();
                int _MaxOrder = MaxOrder.Count();
                string messege = "Order Update Cannot be Greater than Max Order , Max Order " + _MaxOrder;
                if (model.SliderOrder > _MaxOrder)
                    return BadRequest(messege);

                if (model.SliderOrder <= 0)
                    model.SliderOrder = _SliderObj.SliderOrder;

                #region Handle Order Update
                //Get Max Order
                var _Sliders =await unitOfWork.Slider.GetObjects(); _Sliders.OrderBy(Obj => Obj.SliderOrder).ToList();
                //int _MaxOrder = _categories.Count();
                var category = await unitOfWork.Slider.FindObjectAsync( model.SliderId);
                int NewOrder = (int)model.SliderOrder;
                int OldOrder = _OldOrder;

                if (OldOrder < NewOrder)
                {
                    var _SubSliders = _Sliders.Where(obj => obj.SliderOrder > OldOrder && obj.SliderOrder <= NewOrder).OrderBy(o => o.SliderOrder).ToList();
                    foreach (var item in _SubSliders)
                    {
                        item.SliderOrder = item.SliderOrder - 1;
                        bool _result = unitOfWork.Slider.Update(item);
                        if (!_result)
                            return BadRequest("update order operation failed !! ");
                        await unitOfWork.Complete();
                    }
                }
                else if (OldOrder > NewOrder)
                {
                    var _SubSliders = _Sliders.Where(obj => obj.SliderOrder >= NewOrder && obj.SliderOrder < OldOrder).OrderBy(o => o.SliderOrder).ToList();
                    foreach (var item in _SubSliders)
                    {
                        item.SliderOrder = item.SliderOrder + 1;
                        bool _result = unitOfWork.Slider.Update(item);
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
        
    }
}
