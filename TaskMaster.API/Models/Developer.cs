namespace TaskMaster.API.Models
{
    public class Developer
    {
        public int DeveloperId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public ICollection<ProjectTask> Tasks { get; set; }
    }
}
