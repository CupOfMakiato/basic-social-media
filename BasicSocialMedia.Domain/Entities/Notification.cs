namespace BasicSocialMedia.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public string NotificationContent { get; set; }
        public bool IsRead { get; set; } = false;
        public bool IsSent { get; set; } = false;
    }
}