Register-ArgumentCompleter -Native -CommandName .\workshop-branch.ps1 -ScriptBlock {
    param($wordToComplete, $commandAst, $cursorPosition)

    $tokens = $commandAst.CommandElements | ForEach-Object { $_.Extent.Text }
    $commands = @('list', 'branches', 'next', 'prev', 'goto', 'help')
    $phases = @('initial-state', 'initial', 'expand', 'migrate', 'contract', '1', '2', '3', '4')

    if ($tokens.Count -ge 2 -and $tokens[1] -eq 'goto') {
        $phases | Where-Object { $_ -like "$wordToComplete*" } | ForEach-Object {
            [System.Management.Automation.CompletionResult]::new($_, $_, 'ParameterValue', $_)
        }
        return
    }

    $commands | Where-Object { $_ -like "$wordToComplete*" } | ForEach-Object {
        [System.Management.Automation.CompletionResult]::new($_, $_, 'ParameterValue', $_)
    }
}
