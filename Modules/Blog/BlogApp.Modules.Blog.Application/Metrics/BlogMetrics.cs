namespace BlogApp.Modules.Blog.Application.Metrics;

// Define this in Application so Handlers can use it directly
public sealed class BlogMetrics
{
    // The name of the meter to subscribe to in Aspire
    public const string MeterName = "BlogApp.Modules.Blog";

    private readonly Counter<long> _articlesCreatedCounter;
    private readonly Counter<long> _articlesPublishedCounter;
    private readonly Counter<long> _commentsAddedCounter;

    public BlogMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MeterName);

        _articlesCreatedCounter = meter.CreateCounter<long>(
            "blog.articles.created",
            description: "Number of articles created");

        _articlesPublishedCounter = meter.CreateCounter<long>(
            "blog.articles.published",
            description: "Number of articles published");

        _commentsAddedCounter = meter.CreateCounter<long>(
            "blog.comments.added",
            description: "Number of comments posted");
    }

    public void ArticleCreated() => _articlesCreatedCounter.Add(1);
    public void ArticlePublished() => _articlesPublishedCounter.Add(1);
    public void CommentAdded() => _commentsAddedCounter.Add(1);
}