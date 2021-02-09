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

        public List<Employer> Employers { get; set; }
        [Required(ErrorMessage = "Please select some skills")]
        public List<int> SkillIds { get; set; }
        public List<Skill> Skills { get; set; }

        public AddJobViewModel() { }

        public AddJobViewModel(string jobName, List<Employer> employers, List<Skill> skills)
        {
            Employers = employers;
            JobName = jobName;
            Skills = skills;    
        }


    }
}
