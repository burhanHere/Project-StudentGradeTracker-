namespace StudentGradeTracker
{
    public record Enrollment(
        int EnrollmentId = 0,
        int StudentId = 0,
        int CourseId = 0,
        DateTime EnrollmentDate = default
    );
}