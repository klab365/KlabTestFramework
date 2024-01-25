set quiet

# build the solution
build:
	dotnet build

# clean the solution and remove all bin and obj folders
clean:
	dotnet clean
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
