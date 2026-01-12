namespace BlogApp.Modules.Blog.Infrastructure.Auth;

internal sealed class BlogPolicyFactory : IPolicyFactory
{
    public string ModuleName => "Blog";

    public Dictionary<string, Action<AuthorizationPolicyBuilder>> GetPolicies()
    {
        return new Dictionary<string, Action<AuthorizationPolicyBuilder>>
        {
            // Only users with the "publish" claim can hit the publish endpoint
            [BlogPolicyConsts.PublishArticle] = policy =>
                policy.RequireClaim(BlogPolicyConsts.PublishArticle, "true"),

            [BlogPolicyConsts.DeleteArticle] = policy =>
                policy.RequireClaim(BlogPolicyConsts.DeleteArticle, "true"),

            // Anyone authenticated can comment (simple role check example)
            [BlogPolicyConsts.Comment] = policy =>
                policy.RequireAuthenticatedUser()
        };
    }
}