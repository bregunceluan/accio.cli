<#
.SYNOPSIS
    Script para publicar uma aplica��o .NET como um arquivo �nico e copiar para a pasta .tools/accio do usu�rio.
.DESCRIPTION
    Este script executa o comando 'dotnet publish' com as configura��es para gerar um arquivo �nico (single file)
    para Windows x64 e depois copia os arquivos gerados para a pasta .tools/accio no diret�rio do usu�rio atual.
.PARAMETER ProjectPath
    Caminho para o arquivo de projeto .NET (.csproj, .fsproj, etc.)
.PARAMETER Configuration
    Configura��o de build (Debug/Release). Padr�o: Release
.EXAMPLE
    .\publish-accio.ps1 -ProjectPath .\MeuProjeto\MeuProjeto.csproj
.EXAMPLE
    .\publish-accio.ps1 -ProjectPath .\MeuProjeto\MeuProjeto.csproj -Configuration Debug
#>

# Define o caminho do projeto como sendo no mesmo n�vel do script
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectPath = Join-Path -Path $scriptDir -ChildPath "*.csproj"
$ProjectPath = Get-ChildItem -Path $ProjectPath | Select-Object -First 1 -ExpandProperty FullName

# Configura��o fixa como Release
$Configuration = "Release"

# Verificar se encontrou algum arquivo .csproj
if (-not $ProjectPath) {
    Write-Error "Nenhum arquivo .csproj encontrado no diret�rio do script."
    exit 1
}

Write-Host "Usando projeto: $ProjectPath" -ForegroundColor Cyan

# Obter o diret�rio do usu�rio de forma din�mica
$userHome = $env:USERPROFILE
# Ou alternativamente: $userHome = [Environment]::GetFolderPath([Environment+SpecialFolder]::UserProfile)

# Criar o caminho completo para o diret�rio de destino
$destinationPath = Join-Path -Path $userHome -ChildPath ".tools\accio"

# Como j� verificamos a exist�ncia do projeto anteriormente, este bloco n�o � mais necess�rio

# Obter o diret�rio do projeto para determinar onde os arquivos ser�o gerados
$projectDir = Split-Path -Parent $ProjectPath
$projectName = [System.IO.Path]::GetFileNameWithoutExtension($ProjectPath)
$publishOutputPath = Join-Path -Path $projectDir -ChildPath "bin\$Configuration\net8.0\win-x64\publish"

# Informar o usu�rio sobre as a��es
Write-Host "Publicando projeto: $ProjectPath" -ForegroundColor Cyan
Write-Host "Configura��o: $Configuration" -ForegroundColor Cyan
Write-Host "Destino: $destinationPath" -ForegroundColor Cyan

# Executar o comando publish
try {
    Write-Host "Executando dotnet publish..." -ForegroundColor Yellow
    dotnet publish $ProjectPath -c $Configuration -r win-x64 -p:PublishSingleFile=true -p:PublishReadyToRun=true --self-contained true
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Falha ao publicar o projeto. C�digo de sa�da: $LASTEXITCODE"
        exit $LASTEXITCODE
    }
    
    Write-Host "Publica��o conclu�da com sucesso!" -ForegroundColor Green
} catch {
    Write-Error "Erro ao executar dotnet publish: $_"
    exit 1
}

# Criar o diret�rio de destino se n�o existir
if (-not (Test-Path $destinationPath)) {
    Write-Host "Criando diret�rio de destino: $destinationPath" -ForegroundColor Yellow
    try {
        New-Item -Path $destinationPath -ItemType Directory -Force | Out-Null
    } catch {
        Write-Error "N�o foi poss�vel criar o diret�rio de destino: $_"
        exit 1
    }
}

# Copiar os arquivos publicados para o diret�rio de destino
Write-Host "Copiando arquivos para $destinationPath..." -ForegroundColor Yellow
try {
    Copy-Item -Path "$publishOutputPath\*" -Destination $destinationPath -Recurse -Force
    Write-Host "Arquivos copiados com sucesso!" -ForegroundColor Green
} catch {
    Write-Error "Erro ao copiar os arquivos: $_"
    exit 1
}

# Exibir caminho completo para o execut�vel
$exePath = Join-Path -Path $destinationPath -ChildPath "$projectName.exe"
if (Test-Path $exePath) {
    Write-Host "Aplica��o publicada em: $exePath" -ForegroundColor Green
    Write-Host "Para executar a aplica��o, digite: & '$exePath'" -ForegroundColor Cyan
Write-Host "Ou adicione a pasta ao PATH do sistema para executar de qualquer lugar" -ForegroundColor Cyan
} else {
    Write-Host "Arquivos copiados, mas o execut�vel principal n�o foi encontrado." -ForegroundColor Yellow
}

Write-Host "Processo conclu�do!" -ForegroundColor Green