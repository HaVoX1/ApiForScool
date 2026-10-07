using Microsoft.EntityFrameworkCore;
using StudyPlanerSamPlusEFGame.Data;
using StudyPlanner.dtos;
using StudyPlanner.Models;

namespace StudyPlanerSamPlusEFGame.Endpoints_Controller_;

public static class SubjectsEndpoints
{
    public static void MapSubjectsEndpoint(this WebApplication app)
    {
        var group = app.MapGroup("/api/subjects")
            .WithTags("Subjects");

        group.MapGet("/", async (StudyPlannerDbContext db) =>
            {
                var list = await db.Subjects
                    .OrderBy(s => s.Name)
                    .Select(s => new SubjectDto(s.Name, s.Id))
                    .ToListAsync();

                return Results.Ok(list);
            })
            .WithName("GetSubjects")
            .WithSummary("Список предметов")
            .WithDescription("Возвращает все предметы, отсортированные по названию.")
            .Produces<List<SubjectDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:int}", async (int id, StudyPlannerDbContext db) =>
            {
                var subject = await db.Subjects
                    .Where(s => s.Id == id)
                    .Select(s => new SubjectById(s.Id, s.Name))
                    .FirstOrDefaultAsync();

                return subject is null ? Results.NotFound() : Results.Ok(subject);
            })
            .WithName("GetSubjectById")
            .WithSummary("Предмет по id")
            .WithDescription("Возвращает один предмет. Если id не найден — 404.")
            .Produces<SubjectById>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreateNewNameSub dto, StudyPlannerDbContext db) =>
            {
                var error = ValidateName(dto.Name);
                if (error is not null)
                    return Results.BadRequest(new { message = error });

                var subject = new Subject { Name = dto.Name.Trim() };
                db.Subjects.Add(subject);
                await db.SaveChangesAsync();

                var created = new SubjectById(subject.Id, subject.Name);
                return Results.Created($"/api/subjects/{subject.Id}", created);
            })
            .WithName("CreateSubject")
            .WithSummary("Создать предмет")
            .WithDescription("Добавляет предмет с указанным названием.")
            .Produces<SubjectById>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:int}", async (int id, CreateNewNameSub dto, StudyPlannerDbContext db) =>
            {
                var error = ValidateName(dto.Name);
                if (error is not null)
                    return Results.BadRequest(new { message = error });

                var subject = await db.Subjects.FindAsync(id);
                if (subject is null)
                    return Results.NotFound();

                subject.Name = dto.Name.Trim();
                await db.SaveChangesAsync();
                return Results.NoContent();
            })
            .WithName("UpdateSubjectName")
            .WithSummary("Переименовать предмет")
            .WithDescription("Меняет название предмета. Пустое имя — 400, неизвестный id — 404.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:int}", async (int id, StudyPlannerDbContext db) =>
            {
                var subject = await db.Subjects.FindAsync(id);
                if (subject is null)
                    return Results.NotFound();

                db.Subjects.Remove(subject);
                await db.SaveChangesAsync();
                return Results.NoContent();
            })
            .WithName("DeleteSubject")
            .WithSummary("Удалить предмет")
            .WithDescription("Удаляет предмет по id. Если id не найден — 404.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static string? ValidateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "Введите название предмета";

        if (name.Trim().Length > 100)
            return "Название предмета не длиннее 100 символов";

        return null;
    }
}
