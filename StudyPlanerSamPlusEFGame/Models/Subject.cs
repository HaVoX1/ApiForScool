using System.ComponentModel.DataAnnotations;

namespace StudyPlanner.Models;

public class Subject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название предмета")]
    [StringLength(100)]
    public string Name { get; set; } = "";
}