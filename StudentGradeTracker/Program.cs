using System.Data;
using MySql.Data.MySqlClient;

namespace StudentGradeTracker
{
    class StringNotificationArgument : EventArgs
    {
        public string? NotificationTitle { get; set; }
        public string? NotificationText { get; set; }
    }
    class MainClass
    {
        private delegate void StudentGradeTrackerNotifier(object sender, StringNotificationArgument e);
        private static event StudentGradeTrackerNotifier? Notify;

        private static void Notification(object sender, StringNotificationArgument e)
        {
            // this fucntion can call some api to send some data and that api could be capaple of send push notification to n number of devices.
            Console.WriteLine($"Notification Title: {e.NotificationTitle}");
            Console.WriteLine($"Notification Text: {e.NotificationText}");
        }

        private static string InputString(string input)
        {
            Console.Write($"Enter {input}: ");
            string inputString;
            while (true)
            {
                inputString = Console.ReadLine() ?? "";
                if (inputString != "")
                {
                    break;
                }
                Console.WriteLine("Invaliid input!!!\nTry Again: ");
            }
            return inputString;
        }
        private static DateTime InputString(
            string input,
            string dateFromat = "yyyy-MM-dd")
        {
            Console.Write($"Enter {input} in format ({dateFromat}): ");
            string inputString;
            while (true)
            {
                inputString = Console.ReadLine() ?? "";
                if (inputString != "")
                {
                    break;
                }
                Console.WriteLine("Invaliid input!!!\nTry Again: ");
            }
            DateTime returnValue = DateTime.Parse(inputString);
            return returnValue;
        }
        private static int InputInt(string input)
        {
            Console.Write($"Enter {input}: ");
            int inputInt;
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out inputInt) && inputInt >= 0)
                {
                    break;
                }
                Console.WriteLine("Invalid input!!!\nTry Again: ");
            }
            return inputInt;
        }
        private static List<string> AddStudnet(MySqlConnection connection)
        {
            var newStudent = new Student(0, InputString("Studnet Name"), InputString("Enrollment Date", "yyyy-MM-dd"), InputString("Student Email"), InputString("Student Phone Number"));
            List<string> inputString = ["Student Grade Tracker (Notification)"];
            try
            {
                connection.Open();
                MySqlCommand cmd = new("INSERT INTO Students(Student_Name, Date_Of_Birth, EMail, Phone_Number) VALUES (@StudentName, @DateOfBirth, @Email, @PhoneNumber)", connection);
                cmd.Parameters.AddWithValue("@StudentName", newStudent.StudentName);
                cmd.Parameters.AddWithValue("@DateOfBirth", newStudent.DateOfBirth);
                cmd.Parameters.AddWithValue("@Email", newStudent.Email);
                cmd.Parameters.AddWithValue("@PhoneNumber", newStudent.PhoneNumber);
                int rowAffected = cmd.ExecuteNonQuery();
                if (rowAffected == 0)
                {
                    inputString.Add("Adding new student failed...\n(can send confermation mail by this event)");
                }
                else
                {
                    inputString.Add("New Student added successfully...\n(can send confermation mail by this event)");
                }
                Notify += Notification;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
            return inputString;
        }
        public static bool CheckExistenceOfDBAttribute(
            MySqlConnection connection,
            string tableName,
            string atteributeName,
            int atteributeTargetValue)
        {
            bool returnValue = false;
            try
            {
                connection.Open();
                MySqlCommand cmd = new($"SELECT COUNT(*) FROM {tableName} WHERE {atteributeName}=@atteribute1;", connection);
                cmd.Parameters.AddWithValue("@atteribute1", atteributeTargetValue);
                var ret = cmd.ExecuteScalar();
                int rowAffected = Convert.ToInt32(ret);
                if (rowAffected > 0)
                {
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n Catch Block: Error in function.");
            }
            finally
            {
                connection.Close();
            }
            return returnValue;
        }
        private static List<string> AddEnrollment(MySqlConnection connection)
        {
            int StudentId;
            while (true)
            {
                StudentId = InputInt("Student ID");
                if (CheckExistenceOfDBAttribute(connection, "Students", "Student_Id", StudentId))
                {
                    break;
                }
                Console.WriteLine("Student Id does not exist!!!\nTry Again: ");
            }

            int CourseId;
            while (true)
            {
                CourseId = InputInt("Course ID");
                if (CheckExistenceOfDBAttribute(connection, "Courses", "Course_Id", CourseId))
                {
                    break;
                }
                Console.WriteLine("Course does not exist!!!\nTry Again: ");
            }

            List<string> inputString = ["Student Grade Tracker (Notification)"];

            try
            {
                connection.Open();

                // Check if the student is already enrolled in the course
                MySqlCommand checkCmd = new("SELECT COUNT(*) FROM Enrollments WHERE Student_Id = @StudentId AND Course_Id = @CourseId", connection);
                checkCmd.Parameters.AddWithValue("@StudentId", StudentId);
                checkCmd.Parameters.AddWithValue("@CourseId", CourseId);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count > 0)
                {
                    inputString.Add("Student already enrolled in this course!!!");
                    return inputString;
                }

                // Insert enrollment
                DateTime EnrollmentDate = DateTime.Today;
                string finalEnrollmentDate = EnrollmentDate.ToString("yyyy-MM-dd");

                MySqlCommand insertCmd = new("INSERT INTO Enrollments(Student_Id, Course_Id, Enrollment_Date) VALUES (@StudentId, @CourseId, @EnrollmentDate)", connection);
                insertCmd.Parameters.AddWithValue("@StudentId", StudentId);
                insertCmd.Parameters.AddWithValue("@CourseId", CourseId);
                insertCmd.Parameters.AddWithValue("@EnrollmentDate", finalEnrollmentDate);

                int rowsAffected = insertCmd.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    inputString.Add("Course enrollment failed...\n(can send confirmation mail by this event)");
                }
                else
                {
                    inputString.Add("Course enrollment successful...\n(can send confirmation mail by this event)");
                }

                // Insert initial grade record
                MySqlCommand selectLatestCmd = new("SELECT * FROM Enrollments WHERE Enrollment_Id = (SELECT MAX(Enrollment_Id) FROM Enrollments)", connection);
                MySqlDataReader reader = selectLatestCmd.ExecuteReader();

                if (reader.Read())
                {
                    int latestEnrollmentId = Convert.ToInt32(reader["Enrollment_Id"]);

                    MySqlCommand insertGradeCmd = new("INSERT INTO Grades(Enrollment_Id, Grade, Exam_Date, Comments) VALUES (@EnrollmentId, '-', @Date, 'None')", connection);
                    insertGradeCmd.Parameters.AddWithValue("@EnrollmentId", latestEnrollmentId);
                    insertGradeCmd.Parameters.AddWithValue("@Date", DateTime.MinValue.ToString("yyyy-MM-dd")); // Example date value, adjust as needed

                    reader.Close();
                    insertGradeCmd.ExecuteNonQuery();
                }

                Notify += Notification; // Assuming Notify and Notification are defined elsewhere

                inputString.Add("Initial grades set successfully.");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"MySQL Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return inputString;
        }
        private static List<string> UpdateGrades(MySqlConnection connection)
        {
            int StudentId;
            while (true)
            {
                StudentId = InputInt("Student ID");
                if (CheckExistenceOfDBAttribute(connection, "Students", "Student_Id", StudentId))
                {
                    break;
                }
                Console.WriteLine("Student Id does not exist!!!\nTry Again: ");
            }

            int CourseId;
            while (true)
            {
                CourseId = InputInt("Course ID");
                if (CheckExistenceOfDBAttribute(connection, "Courses", "Course_Id", CourseId))
                {
                    break;
                }
                Console.WriteLine("Course does not exist!!!\nTry Again: ");
            }

            List<string> inputString = new() { "Student Grade Tracker (Notification)" };

            try
            {
                connection.Open();

                // Check if the student is enrolled in the course
                MySqlCommand checkCmd = new("SELECT Enrollment_Id FROM Enrollments WHERE Student_Id = @StudentId AND Course_Id = @CourseId", connection);
                checkCmd.Parameters.AddWithValue("@StudentId", StudentId);
                checkCmd.Parameters.AddWithValue("@CourseId", CourseId);
                object result = checkCmd.ExecuteScalar();

                if (result == null)
                {
                    inputString.Add("Student is not enrolled in this course. Cannot update grades.");
                    return inputString;
                }

                int enrollmentId = Convert.ToInt32(result);

                // Update the grade
                Console.Write("Enter new grade: ");
                string newGrade = Console.ReadLine() ?? "";

                MySqlCommand updateCmd = new("UPDATE Grades SET Grade = @Grade WHERE Enrollment_Id = @EnrollmentId", connection);
                updateCmd.Parameters.AddWithValue("@Grade", newGrade);
                updateCmd.Parameters.AddWithValue("@EnrollmentId", enrollmentId);

                int rowsAffected = updateCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    inputString.Add("Grade updated successfully.");
                }
                else
                {
                    inputString.Add("Failed to update grade.");
                }

                Notify += Notification; // Assuming Notify and Notification are defined elsewhere
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"MySQL Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return inputString;
        }
        private static void DisplayRecords(MySqlConnection connection)
        {
            Console.WriteLine("Choose a table to display records:");
            Console.WriteLine("1. Students");
            Console.WriteLine("2. Courses");
            Console.WriteLine("3. Enrollments");
            Console.WriteLine("4. Grades");
            Console.WriteLine("0. Back to main menu");

            Console.Write("Enter your choice: ");
            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice) || (choice < 0 || choice > 4))
            {
                Console.WriteLine("Invalid input! Please enter a valid menu option.");
                Console.Write("Enter your choice: ");
            }

            switch (choice)
            {
                case 1:
                    DisplayTable(connection, "Students");
                    break;
                case 2:
                    DisplayTable(connection, "Courses");
                    break;
                case 3:
                    DisplayTable(connection, "Enrollments");
                    break;
                case 4:
                    DisplayTable(connection, "Grades");
                    break;
                case 0:
                    Console.WriteLine("Returning to main menu...");
                    break;
                default:
                    Console.WriteLine("Invalid choice!");
                    break;
            }
        }

        private static void DisplayTable(MySqlConnection connection, string tableName)
        {
            try
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand($"SELECT * FROM {tableName}", connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    Console.WriteLine($"Records from {tableName}:\n");

                    switch (tableName)
                    {
                        case "Students":
                            while (reader.Read())
                            {
                                Console.WriteLine($"Student ID: {reader["Student_Id"]}");
                                Console.WriteLine($"Student Name: {reader["Student_Name"]}");
                                Console.WriteLine($"Date of Birth: {reader["Date_Of_Birth"]}");
                                Console.WriteLine($"Email: {reader["EMail"]}");
                                Console.WriteLine($"Phone Number: {reader["Phone_Number"]}");
                                Console.WriteLine("--------------------------------------------");
                            }
                            break;
                        case "Courses":
                            while (reader.Read())
                            {
                                Console.WriteLine($"Course ID: {reader["Course_Id"]}");
                                Console.WriteLine($"Course Name: {reader["Course_Name"]}");
                                Console.WriteLine($"Course Code: {reader["Course_Code"]}");
                                Console.WriteLine($"Credit Hours: {reader["Credit_Hours"]}");
                                Console.WriteLine("--------------------------------------------");
                            }
                            break;
                        case "Enrollments":
                            while (reader.Read())
                            {
                                Console.WriteLine($"Enrollment ID: {reader["Enrollment_Id"]}");
                                Console.WriteLine($"Student ID: {reader["Student_Id"]}");
                                Console.WriteLine($"Course ID: {reader["Course_Id"]}");
                                Console.WriteLine($"Enrollment Date: {reader["Enrollment_Date"]}");
                                Console.WriteLine("--------------------------------------------");
                            }
                            break;
                        case "Grades":
                            while (reader.Read())
                            {
                                Console.WriteLine($"Grade ID: {reader["Grade_Id"]}");
                                Console.WriteLine($"Enrollment ID: {reader["Enrollment_Id"]}");
                                Console.WriteLine($"Grade: {reader["Grade"]}");
                                Console.WriteLine($"Exam Date: {reader["Exam_Date"]}");
                                Console.WriteLine($"Comments: {reader["comments"]}");
                                Console.WriteLine("--------------------------------------------");
                            }
                            break;
                    }
                }
                else
                {
                    Console.WriteLine($"No records found in {tableName}.");
                }

                reader.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"MySQL Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }
        private static int SelectionInput()
        {
            Console.WriteLine("Enter 1 to Add New Student.");
            Console.WriteLine("Enter 2 to Add New Enrollment.");
            Console.WriteLine("Enter 3 to Update Student Grade.");
            Console.WriteLine("Enter 4 to Display Records.");
            Console.WriteLine("Enter 0 to Exit.");
            Console.WriteLine("Enter Your selection: ");
            int userInput;
            while (!int.TryParse(Console.ReadLine(), out userInput))
            {
                Console.WriteLine("Invaliid input!!!\nTry Again: ");
            }
            return userInput;
        }

        static void Main(string[] args)
        {
            int selection = 0;
            string DBConnectionString = "server=127.0.0.1;port=3306;user=root;database=StudentGradeTracker;password=;";

            using var DbCLient = new MySqlConnection(DBConnectionString);
            while (true)
            {
                selection = SelectionInput();
                if (selection == 0)
                {
                    Console.WriteLine("Good Bye...");
                    break;
                }
                else if (selection == 1)
                {
                    List<string> notification = AddStudnet(DbCLient);
                    Notify?.Invoke(new object(), new StringNotificationArgument() { NotificationText = notification[1], NotificationTitle = notification[0] });
                }
                else if (selection == 2)
                {
                    List<string> notification = AddEnrollment(DbCLient);
                    Notify?.Invoke(new object(), new StringNotificationArgument() { NotificationText = notification[1], NotificationTitle = notification[0] });
                }
                else if (selection == 3)
                {
                    List<string> notification = UpdateGrades(DbCLient);
                    Notify?.Invoke(new object(), new StringNotificationArgument() { NotificationText = notification[1], NotificationTitle = notification[0] });
                }
                else if (selection == 4)
                {
                    DisplayRecords(DbCLient);
                }
            }
        }
    }
}