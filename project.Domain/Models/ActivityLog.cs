using project.Domain.Exceptions;

namespace project.Domain.Models
{
    public class ActivityLog
    {
        public int Id { get; private set; }
        public int UserId { get; private set; } 
        public int GroupId { get; private set; }
        public ActionType ActionType { get; private set; }
        public int? RelatedEntityId { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public UserApp User { get; private set; }
        public Groups Group { get; private set; }


        private ActivityLog() { }

        public static ActivityLog Create(int userId, int groupId, ActionType actionType,
                             int? relatedEntityId = null)
        {
    
            return new ActivityLog
            {
                UserId = userId,
                GroupId = groupId,
                ActionType = actionType,
                RelatedEntityId = relatedEntityId,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
