using BugHunter.Core.DiagnosticsFormatting.Implementation;

namespace BugHunter.Core.DiagnosticsFormatting
{
    public static class DiagnosticFormatterFactory
    {
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

        public static IDiagnosticFormatter CreateMemberInvocationOnlyFormatter()
        {
            return new MemberInvocationOnlyDiagnosticFormatter();
        }
    }
}