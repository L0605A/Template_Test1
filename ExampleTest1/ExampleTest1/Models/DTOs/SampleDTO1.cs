namespace ExampleTest1.Models.DTOs;

//A dto
public class SampleDto1
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime AdmissionDate { get; set; }
    public SampleDto2 Owner { get; set; } = null!;
    public List<SampleDto3> Procedures { get; set; } = null!;
}

//You can put more than 1 dto in a file
public class SampleDto2
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

//You can put more than 1 dto in a file
public class SampleDto3
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}