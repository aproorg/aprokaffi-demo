#Requires -Version 7.0
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$pluginRoot = Split-Path $PSScriptRoot -Parent
$templateRoot = Join-Path $pluginRoot 'skills\bootstrap\assets\templates'
$manifest = Get-Content -LiteralPath (Join-Path $templateRoot 'template-manifest.json') -Raw | ConvertFrom-Json
$plugin = Get-Content -LiteralPath (Join-Path $pluginRoot 'plugin.json') -Raw | ConvertFrom-Json
$script:assertions = 0

function Assert-That([bool] $Condition, [string] $Message) {
    $script:assertions++
    if (-not $Condition) { throw $Message }
}

function Assert-Matches([string] $Text, [string] $Pattern, [string] $Message) {
    Assert-That ($Text -match $Pattern) $Message
}

function Read-Template([string] $Path) {
    Get-Content -LiteralPath (Join-Path $templateRoot $Path) -Raw
}

function Render-Template([string] $Text, [hashtable] $Values) {
    [regex]::Replace($Text, '\{\{([a-z_]+)\}\}', [System.Text.RegularExpressions.MatchEvaluator] {
        param($Token)
        $name = $Token.Groups[1].Value
        if (-not $Values.ContainsKey($name)) { throw "Unknown variable: $name" }
        [string] $Values[$name]
    })
}

function Read-ProfileHeader([string] $Text) {
    $header = [regex]::Match($Text, '\A---\r?\n(.*?)\r?\n---\r?\n', 'Singleline')
    Assert-That $header.Success 'Missing YAML frontmatter'
    $fields = @{}
    foreach ($line in ($header.Groups[1].Value -split '\r?\n')) {
        # Profiles intentionally use only scalar values and single-line tool lists.
        $field = [regex]::Match($line, '^([a-z][a-z-]*): (.+)$')
        Assert-That $field.Success "Unexpected profile metadata shape: $line"
        $key = $field.Groups[1].Value
        Assert-That (-not $fields.ContainsKey($key)) "Duplicate profile metadata: $key"
        $fields[$key] = $field.Groups[2].Value
    }
    $fields
}

