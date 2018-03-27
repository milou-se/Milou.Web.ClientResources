@ECHO OFF
SET Arbor.X.Build.Bootstrapper.AllowPrerelease=true
SET Arbor.X.NuGet.Package.Artifacts.Suffix=
SET Arbor.X.NuGet.Package.Artifacts.BuildNumber.Enabled=
SET Arbor.X.Log.Level=Debug
SET Arbor.X.NuGetPackageVersion=
SET Arbor.X.Vcs.Branch.Name.Version.OverrideEnabled=false
SET Arbor.X.Build.VariableOverrideEnabled=true
SET Arbor.X.Artifacts.CleanupBeforeBuildEnabled=true
SET Arbor.X.Build.NetAssembly.Configuration=

SET Arbor.X.NuGet.ReinstallArborPackageEnabled=true
SET Arbor.X.NuGet.VersionUpdateEnabled=false
SET Arbor.X.Artifacts.PdbArtifacts.Enabled=true
SET Arbor.X.NuGet.Package.CreateNuGetWebPackages.Enabled=true

SET Arbor.X.Build.NetAssembly.MetadataEnabled=true
SET Arbor.X.Build.NetAssembly.Description=A convention-based build tool
SET Arbor.X.Build.NetAssembly.Company=Milou Communication AB
SET Arbor.X.Build.NetAssembly.Copyright=(C) Milou Communication AB 2015-2018
SET Arbor.X.Build.NetAssembly.Trademark=Milou Communication AB
SET Arbor.X.Build.NetAssembly.Product=Milou Web ClientResources
SET Arbor.X.ShowAvailableVariablesEnabled=false
SET Arbor.X.ShowDefinedVariablesEnabled=false
SET Arbor.X.Tools.External.MSBuild.Verbosity=minimal
SET Arbor.X.NuGet.Package.AllowManifestReWriteEnabled=false

SET Arbor.X.Tools.External.MSBuild.CodeAnalysis.Enabled=false

CALL "%~dp0\Build.exe"

REM Restore variables to default

SET Arbor.X.Build.Bootstrapper.AllowPrerelease=
SET Arbor.X.Tools.External.MSpec.Enabled=
SET Arbor.X.NuGet.Package.Artifacts.Suffix=
SET Arbor.X.NuGet.Package.Artifacts.BuildNumber.Enabled=
SET Arbor.X.Log.Level=
SET Arbor.X.NuGetPackageVersion=
SET Arbor.X.Vcs.Branch.Name.Version.OverrideEnabled=
SET Arbor.X.VariableOverrideEnabled=
SET Arbor.X.Artifacts.CleanupBeforeBuildEnabled=
SET Arbor.X.Build.NetAssembly.Configuration=