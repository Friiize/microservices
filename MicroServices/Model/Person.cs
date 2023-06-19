using System.ComponentModel.DataAnnotations;

namespace MicroServices.Model;

public class Person
{
    public Guid Id { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string Name { get; set; }
}