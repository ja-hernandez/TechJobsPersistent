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
            AddJobViewModel viewModel;
            if (TempData["skills"] != null )
            {
                List<Skill> jobSkills = context.Skills.ToList();
                List<Employer> possibleEmployers = context.Employers.ToList();
                string jobName = "";
                viewModel = new AddJobViewModel(jobName, possibleEmployers, jobSkills);
                viewModel.EmployerId = (int)TempData["employer"];
                viewModel.SkillIds = new List<int>();
                foreach (var skill in (string[])TempData["skills"])
                {
                    viewModel.SkillIds.Add(int.Parse(skill));
                };
                viewModel.JobName = (string)TempData["jobName"];
                return View(viewModel);
             }
            else
            {
                List<Skill> jobSkills = context.Skills.ToList();
                List<Employer> possibleEmployers = context.Employers.ToList();
                string jobName = "";
                viewModel = new AddJobViewModel(jobName, possibleEmployers, jobSkills);
                return View(viewModel);
            }
        }

        [HttpPost("/Add/")]
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
                    //context.SaveChanges();
                    int jobId = newJob.Id;
                    foreach (var skill in selectedSkills)
                    {
                        JobSkill newJoin = new JobSkill
                        {
                            JobId = jobId,
                            Job = newJob,
                            SkillId = int.Parse(skill)
                        };
                        context.JobSkills.Add(newJoin);
                    }
                    context.SaveChanges();
                }
                return Redirect("/Home");
            }
            TempData["employer"] = viewModel.EmployerId;
            TempData["skills"] = selectedSkills;
            TempData["jobName"] = viewModel.JobName;
            return RedirectToAction("AddJob", viewModel);
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
