# sonar-analysis.ps1

# Configura��es do SonarQube
$sonarUrl = "http://localhost:9000" # Altere para a URL do seu servidor SonarQube
$projectKey = "CadastroDePessoas"
$projectName = "Cadastro de Pessoas"
$projectVersion = "1.0"
$openCoverReportPath = "TestResults/coverage.opencover.xml"
$sonarToken = "" # Se necess�rio, adicione o token de autentica��o

# Verificar se as ferramentas do SonarQube est�o instaladas
$sonarScannerPath = Get-Command dotnet-sonarscanner -ErrorAction SilentlyContinue
if ($null -eq $sonarScannerPath) {
    Write-Host "Instalando dotnet-sonarscanner..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-sonarscanner
}

# Primeiro, execute os testes para gerar o relat�rio de cobertura
Write-Host "Executando testes com cobertura de c�digo..." -ForegroundColor Cyan
./run-tests-with-coverage.ps1

# Verificar se o relat�rio de cobertura foi gerado
if (!(Test-Path $openCoverReportPath)) {
    Write-Host "Arquivo de cobertura OpenCover n�o foi gerado. Execute os testes primeiro." -ForegroundColor Red
    exit 1
}

# Configurar par�metros do SonarQube
$sonarParameters = "/k:""$projectKey"" /n:""$projectName"" /v:""$projectVersion"" /d:sonar.host.url=""$sonarUrl"" /d:sonar.cs.opencover.reportsPaths=""$openCoverReportPath"" /d:sonar.coverage.exclusions=""**/*Tests.cs,**/*Testes.cs,**/Program.cs"""

# Adicionar token se fornecido
if ($sonarToken) {
    $sonarParameters += " /d:sonar.login=""$sonarToken"""
}

# Iniciar an�lise do SonarQube
Write-Host "Iniciando an�lise do SonarQube..." -ForegroundColor Cyan
Invoke-Expression "dotnet sonarscanner begin $sonarParameters"

# Compilar o projeto
Write-Host "Compilando o projeto..." -ForegroundColor Cyan
dotnet build --no-incremental

# Finalizar an�lise do SonarQube
Write-Host "Finalizando an�lise do SonarQube..." -ForegroundColor Cyan
if ($sonarToken) {
    dotnet sonarscanner end /d:sonar.login="$sonarToken"
} else {
    dotnet sonarscanner end
}

# Exibir resultados
Write-Host "An�lise conclu�da. Verifique os resultados no SonarQube:" -ForegroundColor Green
Write-Host "$sonarUrl/dashboard?id=$projectKey" -ForegroundColor Cyan

# Instru��es para executar o SonarQube localmente com Docker
Write-Host "`n= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =" -ForegroundColor Yellow
Write-Host "Para executar o SonarQube localmente com Docker:" -ForegroundColor Yellow
Write-Host "docker run -d --name sonarqube -p 9000:9000 sonarqube:latest" -ForegroundColor Cyan
Write-Host "Acesse: http://localhost:9000 (usu�rio/senha padr�o: admin/admin)" -ForegroundColor Cyan
Write-Host "= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =" -ForegroundColor Yellow