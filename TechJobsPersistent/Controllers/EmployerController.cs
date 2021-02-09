using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechJobsPersistent.Data;
using TechJobsPersistent.Models;
using TechJobsPersistent.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TechJobsPersistent.Controllers
{
    public class EmployerController : Controller
    {
        // GET: /<controller>/
        private JobDbContext context;

        public EmployerController(JobDbContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult Index()
        {
            List<Employer> employers = context.Employers.ToList();
            return View(employers); 
        }

        public IActionResult Add()
        {
            AddEmployerViewModel viewModel = new AddEmployerViewModel() ;
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ProcessAddEmployerForm(AddEmployerViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string employerName = viewModel.Name;
                string employerLocation = viewModel.Location;

                List<Employer> existingEmployers = context.Employers
                    .Where(em => em.Name == employerName)
                    .Where(em => em.Location == employerLocation)
                    .ToList();

                if (existingEmployers.Count == 0)
                {
                    Employer theEmployer = new Employer
                    {
                        Name = employerName,
                        Location = employerLocation
                    };
                    context.Employers.Add(theEmployer);
                    context.SaveChanges();
                }


                return Redirect("/Employer/");
                
            }

            return View("Add");
        }

        public IActionResult About(int id)
        {
            Employer employer = context.Employers
                .Single(em => em.Id == id);
            return View(employer);
        }
    }
}
