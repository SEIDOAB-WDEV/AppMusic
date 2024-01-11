using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Models;

namespace AppMusicRazor.ModelAuthorization
{
    public static class csMusicGroupOperations
    {
        public static OperationAuthorizationRequirement Create =
            new() { Name = nameof(Create) };
        public static OperationAuthorizationRequirement Read =
            new() { Name = nameof(Read) };
        public static OperationAuthorizationRequirement Edit =
            new() { Name = nameof(Edit) };
        public static OperationAuthorizationRequirement Delete =
            new() { Name = nameof(Delete) };
    }

    public class csMusicGroupAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, csMusicGroup>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement, csMusicGroup resource)
        {
            //Here I simply allow a logged in User to do all operations
            if (!context.User.Identity.IsAuthenticated)
            {
                return Task.CompletedTask;
            }

            //But I can customize Succeed based on
            //requirement, context.User and resource
            if (requirement.Name == csMusicGroupOperations.Create.Name)
            {
                context.Succeed(requirement);
            }
            if (requirement.Name == csMusicGroupOperations.Read.Name)
            {
                context.Succeed(requirement);
            }
            if (requirement.Name == csMusicGroupOperations.Edit.Name)
            {
                context.Succeed(requirement);
            }
            if (requirement.Name == csMusicGroupOperations.Delete.Name)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}

