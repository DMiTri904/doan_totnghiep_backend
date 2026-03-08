namespace project.Domain.Models
{
    public class  TaskLabel
    {
        public int TaskId { get; private set; }
        public int LabelId { get; private set; }
        public WorkTask Task { get; private set; }
        public Label Label { get; private set; }

        private TaskLabel() { }
        public static TaskLabel Create(int taskId, int labelId, int assignedBy)
        {
            return new TaskLabel
            {
                TaskId = taskId,
                LabelId = labelId,
            };
        }
    }
}
