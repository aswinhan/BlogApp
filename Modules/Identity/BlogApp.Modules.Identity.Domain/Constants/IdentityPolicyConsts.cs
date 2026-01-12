namespace BlogApp.Modules.Identity.Domain.Constants;

public static class IdentityPolicyConsts
{
    // These strings act as both the "Policy Name" and the "Claim Value"
    public const string ReadUsers = "identity:users:read";
    public const string WriteUsers = "identity:users:write";
    public const string DeleteUsers = "identity:users:delete";
}