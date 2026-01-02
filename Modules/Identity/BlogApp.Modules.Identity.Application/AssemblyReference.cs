using System.Reflection;

namespace BlogApp.Modules.Identity.Application;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}