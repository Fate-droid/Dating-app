using System;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IMemberRepository
{
    void Update(Member member);

    Task<bool> SaveAllAsync();

    Task<IReadOnlyList<Member>> GetMembersAsync(PagingParams pagingParams);
    Task<Member?> GetMemberByIDAsync(string id);

    Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string MemberId);

    Task<Member?> GetmemberforUpdate(string id);
}
