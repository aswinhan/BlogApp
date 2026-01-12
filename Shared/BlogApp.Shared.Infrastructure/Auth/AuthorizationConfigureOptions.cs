namespace BlogApp.Shared.Infrastructure.Auth;

public class AuthorizationConfigureOptions(
    IEnumerable<IPolicyFactory> policyFactories,
    ILogger<AuthorizationConfigureOptions> logger)
    : IConfigureOptions<AuthorizationOptions>
{
    public void Configure(AuthorizationOptions options)
    {
        foreach (var factory in policyFactories)
        {
            var policies = factory.GetPolicies();
            foreach (var (policyName, configureAction) in policies)
            {
                options.AddPolicy(policyName, configureAction);
                logger.LogDebug("Registered Auth Policy '{PolicyName}' from module '{ModuleName}'", policyName, factory.ModuleName);
            }
        }
    }
}