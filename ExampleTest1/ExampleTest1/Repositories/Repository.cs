using ExampleTest1.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace ExampleTest1.Repositories;

public class Repository : IRepository
{
    private readonly IConfiguration _configuration;
    public Repository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    //Sample Task Implementation
    //Does X exist
    public async Task<bool> SampleExists(int id)
    {
	    //Query
        var query = "SELECT 1 FROM Animal WHERE ID = @ID";

        //Establish Connection via connection String
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        
        //Create command
        await using SqlCommand command = new SqlCommand();

        //Put the command on connection
        command.Connection = connection;
        //Put the query into the command
        command.CommandText = query;
        //Add with parameter value (To avoid SQL Injection)
        command.Parameters.AddWithValue("@ID", id);

        //Open the connection
        await connection.OpenAsync();

        //Get first object from the execution
        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }
    
    public async Task<SampleDto1> SampleGet(int id)
       {
	       
	       //Get query
	    var query = @"SELECT 
							Animal.ID AS AnimalID,
							Animal.Name AS AnimalName,
							Type,
							AdmissionDate,
							Owner.ID as OwnerID,
							FirstName,
							LastName,
							Date,
							[Procedure].Name AS ProcedureName,
							Description
						FROM Animal
						JOIN Owner ON Owner.ID = Animal.Owner_ID
						JOIN Procedure_Animal ON Procedure_Animal.Animal_ID = Animal.ID
						JOIN [Procedure] ON [Procedure].ID = Procedure_Animal.Procedure_ID
						WHERE Animal.ID = @ID";
	    
	    //Establish Connection via connection String
	    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        
	    //Create command
	    await using SqlCommand command = new SqlCommand();

	    //Put the command on connection
	    command.Connection = connection;
	    //Put the query into the command
	    command.CommandText = query;
	    //Add with parameter value (To avoid SQL Injection)
	    command.Parameters.AddWithValue("@ID", id);

	    //Open the connection
	    await connection.OpenAsync();

	    //Execute an async reader
	    var reader = await command.ExecuteReaderAsync();

	    //Get value from the read data
	    var animalIdOrdinal = reader.GetOrdinal("AnimalID");
	    var animalNameOrdinal = reader.GetOrdinal("AnimalName");
	    var animalTypeOrdinal = reader.GetOrdinal("Type");
	    var admissionDateOrdinal = reader.GetOrdinal("AdmissionDate");
	    var ownerIdOrdinal = reader.GetOrdinal("OwnerID");
	    var firstNameOrdinal = reader.GetOrdinal("FirstName");
	    var lastNameOrdinal = reader.GetOrdinal("LastName");
	    var dateOrdinal = reader.GetOrdinal("Date");
	    var procedureNameOrdinal = reader.GetOrdinal("ProcedureName");
	    var procedureDescriptionOrdinal = reader.GetOrdinal("Description");

	    //Make the DTO for return
	    SampleDto1 animalDto = null;

	    //While there is more data to read
	    while (await reader.ReadAsync())
	    {
		    //???
		    if (animalDto is not null)
		    {
			    animalDto.Procedures.Add(new SampleDto3()
			    {
				    Date = reader.GetDateTime(dateOrdinal),
				    Name = reader.GetString(procedureNameOrdinal),
				    Description = reader.GetString(procedureDescriptionOrdinal)
			    });
		    }
		    else
		    {
			    //Make the DTO from read Data
			    animalDto = new SampleDto1()
			    {
				    Id = reader.GetInt32(animalIdOrdinal),
				    Name = reader.GetString(animalNameOrdinal),
				    Type = reader.GetString(animalTypeOrdinal),
				    AdmissionDate = reader.GetDateTime(admissionDateOrdinal),
				    Owner = new SampleDto2()
				    {
					    Id = reader.GetInt32(ownerIdOrdinal),
					    FirstName = reader.GetString(firstNameOrdinal),
					    LastName = reader.GetString(lastNameOrdinal),
				    },
				    Procedures = new List<SampleDto3>()
				    {
					    new SampleDto3()
					    {
						    Date = reader.GetDateTime(dateOrdinal),
						    Name = reader.GetString(procedureNameOrdinal),
						    Description = reader.GetString(procedureDescriptionOrdinal)
					    }
				    }
			    };
		    }
	    }

	    //If no data found, throw excetion
	    if (animalDto is null) throw new Exception();
        
	    //Otherwise, return the DTO
        return animalDto;
    }


    //Add Sample
    public async Task<int> SampleAdd1(NewSampleDTO animal)
    {
	    var insert = @"INSERT INTO Animal VALUES(@Name, @Type, @AdmissionDate, @OwnerId);
					   SELECT @@IDENTITY AS ID;";
	    
	    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
	    await using SqlCommand command = new SqlCommand();
	    
	    command.Connection = connection;
	    command.CommandText = insert;
	    
	    command.Parameters.AddWithValue("@Name", animal.Name);
	    command.Parameters.AddWithValue("@Type", animal.Type);
	    command.Parameters.AddWithValue("@AdmissionDate", animal.AdmissionDate);
	    command.Parameters.AddWithValue("@OwnerId", animal.OwnerId);
	    
	    await connection.OpenAsync();
	    
	    var id = await command.ExecuteScalarAsync();

	    if (id is null) throw new Exception();
	    
	    return Convert.ToInt32(id);
    }

    //Chase that add with another
    public async Task SampleAdd2(int animalId, ProcedureWithDate procedure)
    {
	    var query = $"INSERT INTO Procedure_Animal VALUES(@ProcedureID, @AnimalID, @Date)";

	    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
	    await using SqlCommand command = new SqlCommand();

	    command.Connection = connection;
	    command.CommandText = query;
	    command.Parameters.AddWithValue("@ProcedureID", procedure.ProcedureId);
	    command.Parameters.AddWithValue("@AnimalID", animalId);
	    command.Parameters.AddWithValue("@Date", procedure.Date);

	    await connection.OpenAsync();

	    await command.ExecuteNonQueryAsync();
    }
}