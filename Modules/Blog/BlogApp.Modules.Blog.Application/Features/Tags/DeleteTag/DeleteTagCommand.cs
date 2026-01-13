namespace BlogApp.Modules.Blog.Application.Features.Tags.DeleteTag;

public sealed record DeleteTagCommand(Guid TagId) : ICommand;