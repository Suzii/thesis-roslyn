using BugHunter.Core.DiagnosticFormatting.Implementation;

namespace BugHunter.Core.DiagnosticFormatting
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