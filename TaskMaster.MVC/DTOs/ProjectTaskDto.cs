namespace TaskMaster.MVC.DTOs
{
    public class ProjectTaskDto
    {
        public int ProjectTaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public int? DeveloperId { get; set; }
        public string? DeveloperFullName { get; set; }
    }
}
