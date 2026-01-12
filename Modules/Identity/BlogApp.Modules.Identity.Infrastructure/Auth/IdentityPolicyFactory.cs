namespace BlogApp.Modules.Identity.Infrastructure.Auth;

internal sealed class IdentityPolicyFactory : IPolicyFactory
{
    public string ModuleName => "Identity";

    public Dictionary<string, Action<AuthorizationPolicyBuilder>> GetPolicies()
    {
        return new Dictionary<string, Action<AuthorizationPolicyBuilder>>
        {
            // Policy: User needs "identity:users:read" claim with value "true"
            [IdentityPolicyConsts.ReadUsers] = policy =>
                policy.RequireClaim(IdentityPolicyConsts.ReadUsers, "true"),

            [IdentityPolicyConsts.WriteUsers] = policy =>
                policy.RequireClaim(IdentityPolicyConsts.WriteUsers, "true"),

            [IdentityPolicyConsts.DeleteUsers] = policy =>
                policy.RequireClaim(IdentityPolicyConsts.DeleteUsers, "true")
        };
    }
}
