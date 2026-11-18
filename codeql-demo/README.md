# Synthetic CodeQL alert demo

This isolated project demonstrates CodeQL query and SARIF reporting. It does not
contain a real vulnerability and is not part of the `csharp-performance` solution.
CI builds this project and runs both the standard `security-and-quality` suite and
the custom demo query. The query reports a harmless marker as a synthetic
high-severity alert; do not treat that alert as a security finding.

From the repository root in PowerShell, set `$codeql` to the path of your CodeQL
CLI executable, then run:

```powershell
$sourceRoot = (Resolve-Path .\codeql-demo).Path
$queryPack = Join-Path $sourceRoot "queries"
$db = Join-Path $env:TEMP "codeql-demo-db"
$sarif = Join-Path $env:TEMP "codeql-demo.sarif"
& $codeql pack install $queryPack
& $codeql database create $db --language=csharp "--source-root=$sourceRoot" --command="dotnet build SyntheticMarker.csproj --configuration Release --no-incremental"
& $codeql database analyze $db $queryPack --format=sarif-latest "--output=$sarif"
```

Open the resulting SARIF file in a SARIF viewer to see the synthetic alert.
