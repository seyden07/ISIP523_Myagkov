using System;
using System.Collections.Generic;

class Program
{
    static List<Student> students = new List<Student>();
    static List<Teacher> teachers = new List<Teacher>();
    static List<Course> courses = new List<Course>();
    static int nextStudentId = 1;
    static int nextTeacherId = 1;
    static int nextCourseId = 1;

    static void Main(string[] args)
    {

        AddTestData();

        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("\n===== Система управления университетом =====");
            Console.WriteLine("1. Добавить студента");
            Console.WriteLine("2. Добавить преподавателя");
            Console.WriteLine("3. Создать курс");
            Console.WriteLine("4. Показать всех студентов");
            Console.WriteLine("5. Показать всех преподавателей");
            Console.WriteLine("6. Показать все курсы");
            Console.WriteLine("7. Записать студента на курс");
            Console.WriteLine("8. Назначить преподавателя на курс");
            Console.WriteLine("9. Показать курсы студента");
            Console.WriteLine("10. Показать студентов курса");
            Console.WriteLine("11. Показать курсы преподавателя");
            Console.WriteLine("0. Выход");
            Console.Write("\nВыберите опцию: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddStudent();
                    break;
                case "2":
                    AddTeacher();
                    break;
                case "3":
                    AddCourse();
                    break;
                case "4":
                    ShowAllStudents();
                    break;
                case "5":
                    ShowAllTeachers();
                    break;
                case "6":
                    ShowAllCourses();
                    break;
                case "7":
                    EnrollStudentInCourse();
                    break;
                case "8":
                    AssignTeacherToCourse();
                    break;
                case "9":
                    ShowStudentCourses();
                    break;
                case "10":
                    ShowCourseStudents();
                    break;
                case "11":
                    ShowTeacherCourses();
                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("\bНеверный выбор!");
                    break;
            }

            if (!exit)
            {
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }
    }


}

class Human
{
    public string FIO { get; private set; }
    public int Age { get; private set; }
    public DateOnly Birthday { get; private set; }
    public string Gender { get; private set; }

    public Human(string fio, int age, DateOnly birthday, string gender)
    {
        FIO = fio;
        Age = age;
        Birthday = birthday;
        Gender = gender;
    }

    public virtual string GetInfo()
    {
        return $"{FIO}, Возраст: {Age}, Пол: {Gender}";
    }
}

class Student : Human
{
    public int StudentID { get; private set; }
    private List<Course> enrolledCourses;

    public Student(string fio, int age, DateOnly birthday, string gender, int studentID)
        : base(fio, age, birthday, gender)
    {
        StudentID = studentID;
        enrolledCourses = new List<Course>();
    }

    public void EnrollInCourse(Course course)
    {
        if (!enrolledCourses.Contains(course))
        {
            enrolledCourses.Add(course);
            course.AddStudent(this);
        }
    }

    public void ShowEnrolledCourses()
    {
        Console.WriteLine($"\nКурсы студента {FIO} (ID: {StudentID}):");
        if (enrolledCourses.Count == 0)
        {
            Console.WriteLine("Нет записей на курсы");
            return;
        }
        foreach (var course in enrolledCourses)
        {
            Console.WriteLine($"- {course.CourseName} (ID: {course.CourseID})");
        }
    }

    public override string GetInfo()
    {
        return $"Студент ID: {StudentID}, {base.GetInfo()}, Курсов: {enrolledCourses.Count}";
    }
}

class Teacher : Human
{
    public int TeacherID { get; private set; }
    public int ExpYear { get; private set; }
    private List<Course> assignedCourses;

    public Teacher(string fio, int age, DateOnly birthday, string gender, int teacherID, int expYear)
        : base(fio, age, birthday, gender)
    {
        TeacherID = teacherID;
        ExpYear = expYear;
        assignedCourses = new List<Course>();
    }

    public void AssignToCourse(Course course)
    {
        if (!assignedCourses.Contains(course))
        {
            assignedCourses.Add(course);
        }
    }

    public void ShowAssignedCourses()
    {
        Console.WriteLine($"\nКурсы преподавателя {FIO} (ID: {TeacherID}):");
        if (assignedCourses.Count == 0)
        {
            Console.WriteLine("Нет назначенных курсов");
            return;
        }
        foreach (var course in assignedCourses)
        {
            Console.WriteLine($"- {course.CourseName} (ID: {course.CourseID})");
        }
    }

    public override string GetInfo()
    {
        return $"Преподаватель ID: {TeacherID}, {base.GetInfo()}, Стаж: {ExpYear} лет, Курсов: {assignedCourses.Count}";
    }
}

class Course
{
    public int CourseID { get; private set; }
    public string CourseName { get; private set; }
    public int CourseYear { get; private set; }
    private Teacher assignedTeacher;
    private List<Student> enrolledStudents;

    public Course(int courseID, string courseName, int courseYear)
    {
        CourseID = courseID;
        CourseName = courseName;
        CourseYear = courseYear;
        enrolledStudents = new List<Student>();
    }

    public void AssignTeacher(Teacher teacher)
    {
        assignedTeacher = teacher;
        teacher.AssignToCourse(this);
    }

    public void AddStudent(Student student)
    {
        if (!enrolledStudents.Contains(student))
        {
            enrolledStudents.Add(student);
        }
    }

    public void ShowEnrolledStudents()
    {
        Console.WriteLine($"\nСтуденты курса '{CourseName}' (ID: {CourseID}):");
        if (enrolledStudents.Count == 0)
        {
            Console.WriteLine("Нет записанных студентов");
            return;
        }
        foreach (var student in enrolledStudents)
        {
            Console.WriteLine($"- {student.FIO} (ID: {student.StudentID})");
        }
    }

    public string GetInfo()
    {
        string teacherInfo = assignedTeacher != null ? assignedTeacher.FIO : "Не назначен";
        return $"Курс ID: {CourseID}, {CourseName}, Год: {CourseYear}, Преподаватель: {teacherInfo}, Студентов: {enrolledStudents.Count}";
    }
}