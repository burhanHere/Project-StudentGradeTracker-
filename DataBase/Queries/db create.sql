-- DROP DATABASE StudentGradeTracker;
CREATE DATABASE StudentGradeTracker;

USE StudentGradeTracker;

CREATE TABLE Students (
    Student_Id INT NOT NULL AUTO_INCREMENT,
    Student_Name VARCHAR(100) NOT NULL,
    Date_Of_Birth DATE,
    EMail VARCHAR(100),
    Phone_Number VARCHAR(12) NOT NULL,
    PRIMARY KEY (Student_Id)
);

CREATE TABLE Courses (
    Course_Id INT NOT NULL AUTO_INCREMENT,
    Course_Name VARCHAR(100) NOT NULL,
    Course_Code VARCHAR(100) UNIQUE,
    Credit_Hours INT,
    PRIMARY KEY (Course_Id)
);

CREATE TABLE Enrollments (
    Enrollment_Id INT NOT NULL AUTO_INCREMENT,
    Student_Id INT NOT NULL,
    Course_Id INT NOT NULL,
    Enrollment_Date DATE,
    PRIMARY KEY (Enrollment_Id),
    FOREIGN KEY (Student_Id) REFERENCES Students (Student_Id),
    FOREIGN KEY (Course_Id) REFERENCES Courses (Course_Id)
);

CREATE TABLE Grades (
    Grade_Id INT NOT NULL AUTO_INCREMENT,
    Enrollment_Id INT NOT NULL,
    Grade CHAR(1), -- Assuming grades are single characters like A, B, C, etc.
    Exam_Date DATE,
    comments TEXT,
    PRIMARY KEY (Grade_Id),
    FOREIGN KEY (Enrollment_Id) REFERENCES Enrollments (Enrollment_Id)
);