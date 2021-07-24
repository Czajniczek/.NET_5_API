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
        private readonly ICampRepository repository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;

        public TalksController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
        }

        #region Create an Association Controller
        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> Get(string moniker)
        {
            try
            {
                var talks = await repository.GetTalksByMonikerAsync(moniker);

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
                var talk = await repository.GetTalkByMonikerAsync(moniker, talkId);

                if (talk == null)
                {
                    return NotFound();
                }

                return mapper.Map<TalkModel>(talk);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Awaria bazy danych");
            }
        }
        #endregion
    }
}
