// Guids.cs
// MUST match guids.h
using System;

namespace Adrup.CommitFormatter
{
    static class GuidList
    {
        public const string guidCommitFormatterPkgString = "b633b569-85c0-4bf2-b54e-96022d4b76c5";
        public const string guidCommitFormatterCmdSetString = "ef27c8af-194f-480f-b3eb-a098eff97d42";

        public static readonly Guid guidCommitFormatterCmdSet = new Guid(guidCommitFormatterCmdSetString);
    };
}