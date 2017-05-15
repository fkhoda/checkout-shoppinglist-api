var target = Argument("target", "Default");
var tag = Argument("tag", "cake");

Task("Restore").Does(() =>
{
    DotNetCoreRestore("ShoppingListService.sln");
});

Task("Build").IsDependentOn("Restore").Does(() =>
{
    DotNetCoreBuild("ShoppingListService.sln");
});

Task("Default").IsDependentOn("Build");

RunTarget(target);