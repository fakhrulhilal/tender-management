using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenderManagement.Application.Common.Model;
using TenderManagement.Application.Tender.Command;
using TenderManagement.Application.Tender.Query;

namespace TenderManagement.WebApi.Controllers
{
    [Authorize]
    public class TenderController : ApiControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedList<GetTenderListQuery.Response>>> GetList(
            [FromQuery] GetTenderListQuery query) => await Mediator.Send(query);

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GetTenderDetailQuery.Response>> GetDetail(int id) =>
            await Mediator.Send(new GetTenderDetailQuery {Id = id});

        [HttpPost]
        public async Task<ActionResult<CreateTenderCommand.Response>> Create(CreateTenderCommand command) =>
            await Mediator.Send(command);

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, UpdateTenderCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            await Mediator.Send(command);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            await Mediator.Send(new DeleteTenderCommand {Id = id});
            return NoContent();
        }
    }
}
