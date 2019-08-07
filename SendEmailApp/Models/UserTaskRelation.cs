using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SendEmailApp.Models
{
    public class UserTaskRelation
    {
        public int Id { get; set; }

        public UserTask UserTask { get; set; }
        public int UserTaskId { get; set; }

        public AppUser AppUser { get; set; }
        public string AppUserId { get; set; }
        
    }
}
