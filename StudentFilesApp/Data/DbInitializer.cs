using StudentFilesApp.Models;
using System;
using System.Linq;

namespace StudentFilesApp.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

           
            if (context.Students.Any()) return;

         
            var roleDean = new Role { RoleName = "Деканат" };
            var roleKaf = new Role { RoleName = "Кафедра" };
            context.Roles.AddRange(roleDean, roleKaf);
            context.SaveChanges();

           
            context.Users.AddRange(
                new User { Login = "deсan", PasswordHash = "12345", LastName = "Смирнова", FirstName = "Елена", RoleId = roleDean.RoleId },
                new User { Login = "kafedra", PasswordHash = "12345", LastName = "Кузнецов", FirstName = "Андрей", RoleId = roleKaf.RoleId }
            );
            context.SaveChanges();

            
            var dept = new Department { Name = "Кафедра физики и прикладной математики", Head = "Лоханов А.В." };
            context.Departments.Add(dept);
            context.SaveChanges();

            var spec = new Specialty { Code = "09.02.07", Name = "Информационные системы и программирование", DepartmentId = dept.DepartmentId };
            context.Specialties.Add(spec);
            context.SaveChanges();

            
            var group1 = new Group { GroupName = "ИПсп-126", Course = 1, FormOfStudy = "Очная", SpecialtyId = spec.SpecialtyId };
            var group2 = new Group { GroupName = "ИПспк-226", Course = 1, FormOfStudy = "Очная", SpecialtyId = spec.SpecialtyId };
            var group3 = new Group { GroupName = "ИПсп-123", Course = 4, FormOfStudy = "Очная", SpecialtyId = spec.SpecialtyId };
            context.Groups.AddRange(group1, group2, group3);
            context.SaveChanges();

           
            context.Students.AddRange(
                new Student { LastName = "Алексеев", FirstName = "Дмитрий", MiddleName = "Сергеевич", BirthDate = new DateTime(2008, 1, 15), Gender = "М", Status = "обучается", EnrollmentDate = new DateTime(2026, 9, 1), GroupId = group1.GroupId },
                new Student { LastName = "Борисова", FirstName = "Анна", MiddleName = "Игоревна", BirthDate = new DateTime(2008, 2, 20), Gender = "Ж", Status = "обучается", EnrollmentDate = new DateTime(2026, 9, 1), GroupId = group1.GroupId },
                new Student { LastName = "Лебедев", FirstName = "Кирилл", MiddleName = "Денисович", BirthDate = new DateTime(2008, 1, 11), Gender = "М", Status = "академ", EnrollmentDate = new DateTime(2026, 9, 1), GroupId = group2.GroupId },
                new Student { LastName = "Ефимов", FirstName = "Владислав", MiddleName = "Дмитриевич", BirthDate = new DateTime(2005, 1, 1), Gender = "М", Status = "обучается", EnrollmentDate = new DateTime(2023, 9, 1), GroupId = group3.GroupId },
                new Student { LastName = "Жданова", FirstName = "Анастасия", MiddleName = "Сергеевна", BirthDate = new DateTime(2005, 2, 2), Gender = "Ж", Status = "отчислен", EnrollmentDate = new DateTime(2023, 9, 1), GroupId = group3.GroupId }
            );
            context.SaveChanges();
        }
    }
}