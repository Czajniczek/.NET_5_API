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
    [Route("api/camps/{moniker}/[controller]")]
    public class TalksController : ControllerBase
    {
        private readonly ICampRepository campRepository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;

        public TalksController(ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this.campRepository = campRepository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
        }

        #region Create an Association Controller
        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> Get(string moniker)
        {
            try
            {
                var talks = await campRepository.GetTalksByMonikerAsync(moniker, true);

                return mapper.Map<TalkModel[]>(talks);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Awaria bazy danych");
            }
        }
        #endregion

        #region GET an Individual Talk
        [HttpGet("{talkId:int}")]
        public async Task<ActionResult<TalkModel>> Get(string moniker, int talkId)
        {
            try
            {
                var talk = await campRepository.GetTalkByMonikerAsync(moniker, talkId, true);
                if (talk == null) return NotFound("The talk was not found");

                return mapper.Map<TalkModel>(talk);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Awaria bazy danych");
            }
        }
        #endregion

        #region POST a New Talk
        [HttpPost]
        public async Task<ActionResult<TalkModel>> Post(string moniker, TalkModel model)
        {
            try
            {
                var camp = await campRepository.GetCampAsync(moniker);
                if (camp == null) return BadRequest("Camp does not exist");

                var talk = mapper.Map<Talk>(model);
                talk.Camp = camp;

                if (model.Speaker == null) return BadRequest("Speaker ID is required");
                var speaker = await campRepository.GetSpeakerAsync(model.Speaker.SpeakerId);
                if (speaker == null) return BadRequest("Speaker could not be found");

                talk.Speaker = speaker;
                campRepository.Add(talk);

                if (await campRepository.SaveChangesAsync())
                {
                    var url = linkGenerator.GetPathByAction(HttpContext, "Get", values: new { moniker, talkId = talk.TalkId });

                    return Created(url, mapper.Map<TalkModel>(talk));
                }
                else return BadRequest("Failed to save new Talk");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Awaria bazy danych");
            }
        }
        #endregion

        #region PUT to Update a Talk
        [HttpPut("{talkId:int}")]
        public async Task<ActionResult<TalkModel>> Put(string moniker, int talkId, TalkModel model)
        {
            try
            {
                var talk = await campRepository.GetTalkByMonikerAsync(moniker, talkId, true);
                if (talk == null) return NotFound("Could not find the talk");

                mapper.Map(model, talk);

                if (model.Speaker != null)
                {
                    var speaker = await campRepository.GetSpeakerAsync(model.Speaker.SpeakerId);
                    if (speaker != null) talk.Speaker = speaker;
                }

                if (await campRepository.SaveChangesAsync()) return mapper.Map<TalkModel>(talk);
                else return BadRequest("Failed to update the database");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Awaria bazy danych");
            }
        }
        #endregion

        #region DELETE a Talk
        [HttpDelete("{talkId:int}")]
        public async Task<IActionResult> Delete(string moniker, int talkId)
        {
            try
            {
                var talk = await campRepository.GetTalkByMonikerAsync(moniker, talkId);
                if (talk == null) return NotFound("Failed to find the talk to delete");

                campRepository.Delete(talk);

                if (await campRepository.SaveChangesAsync()) return Ok();
                else return BadRequest("Failed to delete talk");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Awaria bazy danych");
            }
        }
        #endregion
    }
}
