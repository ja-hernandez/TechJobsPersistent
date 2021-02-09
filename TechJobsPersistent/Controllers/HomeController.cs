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
            AddJobViewModel viewModel = new AddJobViewModel();
            viewModel.Employers = context.Employers.ToList();
            viewModel.Skills = context.Skills.ToList();
            return View(viewModel);
        }

        [HttpPost("/ProcessAddJobForm/")]
        public IActionResult ProcessAddJobForm(AddJobViewModel viewModel, string employer, string[] selectedSkills)
        {
            viewModel.EmployerId = int.Parse(employer);
            viewModel.SkillIds = new List<int>();
            foreach(string selectedSkill in selectedSkills)
            {
                viewModel.SkillIds.Add(int.Parse(selectedSkill));
            };
            string jobName = viewModel.JobName;

            if (ModelState.IsValid)
            {
                job




                List<Job> existingJob = context.Jobs
                    .Where(jo => jo.Name == jobName)
                    .Where(jo => jo.EmployerId == employerId)
                    .ToList();
                if (existingJob.Count == 0)
                {
                    Job theJob = new Job
                    {
                        Name = jobName,
                        EmployerId = employerId
                    };
                    foreach (string skill in selectedSkills)
                    {
                        JobSkill jobSkill = new JobSkill();
                        jobSkill.JobId = theJob.Id;
                        jobSkill.Job = theJob;
                        jobSkill.SkillId = int.Parse(skill);
                        jobSkill.Skill = context.Skills.Find(jobSkill.SkillId);
                        context.JobSkills.Add(jobSkill);
                    }
                    context.Jobs.Add(theJob);
                    context.SaveChanges();
                }
                return Redirect("/Home/");
            }
            return View("Add");              
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
