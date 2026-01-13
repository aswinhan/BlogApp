namespace BlogApp.Modules.Blog.Application.Features.Tags.DeleteTag;

internal sealed class DeleteTagHandler(IBlogDbContext context)
    : ICommandHandler<DeleteTagCommand>
{
    public async Task<Result> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await context.Tags
            .FirstOrDefaultAsync(t => t.Id == request.TagId, cancellationToken);

        if (tag is null)
        {
            return Result.Failure(Error.NotFound("Tag.NotFound", "Tag not found"));
        }

        // Hard Delete is usually fine for Tags, 
        // OR you can use SoftDelete if your Tag entity supports it.
        context.Tags.Remove(tag);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}