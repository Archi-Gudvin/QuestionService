using Xunit.Sdk;

namespace QuestionService.Tests.Traits;

[TraitDiscoverer("QuestionService.Tests.Traits.UnitTestDiscoverer", "QuestionService.Tests")]
[AttributeUsage(AttributeTargets.Class)]
public sealed class UnitTestAttribute : Attribute, ITraitAttribute;
