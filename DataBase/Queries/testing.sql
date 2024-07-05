USE StudentGradeTracker;

SHOW TABLES;

SELECT * FROM Students ORDER BY `Student_Id` DESC;

SELECT * FROM Courses;

SELECT S.Student_Id, S.Student_Name, C.Course_Code, C.Course_Name, G.Grade, G.comments
FROM
    Students S
    INNER JOIN Enrollments E ON S.Student_Id = E.Student_Id
    INNER JOIN Courses C on E.Course_Id = C.Course_Id
    INNER JOIN Grades G on G.Enrollment_Id = E.Enrollment_Id
ORDER BY S.Student_Id;

SELECT * FROM Enrollments;

select COUNT(*) from Students where Student_Id > 100;

SELECT * FROM Students;

SELECT *
from Enrollments
where
    Student_Id = @StudentId
    and Course_Id = @CourseId;

SELECT COUNT(*) FROM Students WHERE Student_id = 99;

SELECT * FROM Courses;

SELECT * FROM Enrollments WHERE Course_Id = 5 and Student_Id = 99;

SELECT * FROM Enrollments WHERE Student_Id = 99;

SELECT * FROM Grades;

SELECT * FROM Grades GROUP BY Comments;

SELECT *
FROM Enrollments
WHERE
    Enrollment_Id = (
        SELECT Max(Enrollment_Id)
        FROM Enrollments
    );

INSERT INTO
    Grades (
        Enrollment_Id,
        Grade,
        Exam_Date,
        Comments
    )
VALUES (
        101,
        '-',
        '0000-00-00',
        'None'
    );

SELECT *
FROM Grades
WHERE
    Grade_Id = (
        SELECT Max(Grade_Id)
        FROM Grades
    );

SELECT * FROM Grades;

SELECT *
FROM Grades
where
    Enrollment_Id = (
        SELECT MAX(Enrollment_Id)
        FROM Enrollments
        where
            Student_Id = 88
    );

SELECT MAX(Enrollment_Id) FROM Enrollments where Student_Id = 88