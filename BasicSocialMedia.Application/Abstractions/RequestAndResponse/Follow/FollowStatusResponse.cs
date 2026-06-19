namespace BasicSocialMedia.Application.Abstractions.RequestAndResponse.Follow;

public class FollowStatusResponse
{
    public FollowUsersResponse Followers { get; set; } = new();
    public FollowUsersResponse Following { get; set; } = new();
}

public class FollowUsersResponse
{
    public int Count { get; set; }
    public List<FollowUserSummaryResponse> Users { get; set; } = new();
}

public class FollowUserSummaryResponse
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
}
