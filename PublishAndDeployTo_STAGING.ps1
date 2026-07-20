set-strictmode -version latest

$destRoot = '\\euvm-appstg\d$\inetpub\wwwroot\SecureShareAdmin'

$appOfflinePath = join-path $destRoot 'app_offline.htm'

"Please wait, the application is being updated..." | set-content -literalpath $appOfflinePath

$projectFile = join-path $psscriptroot 'SecureShareAdmin\SecureShareAdmin.csproj'

$publishOutput = join-path $psscriptroot 'publish\staging'

dotnet publish $projectFile -c Release -r win-x64 --self-contained true -o $publishOutput -p:DeleteExistingFiles=true

robocopy $publishOutput $destRoot /e /w:1 /ndl 

remove-item -literalpath $appOfflinePath
