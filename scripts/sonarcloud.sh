#!/bin/bash

# Load environment variables from .env file if it exists
if [[ -f ".env" ]]; then
    # Load variables from .env ignoring comments
    export $(grep -v '^#' .env | grep -v '^$' | xargs)
fi

# Define environment variables
SONAR_PROJECT_KEY=$SONAR_PROJECT_KEY
SONAR_ORGANIZATION_KEY=$SONAR_ORGANIZATION_KEY
SONAR_TOKEN=$SONAR_TOKEN
SONAR_HOST_URL="https://sonarcloud.io"
COVERAGE_FILE="./test-results/coverage.xml"

# Clean up old reports
if [[ -d "./test-results" ]]; then
    rm -rf ./test-results
fi

# Clean up previous build artifacts
if [[ -d "./.sonarqube" ]]; then
    rm -rf ./.sonarqube
fi

# Execute SonarCloud analysis
dotnet sonarscanner begin \
    -k:"${SONAR_PROJECT_KEY}" \
    -o:"${SONAR_ORGANIZATION_KEY}" \
    -d:sonar.token="${SONAR_TOKEN}" \
    -d:sonar.host.url="${SONAR_HOST_URL}" \
    -d:sonar.projectBaseDir="$(pwd)" \
    -d:sonar.language=cs \
    -d:sonar.cs.vscoveragexml.reportsPaths="${COVERAGE_FILE}" \
    -d:sonar.scanner.scanAll=false

# Build the project
dotnet build --no-incremental

# Collect test coverage
mkdir -p "$(dirname "${COVERAGE_FILE}")"
dotnet-coverage collect "dotnet test" -f xml -o "${COVERAGE_FILE}"

# Finalize SonarCloud analysis
dotnet sonarscanner end -d:sonar.token="${SONAR_TOKEN}"

echo ""
echo "✅ SonarCloud analysis completed successfully!"
echo ""
echo "📊 Access the report:"
echo "   - ${SONAR_HOST_URL}/project/overview?id=${SONAR_PROJECT_KEY}"
echo ""
