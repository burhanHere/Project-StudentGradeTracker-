# Student Grade Tracker

The Student Grade Tracker is a C# console application designed to manage student records and course enrollments using a MySQL database. It supports CRUD operations on students, courses, enrollments, and grades, and features a notification system using events for successful operations.

## Features

- **Add New Student**: Enter student details and store them in the database.
- **Add New Enrollment**: Enroll a student in a course, setting an enrollment date and initial grades.
- **Update Student Grade**: Modify grades for enrolled students in specific courses.
- **Display Records**: View records for Students, Courses, Enrollments, and Grades.
- **Notification System**: Console notifications for successful operations.

## Requirements

- **.NET Core SDK** (version 3.1 or higher)
- **MySQL Server**
- **NuGet Package**: `MySql.Data.MySqlClient`

## Installation and Setup

### Clone the repository

```bash
git clone <repository-url>
cd StudentGradeTracker