$expectedAgents = @(
    'feature-orchestrator', 'feature-planner', 'implementation-agent',
    'test-implementer', 'verification-agent', 'retro-agent'
)
$expectedTools = @{
    'feature-orchestrator' = "['agent', 'read', 'search', 'edit', 'execute', 'example/*']"
    'feature-planner' = "['read', 'search']"
    'implementation-agent' = "['read', 'search', 'edit']"
    'test-implementer' = "['read', 'search', 'edit', 'execute']"
    'verification-agent' = "['read', 'search']"
    'retro-agent' = "['read', 'search']"
}
$expectedModels = @{
    'feature-planner' = 'gpt-6.1-sol'
    'implementation-agent' = 'gpt-6-luna'
    'test-implementer' = 'gpt-6-luna'
}
$expectedSkills = @('feature-implementation', 'implement-jira-ticket', 'test-implementation')
$marketplaceRoot = Split-Path (Split-Path $pluginRoot -Parent) -Parent
$marketplace = Get-Content -LiteralPath (Join-Path $marketplaceRoot '.github\plugin\marketplace.json') -Raw | ConvertFrom-Json
foreach ($entry in $marketplace.plugins) {
    $source = Join-Path $marketplaceRoot $entry.source
    $metadata = Get-Content -LiteralPath (Join-Path $source 'plugin.json') -Raw | ConvertFrom-Json
    Assert-That ($metadata.id -eq $entry.id) "Marketplace/plugin ID mismatch: $($entry.id)"
    foreach ($component in $metadata.components.PSObject.Properties) {
        foreach ($path in $component.Value) {
            Assert-That (Test-Path -LiteralPath (Join-Path $source $path)) "Missing $($entry.id) component: $path"
        }
    }
}
$mappedSources = @($manifest.generatedFileMap.PSObject.Properties.Name)
$actualSources = @(Get-ChildItem -LiteralPath $templateRoot -Filter '*.tmpl' -Recurse -File |
    ForEach-Object { [System.IO.Path]::GetRelativePath($templateRoot, $_.FullName).Replace('\', '/') })

Assert-That ($manifest.version -eq $plugin.version) 'Template/plugin release versions disagree'
Assert-That ($manifest.version -eq '0.3.0') 'Unexpected contract release'
Assert-That (@(Compare-Object $manifest.files $mappedSources).Count -eq 0) 'File map and inventory disagree'
Assert-That (@(Compare-Object $actualSources $mappedSources).Count -eq 0) 'Unmapped or missing templates'
Assert-That (@($mappedSources | Where-Object { $_ -like '*feature-implementer*' }).Count -eq 0) 'Duplicate implementation role'
$destinations = @($manifest.generatedFileMap.PSObject.Properties.Value)
Assert-That (@($destinations | Select-Object -Unique).Count -eq $destinations.Count) 'Duplicate output paths'
$usedVariables = @($mappedSources | ForEach-Object {
    [regex]::Matches((Read-Template $_), '\{\{([a-z_]+)\}\}') |
        ForEach-Object { $_.Groups[1].Value }
} | Sort-Object -Unique)
Assert-That (@(Compare-Object $manifest.variables.PSObject.Properties.Name $usedVariables).Count -eq 0) 'Declared and used variables disagree'

$fixtures = @(
    @{
        project_name = 'DemoTaskApi'
        project_description = '.NET API: tasks and owners.'
        build_command = 'dotnet build'
        test_command = 'dotnet test'
        test_filter_example = 'dotnet test --filter FullyQualifiedName~TaskTests'
        test_path_glob = '**/*.Tests/**'
        run_command = 'dotnet run'
        architecture_notes = "Minimal API entry point.`n`nServices keep state in memory."
        test_conventions = 'xUnit tests in DemoTaskApi.Tests.'
        security_context_description = 'Untrusted request input; no authentication or persistence.'
        ticket_source_description = 'Example tracker, via the connected example MCP server.'
        ticket_tools = "'example/*'"
        template_repo_location = '../test-copilot-marketplace (plugins/workshop-bootstrap/skills/bootstrap/assets/templates)'
    },
    @{
        project_name = 'web-app'
        project_description = 'JavaScript application: browser client.'
        build_command = ''
        test_command = 'npm test'
        test_filter_example = 'npm test -- --test-name-pattern="empty input"'
        test_path_glob = '**/*.test.js'
        run_command = 'npm start'
        architecture_notes = 'A browser entry point and existing request helpers.'
        test_conventions = 'Built-in tests; follow the existing .test.js files.'
        security_context_description = 'External API input; no stored secrets.'
        ticket_source_description = 'Example tracker, via the connected example MCP server.'
        ticket_tools = "'example/*'"
        template_repo_location = '../test-copilot-marketplace (plugins/workshop-bootstrap/skills/bootstrap/assets/templates)'
    }
)

$fixtureRoot = Join-Path ([System.IO.Path]::GetTempPath()) ("workshop-bootstrap-tests-" + [guid]::NewGuid().ToString('N'))
try {
    foreach ($index in 0..($fixtures.Count - 1)) {
        $values = $fixtures[$index]
        Assert-That (@(Compare-Object $manifest.variables.PSObject.Properties.Name @($values.Keys)).Count -eq 0) 'Fixture variable drift'
        foreach ($layout in @('.', 'copilot\phase-1')) {
            $repoRoot = Join-Path $fixtureRoot "$index-$($layout -eq '.')"
            $projectRoot = [System.IO.Path]::GetFullPath((Join-Path $repoRoot $layout))
            $harnessRoot = Join-Path $projectRoot '.github'
            if ($layout -ne '.') {
                $decoy = Join-Path $repoRoot '.github\copilot-instructions.md'
                New-Item -ItemType Directory -Path (Split-Path $decoy -Parent) -Force | Out-Null
                Set-Content -LiteralPath $decoy -Value 'Git-root decoy: do not overwrite'
            }
            foreach ($entry in $manifest.generatedFileMap.PSObject.Properties) {
                Assert-Matches $entry.Value '^\.github/' 'Output is not project-root-relative'
                $output = [System.IO.Path]::GetFullPath((Join-Path $projectRoot $entry.Value))
                Assert-That ($output.StartsWith($harnessRoot + [System.IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) 'Output escaped harness root'
                $rendered = Render-Template (Read-Template $entry.Name) $values
                Assert-That ($rendered -notmatch '\{\{|\}\}') "Unresolved token in $($entry.Name)"
                Assert-That ($rendered -notmatch 'C:\\Users\\|/Users/|/home/') 'Machine-specific root leaked'
                New-Item -ItemType Directory -Path (Split-Path $output -Parent) -Force | Out-Null
                Set-Content -LiteralPath $output -Value $rendered -NoNewline
            }
            $marker = @{
                templateSet = 'workshop-bootstrap'
                version = $manifest.version
                variables = $values
                generatedFiles = $destinations
                projectPathFromGitRoot = $layout.Replace('\', '/')
                harnessPath = '.github'
            }
            $markerPath = Join-Path $harnessRoot '.bootstrap-manifest.json'
            $marker | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $markerPath
            $roundTrip = Get-Content -LiteralPath $markerPath -Raw | ConvertFrom-Json
            Assert-That ($roundTrip.projectPathFromGitRoot -eq $layout.Replace('\', '/')) 'Project location marker drift'
            Assert-That ($roundTrip.harnessPath -eq '.github') 'Harness location marker drift'
            Assert-That ($roundTrip.variables.test_filter_example -eq $values.test_filter_example) 'JSON variable roundtrip failed'
            Assert-That (@(Get-ChildItem -LiteralPath $harnessRoot -Recurse -File).Count -eq ($mappedSources.Count + 1)) 'Wrong generated file count'
            if ($layout -ne '.') {
                Assert-That ((Get-Content -LiteralPath $decoy -Raw).Trim() -eq 'Git-root decoy: do not overwrite') 'Nested generation overwrote Git-root instructions'
            }
            $agents = @(Get-ChildItem -LiteralPath (Join-Path $harnessRoot 'agents') -File)
            Assert-That (@(Compare-Object $expectedAgents @($agents | ForEach-Object { $_.Name.Replace('.agent.md', '') })).Count -eq 0) 'Agent inventory drift'
            Assert-That (@(Compare-Object $expectedSkills @(Get-ChildItem -LiteralPath (Join-Path $harnessRoot 'skills') -Directory | ForEach-Object Name)).Count -eq 0) 'Skill inventory drift'
            foreach ($agent in $agents) {
                $text = Get-Content -LiteralPath $agent.FullName -Raw
                $header = Read-ProfileHeader $text
                $name = $header.name
                Assert-That ($header.description.Length -gt 20 -and $header.description -notmatch ': ') 'Invalid plain YAML description'
                Assert-That ($header['include-custom-instructions'] -eq 'true') "Repo instructions disabled: $name"
                Assert-That (-not $header.ContainsKey('agents')) "Undocumented dispatch allowlist: $name"
                Assert-That ($header.tools -eq $expectedTools[$name]) "Tool envelope drift: $name"
                if ($expectedModels.ContainsKey($name)) {
                    Assert-That ($header.model -eq $expectedModels[$name]) "Noncanonical model: $name"
                    Assert-Matches $header.model '^[a-z0-9]+(?:[.-][a-z0-9]+)*$' "Model must be a scalar ID: $name"
                } else {
                    Assert-That (-not $header.ContainsKey('model')) "Unexpected model pin: $name"
                }
                Assert-Matches $text 'harness_root' "Missing absolute root input: $name"
                Assert-That ($text -notmatch '\(preloaded\)') "Inaccurate skill acknowledgement: $name"
                if ($name -ne 'feature-orchestrator') {
                    Assert-Matches $text '(?i)terminal' "Missing terminal worker contract: $name"
                    foreach ($field in @('status', 'outputs', 'evidence', 'blocker')) {
                        Assert-Matches $text $field "Missing result field $field in $name"
                    }
                }
                foreach ($link in [regex]::Matches($text, '\]\((\.\./skills/[^)]+)\)')) {
                    Assert-That (Test-Path -LiteralPath (Join-Path $agent.DirectoryName $link.Groups[1].Value)) "Broken worker skill reference: $name"
                }
            }
            $testsText = Get-Content -LiteralPath (Join-Path $harnessRoot 'instructions\tests.instructions.md') -Raw
            Assert-That ($testsText.Contains('applyTo: "' + $values.test_path_glob + '"')) 'Valid test glob changed during rendering'
        }
    }
    $unknownRejected = $false
    try { Render-Template '{{missing_variable}}' @{} | Out-Null } catch { $unknownRejected = $true }
    Assert-That $unknownRejected 'Renderer accepted an unknown variable'
} finally {
    if (Test-Path -LiteralPath $fixtureRoot) {
        Remove-Item -LiteralPath $fixtureRoot -Recurse -Force
    }
}

$coordinator = Read-Template 'agents\feature-orchestrator.agent.md.tmpl'
$common = Read-Template 'copilot-instructions.md.tmpl'
$planner = Read-Template 'agents\feature-planner.agent.md.tmpl'
$implementer = Read-Template 'agents\implementation-agent.agent.md.tmpl'
$tester = Read-Template 'agents\test-implementer.agent.md.tmpl'
$verifier = Read-Template 'agents\verification-agent.agent.md.tmpl'
$retro = Read-Template 'agents\retro-agent.agent.md.tmpl'
$bootstrap = Get-Content -LiteralPath (Join-Path $pluginRoot 'skills\bootstrap\SKILL.md') -Raw

Assert-That ($coordinator.IndexOf('## Single execution owner') -lt $coordinator.IndexOf('## Routing')) 'Ownership must precede routing'
foreach ($rule in @(
    'Never spawn another feature-orchestrator', 'Do not perform a worker',
    'Human confirmation or restored authentication resumes this run',
    'Do not substitute another worker', 'Declarations and file existence prove discovery only',
    'not a ticket read/search', 'before confirmation', 'one successful fetch',
    'Finish all five probes before fetching the ticket', 'same canonical',
    'cannot simulate direct selection', 'one attempt per required profile',
    'one explicit requalification', 'actual required file changes',
    'No changes is not success', 'exit code 0', 'Do not verify known-broken code',
    'prevents completion', 'at most one corrective attempt', 'production-remediation',
    'Skill acknowledgement alone never passes', 'private ticket text',
    'including self-delegation or wrong-root attempts', 'OS temporary directory'
)) {
    Assert-Matches $coordinator ([regex]::Escape($rule)) "Missing coordinator rule: $rule"
}
Assert-Matches $coordinator '(?s)implementation plan only.*?No raw ticket or test\s+plan' 'Implementation payload leaked ticket/test plan'
Assert-Matches $coordinator '(?s)Test-discovered production defects.*?return to `implementation-agent`' 'Production recovery owner drift'
Assert-Matches $common 'status: passed\|blocked\|failed' 'Shared status vocabulary drift'
Assert-Matches $common 'skill_source: injected\|read\|unavailable' 'Skill-source vocabulary drift'
Assert-Matches $common '(?s)working directory if it contains.*?\.bootstrap-manifest\.json' 'Runtime root resolution lost project marker'
foreach ($text in @($implementer, $tester)) {
    Assert-Matches $text 'explicitly\s+read `<harness_root>/skills/' 'Missing reliable skill read fallback'
    Assert-Matches $text 'mode: qualification' 'Missing artifact-only worker mode'
    Assert-Matches $text 'read back|read\s+back' 'Missing effective edit read-back'
    Assert-Matches $text 'skill_source: unavailable' 'Missing skill failure reporting'
}
Assert-Matches $planner 'receives neither the raw ticket nor the test plan' 'Plan is not self-contained'
Assert-Matches $tester 'Never edit production code' 'Tester may take over production'
Assert-Matches $verifier 'FAIL requires `status: failed`' 'Verification failure gate missing'
Assert-Matches $common 'Normal stage calls.*not deleted canaries' 'Normal stages depend on cleaned-up probes'
Assert-Matches $planner 'Reuse an existing human decision instead of asking again' 'Approved no-op loops back to confirmation'
Assert-Matches $retro 'requirement, plan, skill, coordinator, worker, or runtime' 'Retro cause taxonomy missing'
Assert-Matches $retro '(?s)Do not edit files, create branches, commit, push,\s+or open a PR anywhere' 'Retro is not proposals-only'
Assert-Matches $retro 'not automatically a Git source checkout' 'Retro assumes installed copy is source'
Assert-Matches $retro 'facts from hypotheses|facts versus hypotheses' 'Retro does not distinguish hypotheses'
Assert-That ($retro -notmatch 'Create a new local branch|is a defect in') 'Predetermined blame or workflow edits returned'
Assert-Matches $bootstrap 'projectPathFromGitRoot' 'Bootstrap marker lacks nested project location'
Assert-Matches $bootstrap 'Do not resolve map\s+values against the Git root' 'Generation root contract missing'
Assert-Matches $bootstrap 'unverified until actually executed' 'Bootstrap overclaims runtime qualification'

Write-Output "PASS: $script:assertions assertions; 2 stack fixtures x 2 project layouts. Static rendering/contracts only; live App qualification not performed."
