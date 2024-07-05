namespace StudentGradeTracker
{
    public record Grade(
        int GradeId,
        int EnrollmentId,
        char GradeValue,
        DateTime ExamDate,
        string Comments
    );
}