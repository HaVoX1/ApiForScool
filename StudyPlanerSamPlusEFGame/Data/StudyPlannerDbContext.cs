using Microsoft.EntityFrameworkCore;
using StudyPlanner.Models;

namespace StudyPlanerSamPlusEFGame.Data;

public class StudyPlannerDbContext(DbContextOptions<StudyPlannerDbContext> options) : DbContext(options)
{
    public DbSet<Subject> Subjects => Set<Subject>();
}