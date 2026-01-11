using System.Reflection;

namespace BlogApp.Modules.Blog.Application;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}