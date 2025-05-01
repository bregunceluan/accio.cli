<#
.SYNOPSIS
    Script para publicar uma aplicação .NET como um arquivo único e copiar para a pasta .tools/accio do usuário.
.DESCRIPTION
    Este script executa o comando 'dotnet publish' com as configurações para gerar um arquivo único (single file)
    para Windows x64 e depois copia os arquivos gerados para a pasta .tools/accio no diretório do usuário atual.
.PARAMETER ProjectPath
    Caminho para o arquivo de projeto .NET (.csproj, .fsproj, etc.)
.PARAMETER Configuration
    Configuração de build (Debug/Release). Padrão: Release
.EXAMPLE
    .\publish-accio.ps1 -ProjectPath .\MeuProjeto\MeuProjeto.csproj
.EXAMPLE
    .\publish-accio.ps1 -ProjectPath .\MeuProjeto\MeuProjeto.csproj -Configuration Debug
#>

# Define o caminho do projeto como sendo no mesmo nível do script
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectPath = Join-Path -Path $scriptDir -ChildPath "*.csproj"
$ProjectPath = Get-ChildItem -Path $ProjectPath | Select-Object -First 1 -ExpandProperty FullName

# Configuração fixa como Release
$Configuration = "Release"

# Verificar se encontrou algum arquivo .csproj
if (-not $ProjectPath) {
    Write-Error "Nenhum arquivo .csproj encontrado no diretório do script."
    exit 1
}

Write-Host "Usando projeto: $ProjectPath" -ForegroundColor Cyan

# Obter o diretório do usuário de forma dinâmica
$userHome = $env:USERPROFILE
# Ou alternativamente: $userHome = [Environment]::GetFolderPath([Environment+SpecialFolder]::UserProfile)

# Criar o caminho completo para o diretório de destino
$destinationPath = Join-Path -Path $userHome -ChildPath ".tools\accio"

# Como já verificamos a existência do projeto anteriormente, este bloco não é mais necessário

# Obter o diretório do projeto para determinar onde os arquivos serão gerados
$projectDir = Split-Path -Parent $ProjectPath
$projectName = [System.IO.Path]::GetFileNameWithoutExtension($ProjectPath)
$publishOutputPath = Join-Path -Path $projectDir -ChildPath "bin\$Configuration\net8.0\win-x64\publish"

# Informar o usuário sobre as ações
Write-Host "Publicando projeto: $ProjectPath" -ForegroundColor Cyan
Write-Host "Configuração: $Configuration" -ForegroundColor Cyan
Write-Host "Destino: $destinationPath" -ForegroundColor Cyan

# Executar o comando publish
try {
    Write-Host "Executando dotnet publish..." -ForegroundColor Yellow
    dotnet publish $ProjectPath -c $Configuration -r win-x64 -p:PublishSingleFile=true -p:PublishReadyToRun=true --self-contained true
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Falha ao publicar o projeto. Código de saída: $LASTEXITCODE"
        exit $LASTEXITCODE
    }
    
    Write-Host "Publicação concluída com sucesso!" -ForegroundColor Green
} catch {
    Write-Error "Erro ao executar dotnet publish: $_"
    exit 1
}

# Criar o diretório de destino se não existir
if (-not (Test-Path $destinationPath)) {
    Write-Host "Criando diretório de destino: $destinationPath" -ForegroundColor Yellow
    try {
        New-Item -Path $destinationPath -ItemType Directory -Force | Out-Null
    } catch {
        Write-Error "Não foi possível criar o diretório de destino: $_"
        exit 1
    }
}

# Copiar os arquivos publicados para o diretório de destino
Write-Host "Copiando arquivos para $destinationPath..." -ForegroundColor Yellow
try {
    Copy-Item -Path "$publishOutputPath\*" -Destination $destinationPath -Recurse -Force
    Write-Host "Arquivos copiados com sucesso!" -ForegroundColor Green
} catch {
    Write-Error "Erro ao copiar os arquivos: $_"
    exit 1
}

# Exibir caminho completo para o executável
$exePath = Join-Path -Path $destinationPath -ChildPath "$projectName.exe"
if (Test-Path $exePath) {
    Write-Host "Aplicação publicada em: $exePath" -ForegroundColor Green
    Write-Host "Para executar a aplicação, digite: & '$exePath'" -ForegroundColor Cyan
Write-Host "Ou adicione a pasta ao PATH do sistema para executar de qualquer lugar" -ForegroundColor Cyan
} else {
    Write-Host "Arquivos copiados, mas o executável principal não foi encontrado." -ForegroundColor Yellow
}

Write-Host "Processo concluído!" -ForegroundColor Green