git -c core.filemode=false status --short --branch
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

git show-ref --heads --abbrev | Select-String "refs/heads/(main|workshop/initial-state|workshop/expand|workshop/migrate|workshop/contract)$"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

git merge-base --is-ancestor main workshop/initial-state
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

git merge-base --is-ancestor workshop/initial-state workshop/expand
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

git merge-base --is-ancestor workshop/expand workshop/migrate
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

git merge-base --is-ancestor workshop/migrate workshop/contract
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Output "ancestry-ok"

git rev-list --merges --count main..workshop/initial-state
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

git rev-list --merges --count workshop/initial-state..workshop/expand
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

git rev-list --merges --count workshop/expand..workshop/migrate
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

git rev-list --merges --count workshop/migrate..workshop/contract
