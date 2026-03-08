using project.Domain.Exceptions;

namespace project.Domain.Models
{
    public class Label
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public ICollection<TaskLabel> TaskLabels => _tasklabels.AsReadOnly();
        private readonly List<TaskLabel> _tasklabels = new();

        private Label() { }
        public static Label Create(int groupId, string name, string color = "#6B7280")
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Tên nhãn không được để trống");

            if (name.Length > 50)
                throw new DomainException("Tên nhãn không được vượt quá 50 ký tự");

            if (!IsValidHexColor(color))
                throw new DomainException("Màu phải là mã hex hợp lệ, ví dụ: #FF5733");

            return new Label
            {
                Name = name.Trim(),
                Color = color.ToUpper()
            };
        }

        public void Update(string name, string color)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Tên nhãn không được để trống");

            if (!IsValidHexColor(color))
                throw new DomainException("Màu phải là mã hex hợp lệ");

            Name = name.Trim();
            Color = color.ToUpper();
        }

        // Validate màu hex: #FFF hoặc #FFFFFF
        private static bool IsValidHexColor(string color)
            => !string.IsNullOrWhiteSpace(color)
            && color.StartsWith("#")
            && (color.Length == 4 || color.Length == 7)
            && color[1..].All(c => "0123456789ABCDEFabcdef".Contains(c));

        public bool IsUsed() => _tasklabels.Any();
    }
}
