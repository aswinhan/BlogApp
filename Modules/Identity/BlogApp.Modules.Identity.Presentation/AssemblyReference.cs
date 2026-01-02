using System.Reflection;

namespace BlogApp.Modules.Identity.Presentation;

// This class exists solely to let the Web API know "This is the Identity Presentation Assembly"
public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}