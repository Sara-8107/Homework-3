namespace Homework3.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public double AttendencePercentage { get; set; }
    }
}
