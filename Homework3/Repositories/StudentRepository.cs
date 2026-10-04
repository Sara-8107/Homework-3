using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Homework3.Models;

namespace Homework3.Repositories
{
    public class StudentRepository
    {
        private string? connectionString;

        public StudentRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found in appsettings.json.");
        }

        public async Task<List<Student>> GetAllStudents()
        {
            List<Student> studentList = new List<Student>();
            SqlConnection connection = new SqlConnection(connectionString);
            string selectQuery = "Select * From AttendenceDB";
            SqlCommand selectCommand = new SqlCommand(selectQuery, connection);

            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = selectCommand;

            DataTable dataTable = new DataTable();
            await Task.Run(() => adapter.Fill(dataTable));

            foreach (DataRow dataRow in dataTable.Rows)
            {
                Student student = new Student();
                student.StudentId = Convert.ToInt32(dataRow["StudentId"]);
                student.StudentName = dataRow["StudentName"].ToString();
                student.RegistrationNumber = dataRow["RegistrationNumber"].ToString();
                student.Department = dataRow["Department"].ToString();
                student.AttendencePercentage = Convert.ToDouble(dataRow["AttendencePercentage"]);
                studentList.Add(student);
            }
            return studentList;
        }

        public async Task<Student?> GetStudentById(int id)
        {
            Student? student = null;
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand selectCommand = new SqlCommand("Select * From AttendenceDB Where StudentId = @id", connection);
            selectCommand.Parameters.AddWithValue("id", id);

            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = selectCommand;

            DataTable dataTable = new DataTable();
            await Task.Run(() => adapter.Fill(dataTable));

            if (dataTable.Rows.Count > 0)
            {
                DataRow dataRow = dataTable.Rows[0];
                student = new Student();
                student.StudentId = Convert.ToInt32(dataRow["StudentId"]);
                student.StudentName = dataRow["StudentName"].ToString();
                student.RegistrationNumber = dataRow["RegistrationNumber"].ToString();
                student.Department = dataRow["Department"].ToString();
                student.AttendencePercentage = Convert.ToDouble(dataRow["AttendencePercentage"]);
            }

            return student;
        }

        public async Task AddStudent(Student student)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand selectCommand = new SqlCommand("Select * from AttendenceDB where 1=0", connection);

            // FIXED: Changed 'value' to 'VALUES' and 'RegistrationNo' to 'RegistrationNumber'
            string insertQuery = "Insert into AttendenceDB(StudentName,RegistrationNumber,Department,AttendencePercentage) VALUES(@name,@regno,@depart,@attendence)";
            SqlCommand insertCommand = new SqlCommand(insertQuery, connection);

            insertCommand.Parameters.Add(new SqlParameter("name", SqlDbType.NVarChar, 100, "StudentName"));
            insertCommand.Parameters.Add(new SqlParameter("regno", SqlDbType.NVarChar, 30, "RegistrationNumber"));
            insertCommand.Parameters.Add(new SqlParameter("depart", SqlDbType.NVarChar, 50, "Department"));
            insertCommand.Parameters.Add(new SqlParameter("attendence", SqlDbType.Decimal, 5, "AttendencePercentage"));

            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = selectCommand;
            adapter.InsertCommand = insertCommand;

            DataTable dataTable = new DataTable();
            await Task.Run(() => adapter.Fill(dataTable));

            DataRow dataRow = dataTable.NewRow();
            dataRow["StudentName"] = student.StudentName;
            dataRow["RegistrationNumber"] = student.RegistrationNumber;
            dataRow["Department"] = student.Department;
            dataRow["AttendencePercentage"] = student.AttendencePercentage;

            dataTable.Rows.Add(dataRow);

            await Task.Run(() => adapter.Update(dataTable));
        }

        public async Task UpdateStudent(Student student)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand selectCommand = new SqlCommand("Select * from AttendenceDB where StudentId = @id", connection);
            selectCommand.Parameters.AddWithValue("id", student.StudentId);

            // FIXED: Matches parameter names in SQL query (@depart and @attendence)
            string updateQuery = "Update AttendenceDB Set StudentName=@name, RegistrationNumber=@regno, Department=@depart, AttendencePercentage=@attendence where StudentId=@id";

            SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
            updateCommand.Parameters.Add(new SqlParameter("name", SqlDbType.NVarChar, 100, "StudentName"));
            updateCommand.Parameters.Add(new SqlParameter("regno", SqlDbType.NVarChar, 30, "RegistrationNumber"));
            updateCommand.Parameters.Add(new SqlParameter("depart", SqlDbType.NVarChar, 50, "Department")); // FIXED: Changed "dept" to "depart"
            updateCommand.Parameters.Add(new SqlParameter("attendence", SqlDbType.Decimal, 5, "AttendencePercentage")); // FIXED: Changed NVarChar to Decimal

            SqlParameter idParam = new SqlParameter("id", SqlDbType.Int, 4, "StudentId");
            idParam.SourceVersion = DataRowVersion.Original;
            updateCommand.Parameters.Add(idParam);

            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = selectCommand;
            da.UpdateCommand = updateCommand;

            DataTable dt = new DataTable();
            await Task.Run(() => da.Fill(dt));

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                row["StudentName"] = student.StudentName;
                row["RegistrationNumber"] = student.RegistrationNumber;
                row["Department"] = student.Department;
                row["AttendencePercentage"] = student.AttendencePercentage; // FIXED: Typo in column key name

                await Task.Run(() => da.Update(dt));
            }
        }

        public async Task DeleteStudent(int id)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand selectCommand = new SqlCommand("Select * from AttendenceDB where StudentId = @id", connection);
            selectCommand.Parameters.AddWithValue("id", id);

            string deleteQuery = "Delete from AttendenceDB where StudentId = @id";
            SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);

            SqlParameter idParam = new SqlParameter("id", SqlDbType.Int, 4, "StudentId");
            idParam.SourceVersion = DataRowVersion.Original;
            deleteCommand.Parameters.Add(idParam);

            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = selectCommand;
            adapter.DeleteCommand = deleteCommand;

            DataTable dataTable = new DataTable();
            await Task.Run(() => adapter.Fill(dataTable));

            if (dataTable.Rows.Count > 0)
            {
                dataTable.Rows[0].Delete();
                await Task.Run(() => adapter.Update(dataTable));
            }
        }
    }
}