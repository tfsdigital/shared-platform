#!/bin/bash
set -e

# Install local tools (reportgenerator)
dotnet tool restore

# Clean previous results
rm -rf coverage-results coverage-report

# Run all tests collecting coverage
dotnet test shared-platform.slnx \
  --collect:"XPlat Code Coverage" \
  --results-directory ./coverage-results \
  --no-restore

# Generate consolidated HTML report
dotnet reportgenerator \
  -reports:"coverage-results/**/coverage.cobertura.xml" \
  -targetdir:"coverage-report" \
  -reporttypes:"Html;TextSummary"

# Display summary in terminal
cat coverage-report/Summary.txt

echo ""
echo "Full report: coverage-report/index.html"
