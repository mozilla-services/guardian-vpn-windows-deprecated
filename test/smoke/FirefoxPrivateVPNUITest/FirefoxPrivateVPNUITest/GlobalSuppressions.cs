// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Build", "SA1636:The file header copyright text should match the copyright text from the settings.", Justification = "Copyright text contents may change occasionally, no easy way to suppress per-file")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Build", "SA1633:The file header XML is invalid.", Justification = "the file header xml is valid.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "there is no need to implement IDispose since the test will auto run clean up method to dispose.")]