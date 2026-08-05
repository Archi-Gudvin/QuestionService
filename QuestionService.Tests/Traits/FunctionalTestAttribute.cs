using Xunit.Sdk;

namespace QuestionService.Tests.Traits;

[TraitDiscoverer("QuestionService.Tests.Traits.FunctionalTestDiscoverer", "QuestionService.Tests")]
[AttributeUsage(AttributeTargets.Class)]
public sealed class FunctionalTestAttribute : Attribute, ITraitAttribute;
