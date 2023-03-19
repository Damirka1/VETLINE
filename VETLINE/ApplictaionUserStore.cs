//namespace VETLINE
//{
//	using System;
//	using System.Threading;
//	using System.Threading.Tasks;
//	using Microsoft.AspNetCore.Identity;
//	using Microsoft.EntityFrameworkCore;
//	using VETLINE.Models;

//	public class ApplicationUserStore : IUserStore<ApplicationUser>
//	{
//		private readonly DatabaseContext _context;

//		public ApplicationUserStore(DatabaseContext context)
//		{
//			_context = context;
//		}

//		public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
//		{
//			_context.ApplicationUsers.Add(user);
//			await _context.SaveChangesAsync(cancellationToken);
//			return IdentityResult.Success;
//		}

//		public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
//		{
//			_context.ApplicationUsers.Update(user);
//			await _context.SaveChangesAsync(cancellationToken);
//			return IdentityResult.Success;
//		}

//		public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
//		{
//			_context.ApplicationUsers.Remove(user);
//			await _context.SaveChangesAsync(cancellationToken);
//			return IdentityResult.Success;
//		}

//		public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
//		{
//			return await _context.ApplicationUsers.FindAsync(new object[] { userId }, cancellationToken);
//		}

//		public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
//		{
//			return await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName, cancellationToken);
//		}

//		public void Dispose()
//		{
//			// nothing to dispose
//		}

//		public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
//		{
//			return Task.FromResult(user.Id);
//		}

//		public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
//		{
//			return Task.FromResult(user.UserName);
//		}

//		public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
//		{
//			user.UserName = userName;
//			return Task.CompletedTask;
//		}

//		public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
//		{
//			return Task.FromResult(user.NormalizedUserName);
//		}

//		public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
//		{
//			user.NormalizedUserName = normalizedName;
//			return Task.CompletedTask;
//		}
//	}

//}
