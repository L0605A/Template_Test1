using ExampleTest1.Models.DTOs;

namespace ExampleTest1.Repositories;

public interface IRepository
{
    //Task<Return_Type> TaskName(Argument_Type argument_Name);

    
    Task<bool> SampleExists(int id);
    
    Task<SampleDto1> SampleGet(int id);
    
    //2 phase add
    Task<int> SampleAdd1(NewSampleDTO animal);
    Task SampleAdd2(int animalId, ProcedureWithDate procedure);

}