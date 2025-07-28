# run-tests-with-coverage.ps1

$outputFolder = "TestResults"
$coberturaFile = "$outputFolder/coverage.cobertura.xml"
$openCoverFile = "$outputFolder/coverage.opencover.xml"
$reportFolder = "$outputFolder/CoverageReport"

# Criar diret�rio de sa�da se n�o existir
if (!(Test-Path $outputFolder)) {
    New-Item -ItemType Directory -Path $outputFolder | Out-Null
}

# Limpar relat�rios anteriores
if (Test-Path $reportFolder) {
    Remove-Item -Path $reportFolder -Recurse -Force
}

Write-Host "Executando testes com cobertura de c�digo..." -ForegroundColor Cyan

# Executar testes com cobertura - gerando em dois formatos
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat="cobertura,opencover" /p:CoverletOutput=./$outputFolder/coverage /p:MergeWith=./$outputFolder/coverage.json /p:Include="[CadastroDePessoas.Domain]*,[CadastroDePessoas.Application]*,[CadastroDePessoas.Infraestructure]*,[CadastroDePessoas.API]*" /p:Exclude="[*Tests]*,[*Testes]*"

# Verificar se o teste foi bem-sucedido
if ($LASTEXITCODE -ne 0) {
    Write-Host "Falha ao executar os testes." -ForegroundColor Red
    exit $LASTEXITCODE
}

# Verificar se os arquivos de cobertura foram gerados
if (!(Test-Path $coberturaFile) -or !(Test-Path $openCoverFile)) {
    Write-Host "Arquivos de cobertura n�o foram gerados." -ForegroundColor Red
    exit 1
}

# Instalar reportgenerator global se n�o estiver instalado
$reportGenPath = Get-Command reportgenerator -ErrorAction SilentlyContinue
if ($null -eq $reportGenPath) {
    Write-Host "Instalando reportgenerator..." -ForegroundColor Yellow
    dotnet tool install -g dotnet-reportgenerator-globaltool
}

# Gerar relat�rio HTML
Write-Host "Gerando relat�rio de cobertura..." -ForegroundColor Cyan
reportgenerator -reports:$coberturaFile -targetdir:$reportFolder -reporttypes:Html

# Calcular a porcentagem de cobertura
$coverageXml = [xml](Get-Content $coberturaFile)
$lineCoverage = $coverageXml.coverage.lines.coveredlines / $coverageXml.coverage.lines.line.Count * 100
$branchCoverage = if ($coverageXml.coverage.branches.branchesvalid -gt 0) { $coverageXml.coverage.branches.covered / $coverageXml.coverage.branches.branchesvalid * 100 } else { 0 }

Write-Host "Cobertura de Linhas: $([math]::Round($lineCoverage, 2))%" -ForegroundColor Green
Write-Host "Cobertura de Branches: $([math]::Round($branchCoverage, 2))%" -ForegroundColor Green

# Verificar se atingiu 80% de cobertura
if ($lineCoverage -lt 80) {
    Write-Host "A cobertura de linhas est� abaixo de 80%." -ForegroundColor Yellow
} else {
    Write-Host "Meta de cobertura de c�digo atingida!" -ForegroundColor Green
}

# Abrir o relat�rio no navegador
$reportPath = Resolve-Path "$reportFolder/index.html"
Write-Host "Relat�rio gerado em: $reportPath" -ForegroundColor Cyan
Write-Host "Para visualizar o relat�rio, abra o arquivo no navegador." -ForegroundColor Cyan

# Opcional: abrir automaticamente o relat�rio no navegador padr�o
# Start-Process $reportPath