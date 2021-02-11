using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TechJobsPersistent.Models;

namespace TechJobsPersistent.ViewModels
{
    public class AddJobViewModel
    {
        [Required(ErrorMessage="Must enter job name")]
        public string JobName { get; set; }
        [Required(ErrorMessage ="Must select employer")]
        public int EmployerId { get; set; }

        public List<SelectListItem> Employers { get; set; }

        public List<Skill> Skills { get; set; }
        public List<int> SkillIds { get; set; }

        public AddJobViewModel() {}

        public AddJobViewModel(string jobName, List<Employer> possibleEmployers, List<Skill> jobSkills)
        {
            JobName = jobName;
            Employers = new List<SelectListItem>();
            foreach (var employer in possibleEmployers)
            {
                Employers.Add(new SelectListItem
                {
                    Value = employer.Id.ToString(),
                    Text = employer.Name
                });
            }

            Skills = jobSkills;
        }


    }
}
