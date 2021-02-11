using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TechJobsPersistent.Models;
using TechJobsPersistent.ViewModels;
using TechJobsPersistent.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace TechJobsPersistent.Controllers
{
    public class HomeController : Controller
    {
        private JobDbContext context;

        public HomeController(JobDbContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult Index()
        {
            List<Job> jobs = context.Jobs.Include(j => j.Employer).ToList();

            return View(jobs);
        }

        [HttpGet("/Add")]
        public IActionResult AddJob()
        {
            List<Skill> jobSkills = context.Skills.ToList();
            List<Employer> possibleEmployers = context.Employers.ToList();
            string jobName = "";
            AddJobViewModel viewModel = new AddJobViewModel(jobName, possibleEmployers, jobSkills);
            return View(viewModel);
            
        }

        [HttpPost("/ProcessAddJobForm/")]
        public IActionResult ProcessAddJobForm(AddJobViewModel viewModel, string[] selectedSkills)
        {

            if (ModelState.IsValid)
            {
                string jobName = viewModel.JobName;
                int employerId = viewModel.EmployerId;


                List<Job> existingJobs = context.Jobs
                    .Where(ej => ej.Name == jobName)
                    .Where(ej => ej.EmployerId == employerId)
                    .ToList();

                if (existingJobs.Count == 0)
                {
                    Job newJob = new Job
                    {
                        Name = jobName,
                        EmployerId = employerId
                    };
                    context.Jobs.Add(newJob);
                    context.SaveChanges();
                    int jobId = newJob.Id;
                    foreach (var skill in selectedSkills)
                    {
                        JobSkill newJoin = new JobSkill
                        {
                            JobId = jobId,
                            SkillId = int.Parse(skill)
                        };
                        context.JobSkills.Add(newJoin);
                    }
                    context.SaveChanges();
                }
                return Redirect("/Home");
            }

            return View("AddJob", viewModel);
        }

        public IActionResult Detail(int id)
        {
            Job theJob = context.Jobs
                .Include(j => j.Employer)
                .Single(j => j.Id == id);

            List<JobSkill> jobSkills = context.JobSkills
                .Where(js => js.JobId == id)
                .Include(js => js.Skill)
                .ToList();

            JobDetailViewModel viewModel = new JobDetailViewModel(theJob, jobSkills);
            return View(viewModel);
        }
    }
}
