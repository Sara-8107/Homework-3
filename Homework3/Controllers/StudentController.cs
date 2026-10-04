using Microsoft.AspNetCore.Mvc;
using Homework3.Models;
using Homework3.Repositories;

namespace Homework3.Controllers
{
    public class StudentController : Controller
    {
        private StudentRepository repository;

        public StudentController(StudentRepository repo)
        {
            repository = repo;
        }
        public async Task<IActionResult> Index()
        {
            List<Student> students = await repository.GetAllStudents();
            return View(students);
        }
        public IActionResult Create()
        {
            return View();
        }
        public async Task<IActionResult> Add(Student student)
        {
            await repository.AddStudent(student);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int id)
        {
            Student? student = await repository.GetStudentById(id);
            return View(student);
        }
        public async Task<IActionResult> Update(Student student)
        {
            await repository.UpdateStudent(student);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            await repository.DeleteStudent(id);
            return RedirectToAction("Index");
        }
    }
}