namespace BasicSocialMedia.Application.Abstractions.RequestAndResponse.Follow;

public class FollowResponse
{
    public Guid FollowId { get; set; }
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
    public DateTime CreationDate { get; set; }
}
