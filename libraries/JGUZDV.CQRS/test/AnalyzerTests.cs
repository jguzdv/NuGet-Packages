using JGUZDV.CQRS.Analyzer;
using JGUZDV.CQRS.Analyzer.CodeFix;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace JGUZDV.CQRS.Tests
{
    public class AnalyzerTests
    {
        [Fact]
        public async Task ExpectedDiagnosticRegisteredCorrectly()
        {
            var context = new CSharpAnalyzerTest<CQRSAnalyzer, DefaultVerifier>();
            context.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
            context.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile("JGUZDV.CQRS.dll"));

            context.TestCode =
                """
                using JGUZDV.CQRS.Queries;

                public record TestQuery() : {|#0:IQuery<int>|}
                {
                    public QueryResult<int> Result { get; set; }
                }
                """;

            // Diagnostic gets reported twice because of how RegisterSyntaxNodeAction with SyntaxKind.RecordDeclaration works
            // so it needs to be expected twice
            context.ExpectedDiagnostics.Add(new DiagnosticResult(CQRSAnalyzer.Rule).WithLocation(0));
            context.ExpectedDiagnostics.Add(new DiagnosticResult(CQRSAnalyzer.Rule).WithLocation(0));

            await context.RunAsync();
        }

        [Fact]
        public async Task NoDiagnosticWhenIheritingQueryBase()
        {
            var context = new CSharpAnalyzerTest<CQRSAnalyzer, DefaultVerifier>();
            context.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
            context.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile("JGUZDV.CQRS.dll"));

            context.TestCode =
                """
                using JGUZDV.CQRS.Queries;

                public record TestQuery() : QueryBase<int>;
                """;

            await context.RunAsync();
        }

        [Fact]
        public async Task InvalidInheritanceofIQuery()
        {
            var context = new CSharpAnalyzerTest<CQRSAnalyzer, DefaultVerifier>();
            context.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
            context.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile("JGUZDV.CQRS.dll"));

            context.TestCode =
                """
                using JGUZDV.CQRS.Queries;

                public record TestQuery() : IQuery<int>;
                """;

            // The above Code is invalid but there shouldn't be a diagnostic reported by CQRSAnalyzer
            context.ExpectedDiagnostics.Add(
                DiagnosticResult.CompilerError("CS0535")
                    .WithSpan(3, 29, 3, 40)
                    .WithArguments("TestQuery", "JGUZDV.CQRS.Queries.IQuery<int>.Result"));

            await context.RunAsync();
        }

        [Fact]
        public async Task CodeFixProviderFixesQueryCorrectly()
        {
            var context = new CSharpCodeFixTest<CQRSAnalyzer, CQRSAnalyzerCodeFixProvider, DefaultVerifier>();
            context.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
            context.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile("JGUZDV.CQRS.dll"));

            context.TestCode =
                """
                using JGUZDV.CQRS.Queries;
                
                public record TestQuery() : {|#0:IQuery<int>|}
                {
                    public QueryResult<int> Result { get; set; }
                }
                """;

            context.FixedCode =
                """
                using JGUZDV.CQRS.Queries;
                
                public record TestQuery() : QueryBase<int>
                {
                }
                """;

            context.ExpectedDiagnostics.Add(new DiagnosticResult(CQRSAnalyzer.Rule).WithLocation(0));
            context.ExpectedDiagnostics.Add(new DiagnosticResult(CQRSAnalyzer.Rule).WithLocation(0));

            await context.RunAsync();
        }

        [Fact]
        public async Task CodeFixDoesNotTriggerIfNoDiagnosticReported()
        {
            var context = new CSharpCodeFixTest<CQRSAnalyzer, CQRSAnalyzerCodeFixProvider, DefaultVerifier>();
            context.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
            context.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile("JGUZDV.CQRS.dll"));

            context.TestCode =
                """
                using JGUZDV.CQRS.Queries;

                public record TestQuery() : IQuery<int>;
                """;

            context.ExpectedDiagnostics.Add(
                DiagnosticResult.CompilerError("CS0535")
                    .WithSpan(3, 29, 3, 40)
                    .WithArguments("TestQuery", "JGUZDV.CQRS.Queries.IQuery<int>.Result"));

            // Since the diagnostic is not reported from CQRSAnalyzer the CodeFixProvider does not do anything
            context.FixedCode =
                """
                using JGUZDV.CQRS.Queries;

                public record TestQuery() : IQuery<int>;
                """;

            await context.RunAsync();
        }
    }
}
