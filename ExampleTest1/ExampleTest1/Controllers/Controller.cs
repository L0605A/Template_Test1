using System.Transactions;
using ExampleTest1.Models.DTOs;
using ExampleTest1.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExampleTest1.Controllers
{
    [Route("api/animals")]
    [ApiController]
    public class Controller : ControllerBase
    {
        private readonly IRepository _animalsRepository;
        public Controller(IRepository animalsRepository)
        {
            _animalsRepository = animalsRepository;
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnimal(int id)
        {
            //Check for the validity
            if (!await _animalsRepository.SampleExists(id))
                return NotFound($"Animal with given ID - {id} doesn't exist");

            //Get the data
            var animal = await _animalsRepository.SampleGet(id);
            
            return Ok(animal);
        }
        
        
        // Version with transaction scope
        [HttpPost]
        public async Task<IActionResult> AddAnimalV2(NewSampleDTOProcedures newAnimalWithProcedures)
        {

            //Check if exist
            if (!await _animalsRepository.SampleExists(newAnimalWithProcedures.OwnerId))
                return NotFound($"Owner with given ID - {newAnimalWithProcedures.OwnerId} doesn't exist");

            //You can check existance in loops
            foreach (var procedure in newAnimalWithProcedures.Procedures)
            {
                if (!await _animalsRepository.SampleExists(procedure.ProcedureId))
                    return NotFound($"Procedure with given ID - {procedure.ProcedureId} doesn't exist");
            }

            //Use transaction for rollback
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var id = await _animalsRepository.SampleAdd1(new NewSampleDTO()
                {
                    Name = newAnimalWithProcedures.Name,
                    Type = newAnimalWithProcedures.Type,
                    AdmissionDate = newAnimalWithProcedures.AdmissionDate,
                    OwnerId = newAnimalWithProcedures.OwnerId
                });

                foreach (var procedure in newAnimalWithProcedures.Procedures)
                {
                    await _animalsRepository.SampleAdd2(id, procedure);
                }

                scope.Complete();
            }

            return Created(Request.Path.Value ?? "api/animals", newAnimalWithProcedures);
        }
    }
}
