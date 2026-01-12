namespace BlogApp.Shared.Infrastructure.Auth;

public interface IPolicyFactory
{
    /// <summary>
    /// The name of the module (e.g., "Blog", "Identity").
    /// </summary>
    string ModuleName { get; }

    /// <summary>
    /// Returns a dictionary of Policies.
    /// Key = Policy Name (e.g., "articles:delete")
    /// Value = The Action to configure the policy
    /// </summary>
    Dictionary<string, Action<AuthorizationPolicyBuilder>> GetPolicies();
}