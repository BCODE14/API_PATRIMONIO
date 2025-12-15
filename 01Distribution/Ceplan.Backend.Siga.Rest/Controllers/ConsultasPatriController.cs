using Microsoft.AspNetCore.Mvc;
using Ceplan.Backend.Siga.Application.Commands;
using Ceplan.Backend.Siga.Application.Dto.Response;
using Ceplan.Backend.Siga.Application.Query;
using MediatR;

namespace Ceplan.Backend.Siga.Rest.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class DesplaController : ControllerBase
    {
        private readonly IMediator _mediator; //una sola instancia

        public DesplaController(IMediator mediator) //constructor
        {
            this._mediator = mediator;
        }


        //definicion del metodo post (disfrasado de get)
        [HttpPost("Despla")]
        [ProducesResponseType(typeof(ResponseModelDto<List<DesplaListQuery>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModelDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> List([FromBody] DesplaListQuery oInput)
        {
            var oResult = await this._mediator.Send(oInput); //envia comando o Despla al mediator para que busque al handle encargado de ejecutar
            return Ok(oResult);

        }




    }
}
