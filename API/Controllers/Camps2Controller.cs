using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [ApiController]
    [ApiVersion("2.0")]
    //[Route("api/camps")]
    [Route("api/v{version:apiVersion}/camps")]
    public class Camps2Controller : ControllerBase
    {
        private readonly ICampRepository campRepository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;

        public Camps2Controller(ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this.campRepository = campRepository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
        }

        #region Using Query Strings
        [HttpGet]
        public async Task<IActionResult> Get(bool includeTalks = false)
        {
            try
            {
                var results = await campRepository.GetAllCampsAsync(includeTalks);
                var result = new
                {
                    Count = results.Length,
                    Results = mapper.Map<CampModel[]>(results)
                };

                return Ok(result);
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

        #region Model Binding, Implementing POST and Adding Model Validation
        [HttpPost]
        public async Task<ActionResult<CampModel>> Post(CampModel model)
        {
            try
            {
                // W przyoadku jeżeli chcemy napisać własną reakcję na walidację zamiast tej z [ApiController]
                //if(ModelState.IsValid) ...

                var existingCamp = await campRepository.GetCampAsync(model.Moniker);
                if (existingCamp != null) return BadRequest("Moniker in Use");

                var location = linkGenerator.GetPathByAction("Get", "Camps", new { moniker = model.Moniker });
                if (string.IsNullOrWhiteSpace(location)) return BadRequest("Could not use curent moniker");

                // Create a new Camp
                var camp = mapper.Map<Camp>(model);
                campRepository.Add(camp);

                if (await campRepository.SaveChangesAsync())
                {
                    //return Created($"/api/camps/{camp.Moniker}", mapper.Map<CampModel>(camp));
                    return Created(location, mapper.Map<CampModel>(camp));
                }
                else return BadRequest("Failed to save new Camp");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Awaria bazy danych");
            }
        }
        #endregion

        #region Implementing PUT
        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker, CampModel model)
        {
            try
            {
                var oldCamp = await campRepository.GetCampAsync(moniker);
                if (oldCamp == null) return NotFound($"Could not find camp with moniker of {moniker}");

                mapper.Map(model, oldCamp);

                if (await campRepository.SaveChangesAsync()) return mapper.Map<CampModel>(oldCamp);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Awaria bazy danych");
            }

            return BadRequest();
        }
        #endregion

        #region Implementing DELETE
        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete(string moniker)
        {
            try
            {
                var oldCamp = await campRepository.GetCampAsync(moniker);
                if (oldCamp == null) return NotFound();

                campRepository.Delete(oldCamp);

                if (await campRepository.SaveChangesAsync()) return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Awaria bazy danych");
            }

            return BadRequest("Failed to delete the camp");
        }
        #endregion
    }
}
