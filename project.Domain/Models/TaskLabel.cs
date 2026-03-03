namespace project.Domain.Models
{
    public class  TaskLabel
    {
        public int TaskId { get; set; }
        public int LabelId { get; set; }
        public WorkTask Task { get; set; }
        public Label Label { get; set; }

    }
}
