
using Microsoft.AspNetCore.Mvc;
using Ceplan.Backend.Siga.Application.Commands;
using Ceplan.Backend.Siga.Application.Dto.Response;
using Ceplan.Backend.Siga.Application.Query;
using MediatR;

namespace Ceplan.Backend.Siga.Rest.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AsignacionbienesController : ControllerBase
    {
        private readonly IMediator _mediator; //una sola instancia

        public AsignacionbienesController(IMediator mediator) //constructor
        {
            this._mediator = mediator;
        }


        //definicion del metodo post (disfrasado de get)
        [HttpPost("solicbien")]
        [Produces("application/pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType( StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Asigbien([FromBody] AsignacionBienQuery oInput)
        {
            // var oResult = await this._mediator.Send(oInput); //envia comando o consulta al mediator para que busque al handle encargado de ejecutar
            //return Ok(oResult);

            var result = await _mediator.Send(oInput);

            var archivo = $"Solicitudpatrimonio_{DateTime.Now:yyyyMMdd}.pdf";

            if (result.bSuccess && result.oData != null)
            {
                return File(
                    result.oData,
                    "application/pdf",
                    archivo
                );
            }

            return BadRequest(new
            {
                message = result.sMessage
            });



        }




    }
}
