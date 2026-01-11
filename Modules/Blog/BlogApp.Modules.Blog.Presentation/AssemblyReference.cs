using System.Reflection;

namespace BlogApp.Modules.Blog.Presentation;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}