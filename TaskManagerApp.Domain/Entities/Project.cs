using System.Collections.Generic;

namespace TaskManagerApp.Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? Deadline { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string OwnerId { get; set; }
        public User Owner { get; set; }
        public ICollection<UserProject>? UserProjects { get; set; }
        public ICollection<Task>? Tasks { get; set; }
    }
}

