using AspNetCoreWebapiSample.Domain.Entities;
using AspNetCoreWebapiSample.Domain.Exceptions;
using AspNetCoreWebapiSample.Domain.Interfaces.Service;
using AspNetCoreWebapiSample.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCoreWebapiSample.Web.Controllers
{
    [Route("api/[controller]")]
    public class HeroController : Controller
    {
        private readonly IHeroService _heroService;
        private readonly IMapper _mapper;

        public HeroController(IHeroService heroService, IMapper mapper)
        {
            _heroService = heroService ?? throw new ArgumentNullException(nameof(heroService));
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<HeroModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var model = _mapper.Map<IEnumerable<HeroModel>>(await _heroService.GetAllAsync());
            return Ok(model);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(HeroModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var hero = await _heroService.GetByIdAsync(id);
                var model = _mapper.Map<HeroModel>(hero);
                return Ok(model);
            }
            catch (CustomNotFoundException)
            {
                return NotFound();
            }
        }


        [HttpPost]
        [ProducesResponseType(typeof(HeroModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] HeroModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var hero = _mapper.Map<Hero>(model);
                var created = await _heroService.InsertAsync(hero);
                model = _mapper.Map<HeroModel>(created);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, model);
            }
            catch (CustomException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut]
        [ProducesResponseType(typeof(HeroModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id,[FromBody] HeroModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var hero = await _heroService.GetByIdAsync(id);
                hero.Name = model.Name;
                hero.SuperPowerId = model.SuperPowerId;
                hero.Age = model.Age;
                hero.Code = model.Code;

                model = _mapper.Map<HeroModel>(await _heroService.UpdateAsync(hero));
                return Ok(model);
            }
            catch (CustomNotFoundException)
            {
                return NotFound();
            }
            catch (CustomException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(int id)
        {
            await _heroService.DeleteAsync(id);
            return Ok();
        }

    }
}
