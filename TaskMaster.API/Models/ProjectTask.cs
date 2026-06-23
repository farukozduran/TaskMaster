namespace TaskMaster.API.Models
{
    public class ProjectTask
    {
        public int ProjectTaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public int? DeveloperId { get; set; }
        public Developer Developer { get; set; }
    }
}
