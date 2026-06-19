using BasicSocialMedia.Application.Abstractions.RequestAndResponse.Follow;
using BasicSocialMedia.Application.Abstractions.Shared;
using BasicSocialMedia.Application.IRepositories;
using BasicSocialMedia.Application.IServices;
using BasicSocialMedia.Domain.Entities;

namespace BasicSocialMedia.Application.Services
{
    public class FollowService : IFollowService
    {
        private readonly IFollowRepository _followRepository;
        private readonly ICurrentTime _currentTime;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimService _claimService;

        public FollowService(
            IFollowRepository followRepository,
            ICurrentTime currentTime,
            IUnitOfWork unitOfWork,
            IClaimService claimService)
        {
            _followRepository = followRepository;
            _currentTime = currentTime;
            _unitOfWork = unitOfWork;
            _claimService = claimService;
        }

        public async Task<List<Follow>> GetFollowingStatsAsync (Guid followerId, Guid followingId)
        {
            return await _followRepository.GetFollowingStatsAsync(followerId, followingId);
        }

        public async Task<Result<FollowResponse>> FollowUserAsync(Guid followingId)
        {
            var followerId = _claimService.GetCurrentUserId;
            if (followerId == Guid.Empty)
            {
                return FailedFollow("Invalid token.");
            }

            if (followingId == Guid.Empty)
            {
                return FailedFollow("Following user id is required.");
            }

            if (followerId == followingId)
            {
                return FailedFollow("You cannot follow yourself.");
            }

            var followingUser = await _unitOfWork.UserRepository.GetUserById(followingId);
            if (followingUser == null || followingUser.IsDeleted)
            {
                return FailedFollow("Following user was not found.");
            }

            var existingFollows = await _followRepository.FindAsync(follow =>
                follow.FollowerId == followerId
                && follow.FollowingId == followingId);
            var existingFollow = existingFollows.FirstOrDefault();
            if (existingFollow is { IsDeleted: false })
            {
                return new Result<FollowResponse>
                {
                    Error = 0,
                    Message = "You are already following this user.",
                    Data = ToFollowResponse(existingFollow)
                };
            }

            if (existingFollow is { IsDeleted: true })
            {
                existingFollow.IsDeleted = false;
                existingFollow.DeletionDate = null;
                existingFollow.DeleteBy = null;

                _followRepository.Update(existingFollow);
                await _unitOfWork.SaveChangeAsync();

                return new Result<FollowResponse>
                {
                    Error = 0,
                    Message = "Followed user successfully.",
                    Data = ToFollowResponse(existingFollow)
                };
            }

            var follow = new Follow
            {
                Id = Guid.NewGuid(),
                FollowerId = followerId,
                FollowingId = followingId
            };

            await _followRepository.AddAsync(follow);
            await _unitOfWork.SaveChangeAsync();

            return new Result<FollowResponse>
            {
                Error = 0,
                Message = "Followed user successfully.",
                Data = ToFollowResponse(follow)
            };
        }

        public async Task<Result<FollowResponse>> UnfollowUserAsync(Guid followingId)
        {
            var followerId = _claimService.GetCurrentUserId;
            if (followerId == Guid.Empty)
            {
                return FailedFollow("Invalid token.");
            }

            if (followingId == Guid.Empty)
            {
                return FailedFollow("Following user id is required.");
            }

            if (followerId == followingId)
            {
                return FailedFollow("You cannot unfollow yourself.");
            }

            var followingUser = await _unitOfWork.UserRepository.GetUserById(followingId);
            if (followingUser == null || followingUser.IsDeleted)
            {
                return FailedFollow("Following user was not found.");
            }

            var existingFollow = (await _followRepository.FindAsync(follow =>
                    !follow.IsDeleted
                    && follow.FollowerId == followerId
                    && follow.FollowingId == followingId))
                .FirstOrDefault();

            if (existingFollow == null)
            {
                return FailedFollow("You are not following this user.");
            }

            var response = ToFollowResponse(existingFollow);
            _followRepository.SoftRemove(existingFollow);
            await _unitOfWork.SaveChangeAsync();

            return new Result<FollowResponse>
            {
                Error = 0,
                Message = "Unfollowed user successfully.",
                Data = response
            };
        }

        public async Task<Result<FollowStatusResponse>> GetFollowStatusAsync(Guid targetUserId)
        {
            var currentUserId = _claimService.GetCurrentUserId;
            if (currentUserId == Guid.Empty)
            {
                return new Result<FollowStatusResponse>
                {
                    Error = 1,
                    Message = "Invalid token."
                };
            }

            if (targetUserId == Guid.Empty)
            {
                return FailedStatus("Target user id is required.");
            }

            var targetUser = await _unitOfWork.UserRepository.GetUserById(targetUserId);
            if (targetUser == null || targetUser.IsDeleted)
            {
                return FailedStatus("Target user was not found.");
            }

            var currentUser = await _unitOfWork.UserRepository.GetUserById(currentUserId);
            if (currentUser == null || currentUser.IsDeleted)
            {
                return FailedStatus("Current user was not found.");
            }

            var isFollowing = (await _followRepository.GetFollowingStatsAsync(currentUserId, targetUserId)).Any();
            var isFollowedBy = (await _followRepository.GetFollowingStatsAsync(targetUserId, currentUserId)).Any();
            var followers = await _followRepository.GetFollowersAsync(targetUserId);
            var following = await _followRepository.GetFollowingAsync(targetUserId);

            return new Result<FollowStatusResponse>
            {
                Error = 0,
                Message = "Follow status retrieved successfully.",
                Data = new FollowStatusResponse
                {
                    Followers = new FollowUsersResponse
                    {
                        Count = followers.Count,
                        Users = followers
                            .Select(follow => ToUserSummary(follow.Follower))
                            .ToList()
                    },
                    Following = new FollowUsersResponse
                    {
                        Count = following.Count,
                        Users = following
                            .Select(follow => ToUserSummary(follow.Following))
                            .ToList()
                    }
                }
            };
        }

        private static FollowResponse ToFollowResponse(Follow follow)
        {
            return new FollowResponse
            {
                FollowId = follow.Id,
                FollowerId = follow.FollowerId,
                FollowingId = follow.FollowingId,
                CreationDate = follow.CreationDate
            };
        }

        private static FollowUserSummaryResponse ToUserSummary(User user)
        {
            return new FollowUserSummaryResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                ProfilePictureUrl = user.ProfilePicture?.FileUrl
            };
        }

        private static Result<FollowResponse> FailedFollow(string message)
        {
            return new Result<FollowResponse>
            {
                Error = 1,
                Message = message
            };
        }

        private static Result<FollowStatusResponse> FailedStatus(string message)
        {
            return new Result<FollowStatusResponse>
            {
                Error = 1,
                Message = message
            };
        }
    }
}
