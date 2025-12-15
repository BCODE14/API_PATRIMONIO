using Microsoft.AspNetCore.Mvc;
using Ceplan.Backend.Siga.Application.Commands;
using Ceplan.Backend.Siga.Application.Dto.Response;
using Ceplan.Backend.Siga.Application.Query;
using MediatR;

namespace Ceplan.Backend.Siga.Rest.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class AsignacionConsultaController : ControllerBase
    {
        private readonly IMediator _mediator; //mediador

        public AsignacionConsultaController(IMediator mediator)
        {
            this._mediator = mediator;
        }


        //definicion de el metodo put distrasado de post - devuelve un mensaje
        [HttpPost("Asig")]
        [ProducesResponseType(typeof(ResponseModelDto<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModelDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Asig([FromBody] AsignacionConsultaListQuery oInput)
        {
            var oResult = await this._mediator.Send(oInput);
            return Ok(oResult);

        }
    }
}
