set quiet

# add projects to solution
slnadd:
    dotnet sln add ls -r **/*.csproj

# build the solution
build:
    dotnet build \
        KlabTestFramework.sln \
        /property:GenerateFullPaths=true \
        /consoleloggerparameters:"NoSummary;ForceNoAlign;"

# clean the solution and remove all bin and obj folders
clean:
    find . -iname "bin" | xargs rm -rf
    find . -iname "obj" | xargs rm -rf

# test the solution
test:
    dotnet test

# format the solution
format:
    dotnet format -v diag

# run the workflow example
workflowex:
    dotnet run --project ./samples/workflow/WorkflowConsoleExample/WorkflowConsoleExample.csproj

build_systemex:
    dotnet build ./samples/system/SystemConsoleExample/SystemConsoleExample.csproj

run-systemex:
    dotnet run --project ./samples/system/SystemConsoleExample/SystemConsoleExample.csproj
