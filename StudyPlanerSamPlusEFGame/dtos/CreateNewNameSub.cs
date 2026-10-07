using System.ComponentModel.DataAnnotations;

namespace StudyPlanner.dtos;

public record CreateNewNameSub(
    [property: Required(ErrorMessage = "Введите название предмета")]
    [property: StringLength(100)]
    string Name
);