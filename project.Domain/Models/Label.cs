namespace project.Domain.Models
{
    public class Label
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public ICollection<TaskLabel> TaskLabels { get; set; }
    }
}
