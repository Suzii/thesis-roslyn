using BugHunter.Core.DiagnosticsFormatting.Implementation;

namespace BugHunter.Core.DiagnosticsFormatting
{
    public static class DiagnosticFormatterFactory
    {
        public static IDiagnosticFormatter CreateDefaultFormatter()
        {
            return new DefaultDiagnosticFormatter();
        }

        public static IDiagnosticFormatter CreateMemberAccessFormatter()
        {
            return new MemberAccessDiagnosticFormatter();
        }

        public static IDiagnosticFormatter CreateMemberAccessOnlyFormatter()
        {
            return new MemberAccessOnlyDiagnosticFormatter();
        }

        public static IDiagnosticFormatter CreateMemberInvocationFormatter()
        {
            return new MemberInvocationDiagnosticFormatter();
        }

        public static IDiagnosticFormatter CreateMemberInvocationOnlyFormatter(bool stripOfArgsFromMessage = false)
        {
            if (stripOfArgsFromMessage)
            {
                return new MemberInvocationOnlyNoArgsDiagnosticFormatter();
            }

            return new MemberInvocationOnlyDiagnosticFormatter();
        }

        public static IDiagnosticFormatter CreateConditionalAccessFormatter()
        {
            return new ConditionalAccessDiagnosticFormatter();
        }


        public static IDiagnosticFormatter CreateFormatter<TFormatter>()
            where TFormatter : IDiagnosticFormatter, new()
        {
            return new TFormatter();
        }
    }
}