using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository campRepository;
        private readonly IMapper mapper;

        public CampsController(ICampRepository campRepository, IMapper mapper)
        {
            this.campRepository = campRepository;
            this.mapper = mapper;
        }

        #region Creating an Action
        //public object Get()
        //{
        //    return new { Moniker = "ATL2018", Name = "Atlanta Code Camp" };
        //}
        #endregion

        #region Using Status Codes
        //[HttpGet]
        //public IActionResult Get()
        //{
        //    if (false) return BadRequest("Wiadomość - zła/błedna prośba");
        //    if (false) return NotFound("Wiadomość - nie znaleziono");

        //    return Ok(new { Moniker = "ATL2018", Name = "Atlanta Code Camp" });
        //}
        #endregion

        #region Using GET for Collections
        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //    try
        //    {
        //        var results = await campRepository.GetAllCampsAsync();

        //        return Ok(results);
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, "Awaria bazy danych");
        //    }
        //}
        #endregion

        #region Returning Models Instead of Entities
        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //    try
        //    {
        //        var results = await campRepository.GetAllCampsAsync();
        //        CampModel[] models = mapper.Map<CampModel[]>(results);

        //        return Ok(models);
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, "Awaria bazy danych");
        //    }
        //}

        //[HttpGet]
        //public async Task<ActionResult<CampModel[]>> Get()
        //{
        //    try
        //    {
        //        var results = await campRepository.GetAllCampsAsync();
        //        //CampModel[] models = mapper.Map<CampModel[]>(results);

        //        //return models;

        //        return mapper.Map<CampModel[]>(results);
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, "Awaria bazy danych");
        //    }
        //}
        #endregion

        #region Using Query Strings
        [HttpGet]
        public async Task<ActionResult<CampModel[]>> Get(bool includeTalks = false)
        {
            try
            {
                var results = await campRepository.GetAllCampsAsync(includeTalks);

                return mapper.Map<CampModel[]>(results);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Awaria bazy danych");
            }
        }
        #endregion

        #region Getting an Individual Item
        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModel>> Get(string moniker)
        {
            try
            {
                var result = await campRepository.GetCampAsync(moniker);

                if (result == null) return NotFound();

                return mapper.Map<CampModel>(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Awaria bazy danych");
            }
        }
        #endregion

        #region Implementing Searching
        [HttpGet("search")]
        public async Task<ActionResult<CampModel[]>> SearchByDate(DateTime theDate, bool includeTalks = false)
        {
            try
            {
                var result = await campRepository.GetAllCampsByEventDate(theDate, includeTalks);

                if (!result.Any()) return NotFound();

                return mapper.Map<CampModel[]>(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Awaria bazy danych");
            }
        }
        #endregion

        #region Model Binding
        public async Task<ActionResult<CampModel>> Post(CampModel campModel)
        {
            try
            {
                // Create a new Camp
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Awaria bazy danych");
            }
        }
        #endregion
    }
}
