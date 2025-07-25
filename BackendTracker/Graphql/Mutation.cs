﻿using BackendTracker.Context;
using BackendTracker.Entities.ApplicationUser;
using BackendTracker.GraphQueries.GraphqlTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BackendTracker.Graphql;

public class Mutation(IDbContextFactory<ApplicationContext> dbContextFactory)
{
    public async Task<ApplicationUser?> CreateUser(ApplicationUserInput user)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        try
        {
            var hasher = new PasswordHasher<ApplicationUser>();

            var applicationUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role ?? "User",
                RefreshToken = "",
                RefreshTokenExpiryTime = DateTime.Now.AddHours(24).ToUniversalTime(),
                LastLoginTime = DateTime.Now.ToUniversalTime(),
                IsOnline = false
            };

            applicationUser.Password = hasher.HashPassword(applicationUser,
                user.Password ?? throw new InvalidOperationException("Password cannot be null"));

            context.ApplicationUsers.Add(applicationUser);
            await context.SaveChangesAsync();
            return applicationUser;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
