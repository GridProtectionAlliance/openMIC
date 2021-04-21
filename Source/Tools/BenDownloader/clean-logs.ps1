# Run this script in the BenDownloader.logs folder
# This script is set up to do a dry run
# Remove the -WhatIf flag at the end of the script to remove logs

$now = [DateTime]::UtcNow
$threshold = ($now - [TimeSpan]::FromDays(30))

$oldLogs = Get-ChildItem . -Directory |
	ForEach-Object { Get-ChildItem $_ -Directory } |
	Where-Object {
		$logName = $_.Name
		$logTime = [DateTime]::MinValue
		$isOld = $false

		if ([DateTime]::TryParseExact($logName, "yyyy-MM-dd HH.mm", $null, "AssumeUniversal", [ref]$logTime)) {
			$isOld = $logTime -lt $threshold
		}

		$isOld
	}

if (-not $oldLogs) {
	"Nothing to do"
	return
}

$oldLogs | Remove-Item -Recurse -WhatIf
"Removed $($oldLogs.Count) logs"