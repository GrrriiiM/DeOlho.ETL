def exec(cmd) {
    if (isUnix()) {
        sh cmd
    }
    else {
        bat cmd
    }
}

node {
    stage('Git pull') {
        checkout scm
    }
    stage('Test') {
        exec('dotnet test src /p:CollectCoverage=true /p:Exclude="[xunit*]*" /p:CoverletOutputFormat="cobertura" /p:CoverletOutput=./coverage/')
    }
    stage('Code Coverage Report') {
        cobertura autoUpdateHealth: false, autoUpdateStability: false, coberturaReportFile: 'src/UnitTests/coverage/coverage.cobertura.xml', conditionalCoverageTargets: '70, 0, 0', lineCoverageTargets: '80, 0, 0', maxNumberOfBuilds: 0, methodCoverageTargets: '80, 0, 0', onlyStable: false, sourceEncoding: 'ASCII', zoomCoverageChart: false
    }
    stage('Pack nuget') {
        exec('dotnet pack src/DeOlho.ETL -c Release ')
    }
    stage('Push nuget') {
        exec('dotnet nuget push src/DeOlho.ETL/bin/Release -s DeOlho -k 730ebfc8d61bea02ac6a5262c8cca917 || true')
    }
}