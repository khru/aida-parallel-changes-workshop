$ErrorActionPreference = 'Stop'

. "$PSScriptRoot/common.ps1"
Import-AidaEnv

if (Test-Path $env:AIDA_ALERT_SOUND) {
    try {
        $player = New-Object System.Media.SoundPlayer $env:AIDA_ALERT_SOUND
        $player.PlaySync()
        exit 0
    }
    catch {
    }
}

try {
    [console]::Beep(700, 700)
}
catch {
}
