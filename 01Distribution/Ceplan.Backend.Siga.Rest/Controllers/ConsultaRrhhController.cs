
using Microsoft.AspNetCore.Mvc;
using Ceplan.Backend.Siga.Application.Commands;
using Ceplan.Backend.Siga.Application.Dto.Response;
using Ceplan.Backend.Siga.Application.Query;
using MediatR;

namespace Ceplan.Backend.Siga.Rest.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class RrhhController : ControllerBase
    {
        private readonly IMediator _mediator; //una sola instancia

        public RrhhController(IMediator mediator) //constructor
        {
            this._mediator = mediator;
        }


        //definicion del metodo post (disfrasado de get)
        [HttpPost("List")]
        [ProducesResponseType(typeof(ResponseModelDto<List<RrhhListQuery>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModelDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> List([FromBody] RrhhListQuery oInput)
        {
            var oResult = await this._mediator.Send(oInput); //envia comando o Rrhh al mediator para que busque al handle encargado de ejecutar
            return Ok(oResult);

        }




    }
}
