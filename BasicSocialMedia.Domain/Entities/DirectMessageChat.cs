namespace BasicSocialMedia.Domain.Entities
{
    public class DirectMessageChat : BaseEntity
    {
        // this is DMs meaning 2 users, but i still figure out how to do large group chat
        public Guid ParticipantOneId { get; set; }
        public User ParticipantOne { get; set; } = null!;

        public Guid ParticipantTwoId { get; set; }
        public User ParticipantTwo { get; set; } = null!;

        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}