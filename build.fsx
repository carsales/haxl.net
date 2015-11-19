#r "tools/FAKE/tools/FakeLib.dll"

open Fake

RestorePackages()

let buildDir = "./build/"
let testDir  = "./tests/"

let packagingDir = "./nuget/"

let version =
  let major = 0
  let minor = 1
  let revision = 0
  let build = 0
  sprintf "%d.%d.%d.%d" major minor revision build

Target "Clean" (fun _ ->
  CleanDir buildDir
  CleanDir testDir
  CleanDir packagingDir
)

Target "BuildHaxl" (fun _ ->
  !! "Haxl.Net/**.csproj"
  |> MSBuildRelease buildDir "Build"
  |> Log "Build-Output: "
)

Target "BuildTests" (fun _ ->
  !! "Haxl.Net.Tests/**.csproj"
  |> MSBuildRelease testDir "Build"
  |> Log "Build-Tests: "
)

Target "RunTests" (fun _ ->
  !! (testDir + "/*.Tests.dll")
  |> NUnit (fun p ->
      {p with
        ToolPath = "./tools/nunit.runners.net4/tools"
        DisableShadowCopy = true
        OutputFile = testDir + "TestResults.xml" })
)

Target "PackageAsNugetAndPublish" (fun _ ->
    !! (buildDir + "/Haxl.Net.dll")
    |> CopyFiles packagingDir

    NuGet (fun p -> 
        {p with
            Project = "Haxl.Net"
            Version = version

            WorkingDir = packagingDir

            Files = ["Haxl.Net.dll", Some "lib/NET45", None]

            PublishUrl = "http://nuget.dev-web-01.csdev.com.au/api/"
            AccessKey = ""
            Publish = true }) 
            "Haxl.Net.nuspec"
)

Target "Default" ignore

"Clean"
  ==> "BuildHaxl"
  ==> "BuildTests"
  ==> "RunTests"
  ==> "PackageAsNugetAndPublish"
  ==> "Default"

RunTargetOrDefault "Default"