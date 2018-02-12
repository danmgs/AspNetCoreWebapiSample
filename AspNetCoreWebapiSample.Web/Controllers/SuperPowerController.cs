using AspNetCoreWebapiSample.Domain.Entities;
using AspNetCoreWebapiSample.Domain.Exceptions;
using AspNetCoreWebapiSample.Domain.Interfaces.Service;
using AspNetCoreWebapiSample.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCoreWebapiSample.Web.Controllers
{
    
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class SuperPowerController : Controller
    {
        private readonly ISuperPowerService _superPowerService;
        private readonly IMapper _mapper;

        public SuperPowerController(ISuperPowerService superPowerService, IMapper mapper)
        {
            _superPowerService = superPowerService ?? throw new ArgumentNullException(nameof(superPowerService));
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SuperPowerModel>), StatusCodes.Status200OK)]                
        public async Task<IActionResult> GetAll()
        {
            var model = _mapper.Map<IEnumerable<SuperPowerModel>>(await _superPowerService.GetAllAsync());
            return Ok(model);
        }
                
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SuperPowerModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var superPower = await _superPowerService.GetByIdAsync(id);
                var model = _mapper.Map<SuperPowerModel>(superPower);
                return Ok(model);
            }
            catch (CustomNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(SuperPowerModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] SuperPowerModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var superPower = _mapper.Map<SuperPower>(model);
                var created = await _superPowerService.InsertAsync(superPower);
                model = _mapper.Map<SuperPowerModel>(created);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, model);
            }
            catch (CustomException ex)
            {
                return BadRequest(ex.Message);
            }

        }
        
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SuperPowerModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] SuperPowerModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var superPower = await _superPowerService.GetByIdAsync(id);
                superPower.Name = model.Name;                

                model = _mapper.Map<SuperPowerModel>(await _superPowerService.UpdateAsync(superPower));
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
            await _superPowerService.DeleteAsync(id);

            return Ok();
        }

    }
}
