using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagerApp.Domain.Entities
{
    public class User : IdentityUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public RefreshToken? RefreshToken { get; set; }
        public ICollection<Project>? OwnedProjects { get; set; }
        public ICollection<UserProject>? UserProjects { get; set; }
        public ICollection<Task>? CreatedTasks { get; set; }
        public ICollection<Task>? AssignedTasks { get; set; }
        public ICollection<TaskOperationRequest>? TaskOperationRequests { get; set; }

    }
}
